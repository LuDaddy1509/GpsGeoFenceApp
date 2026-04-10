#if ANDROID
using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.OS;

namespace MauiApp1.Services.Audio
{
    public sealed class AndroidAudioPlayer : IAudioPlayer
    {
        private readonly object _gate = new();
        private MediaPlayer? _player;
        private CancellationTokenSource? _cts;

        // ── AudioFocus ────────────────────────────────────────────────
        private readonly AudioManager? _audioManager;
        private AudioFocusRequestClass? _focusRequest;
        private bool _pausedByFocus = false;

        public AndroidAudioPlayer()
        {
            _audioManager = (AudioManager?)
                global::Android.App.Application.Context.GetSystemService(Context.AudioService);
        }

        private bool RequestAudioFocus()
        {
            if (_audioManager == null) return true;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var attrs = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Speech)
                    .SetUsage(AudioUsageKind.Media)
                    .Build();

                _focusRequest = new AudioFocusRequestClass.Builder(AudioFocus.Gain)
                    .SetAudioAttributes(attrs!)
                    .SetAcceptsDelayedFocusGain(false)
                    .SetOnAudioFocusChangeListener(new FocusListener(this))
                    .Build();

                return _audioManager.RequestAudioFocus(_focusRequest) == AudioFocusRequest.Granted;
            }
            else
            {
#pragma warning disable CA1422
                return _audioManager.RequestAudioFocus(
                    new FocusListener(this),
                    Stream.Music,
                    AudioFocus.Gain) == AudioFocusRequest.Granted;
#pragma warning restore CA1422
            }
        }

        private void AbandonAudioFocus()
        {
            if (_audioManager == null) return;
            if (_focusRequest != null && Build.VERSION.SdkInt >= BuildVersionCodes.O)
                _audioManager.AbandonAudioFocusRequest(_focusRequest);
        }

        private sealed class FocusListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
        {
            private readonly AndroidAudioPlayer _owner;
            public FocusListener(AndroidAudioPlayer owner) => _owner = owner;

            public void OnAudioFocusChange(AudioFocus focusChange)
            {
                switch (focusChange)
                {
                    case AudioFocus.Loss:
                    case AudioFocus.LossTransient:
                        // Cuộc gọi đến hoặc app khác chiếm âm thanh — pause
                        lock (_owner._gate)
                        {
                            _owner._pausedByFocus = _owner._player?.IsPlaying == true;
                            try { _owner._player?.Pause(); } catch { /* ignore */ }
                        }
                        break;

                    case AudioFocus.Gain:
                        // Cuộc gọi kết thúc — resume nếu bị pause bởi focus
                        lock (_owner._gate)
                        {
                            if (_owner._pausedByFocus)
                            {
                                try { _owner._player?.Start(); } catch { /* ignore */ }
                                _owner._pausedByFocus = false;
                            }
                        }
                        break;

                    case AudioFocus.LossTransientCanDuck:
                        // Notification ngắn — giảm volume thay vì dừng hẳn
                        lock (_owner._gate)
                        {
                            try { _owner._player?.SetVolume(0.2f, 0.2f); } catch { /* ignore */ }
                        }
                        break;
                }
            }
        }

        // ── Playback ──────────────────────────────────────────────────

        public async Task PlayFileAsync(string filePath, CancellationToken ct = default)
        {
            Stop();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var token = _cts.Token;

            var tcs = new TaskCompletionSource();

            try
            {
                var player = new MediaPlayer();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var attrs = new AudioAttributes.Builder()
                        .SetContentType(AudioContentType.Speech)
                        .SetUsage(AudioUsageKind.Media)
                        .Build();
                    player.SetAudioAttributes(attrs);
                }

                player.SetDataSource(filePath);
                player.Completion += (_, __) => tcs.TrySetResult();

                lock (_gate)
                {
                    _player = player;
                }

                player.Prepare();

                // Xin AudioFocus trước khi phát
                if (!RequestAudioFocus())
                {
                    System.Diagnostics.Debug.WriteLine("[Audio] AudioFocus denied, skip play");
                    tcs.TrySetResult();
                    return;
                }

                player.Start();

                using var reg = token.Register(() =>
                {
                    try
                    {
                        lock (_gate)
                        {
                            if (_player != null && _player.IsPlaying)
                                _player.Stop();
                        }
                    }
                    catch { /* ignore */ }
                    tcs.TrySetCanceled();
                });

                await tcs.Task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // bị cancel từ Stop() hoặc ct ngoài
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidAudioPlayer] {ex.Message}");
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            lock (_gate)
            {
                try { _cts?.Cancel(); } catch { /* ignore */ }

                try
                {
                    if (_player != null)
                    {
                        if (_player.IsPlaying)
                            _player.Stop();
                        _player.Reset();
                        _player.Release();
                    }
                }
                catch { /* ignore */ }

                _player = null;
                _cts?.Dispose();
                _cts = null;
            }

            AbandonAudioFocus();
        }
    }
}
#endif
