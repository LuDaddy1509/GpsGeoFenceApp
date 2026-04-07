; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [370 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1110 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 324
	i64 u0x0071cf2d27b7d61e, ; 2: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 297
	i64 u0x01109b0e4d99e61f, ; 3: System.ComponentModel.Annotations.dll => 13
	i64 u0x01af0bd6467d518e, ; 4: lib_ZXing.Net.MAUI.dll.so => 329
	i64 u0x02123411c4e01926, ; 5: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 283
	i64 u0x022e81ea9c46e03a, ; 6: lib_CommunityToolkit.Maui.Core.dll.so => 180
	i64 u0x022f31be406de945, ; 7: Microsoft.Extensions.Options.ConfigurationExtensions => 205
	i64 u0x0284512fad379f7e, ; 8: System.Runtime.Handles => 107
	i64 u0x02a4c5a44384f885, ; 9: Microsoft.Extensions.Caching.Memory => 189
	i64 u0x02abedc11addc1ed, ; 10: lib_Mono.Android.Runtime.dll.so => 174
	i64 u0x02f55bf70672f5c8, ; 11: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x032267b2a94db371, ; 12: lib_Xamarin.AndroidX.AppCompat.dll.so => 233
	i64 u0x03621c804933a890, ; 13: System.Buffers => 7
	i64 u0x0363ac97a4cb84e6, ; 14: SQLitePCLRaw.provider.e_sqlite3.dll => 221
	i64 u0x0399610510a38a38, ; 15: lib_System.Private.DataContractSerialization.dll.so => 88
	i64 u0x043032f1d071fae0, ; 16: ru/Microsoft.Maui.Controls.resources => 355
	i64 u0x044440a55165631e, ; 17: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 333
	i64 u0x046eb1581a80c6b0, ; 18: vi/Microsoft.Maui.Controls.resources => 361
	i64 u0x0470607fd33c32db, ; 19: Microsoft.IdentityModel.Abstractions.dll => 207
	i64 u0x047408741db2431a, ; 20: Xamarin.AndroidX.DynamicAnimation => 257
	i64 u0x0517ef04e06e9f76, ; 21: System.Net.Primitives => 72
	i64 u0x051a3be159e4ef99, ; 22: Xamarin.GooglePlayServices.Tasks => 319
	i64 u0x0565d18c6da3de38, ; 23: Xamarin.AndroidX.RecyclerView => 289
	i64 u0x057bf9fa9fb09f7c, ; 24: Microsoft.Data.Sqlite.dll => 183
	i64 u0x0581db89237110e9, ; 25: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 26: Microsoft.Maui.dll => 214
	i64 u0x05a1c25e78e22d87, ; 27: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 104
	i64 u0x05ef98b6a1db882c, ; 28: lib_Microsoft.Data.Sqlite.dll.so => 183
	i64 u0x06076b5d2b581f08, ; 29: zh-HK/Microsoft.Maui.Controls.resources => 362
	i64 u0x06388ffe9f6c161a, ; 30: System.Xml.Linq.dll => 159
	i64 u0x06600c4c124cb358, ; 31: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 32: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 262
	i64 u0x0680a433c781bb3d, ; 33: Xamarin.AndroidX.Collection.Jvm => 244
	i64 u0x069fff96ec92a91d, ; 34: System.Xml.XPath.dll => 164
	i64 u0x070b0847e18dab68, ; 35: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 259
	i64 u0x0739448d84d3b016, ; 36: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 301
	i64 u0x07469f2eecce9e85, ; 37: mscorlib.dll => 170
	i64 u0x07c57877c7ba78ad, ; 38: ru/Microsoft.Maui.Controls.resources.dll => 355
	i64 u0x07dcdc7460a0c5e4, ; 39: System.Collections.NonGeneric => 10
	i64 u0x08122e52765333c8, ; 40: lib_Microsoft.Extensions.Logging.Debug.dll.so => 203
	i64 u0x088610fc2509f69e, ; 41: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 302
	i64 u0x08a7c865576bbde7, ; 42: System.Reflection.Primitives => 98
	i64 u0x08c9d051a4a817e5, ; 43: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 255
	i64 u0x08f3c9788ee2153c, ; 44: Xamarin.AndroidX.DrawerLayout => 256
	i64 u0x09138715c92dba90, ; 45: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 46: lib_Microsoft.Extensions.Options.dll.so => 204
	i64 u0x092266563089ae3e, ; 47: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x098b50f911ccea8d, ; 48: lib_Xamarin.GooglePlayServices.Basement.dll.so => 316
	i64 u0x09d144a7e214d457, ; 49: System.Security.Cryptography => 129
	i64 u0x09e2b9f743db21a8, ; 50: lib_System.Reflection.Metadata.dll.so => 97
	i64 u0x0a805f95d98f597b, ; 51: lib_Microsoft.Extensions.Caching.Abstractions.dll.so => 188
	i64 u0x0abb3e2b271edc45, ; 52: System.Threading.Channels.dll => 143
	i64 u0x0b06b1feab070143, ; 53: System.Formats.Tar => 39
	i64 u0x0b3b632c3bbee20c, ; 54: sk/Microsoft.Maui.Controls.resources => 356
	i64 u0x0b6aff547b84fbe9, ; 55: Xamarin.KotlinX.Serialization.Core.Jvm => 327
	i64 u0x0be2e1f8ce4064ed, ; 56: Xamarin.AndroidX.ViewPager => 304
	i64 u0x0c3ca6cc978e2aae, ; 57: pt-BR/Microsoft.Maui.Controls.resources => 352
	i64 u0x0c3d7adcdb333bf0, ; 58: Xamarin.AndroidX.Camera.Lifecycle => 240
	i64 u0x0c3dd9438f54f672, ; 59: lib_Xamarin.GooglePlayServices.Maps.dll.so => 318
	i64 u0x0c59ad9fbbd43abe, ; 60: Mono.Android => 175
	i64 u0x0c65741e86371ee3, ; 61: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 227
	i64 u0x0c74af560004e816, ; 62: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 63: lib_Microsoft.Maui.Essentials.dll.so => 215
	i64 u0x0c83c82812e96127, ; 64: lib_System.Net.Mail.dll.so => 68
	i64 u0x0cce4bce83380b7f, ; 65: Xamarin.AndroidX.Security.SecurityCrypto => 294
	i64 u0x0d13cd7cce4284e4, ; 66: System.Security.SecureString => 132
	i64 u0x0d63f4f73521c24f, ; 67: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 293
	i64 u0x0e04e702012f8463, ; 68: Xamarin.AndroidX.Emoji2 => 258
	i64 u0x0e14e73a54dda68e, ; 69: lib_System.Net.NameResolution.dll.so => 69
	i64 u0x0f37dd7a62ae99af, ; 70: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 245
	i64 u0x0f5e7abaa7cf470a, ; 71: System.Net.HttpListener => 67
	i64 u0x1001f97bbe242e64, ; 72: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102a31b45304b1da, ; 73: Xamarin.AndroidX.CustomView => 254
	i64 u0x1065c4cb554c3d75, ; 74: System.IO.IsolatedStorage.dll => 52
	i64 u0x10f6cfcbcf801616, ; 75: System.IO.Compression.Brotli => 43
	i64 u0x114443cdcf2091f1, ; 76: System.Security.Cryptography.Primitives => 127
	i64 u0x118d570f508803d1, ; 77: Xamarin.AndroidX.Camera.Camera2.dll => 238
	i64 u0x11a603952763e1d4, ; 78: System.Net.Mail => 68
	i64 u0x11a70d0e1009fb11, ; 79: System.Net.WebSockets.dll => 83
	i64 u0x11f26371eee0d3c1, ; 80: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 272
	i64 u0x11fbe62d469cc1c8, ; 81: Microsoft.VisualStudio.DesignTools.TapContract.dll => 367
	i64 u0x12128b3f59302d47, ; 82: lib_System.Xml.Serialization.dll.so => 161
	i64 u0x123639456fb056da, ; 83: System.Reflection.Emit.Lightweight.dll => 94
	i64 u0x12521e9764603eaa, ; 84: lib_System.Resources.Reader.dll.so => 101
	i64 u0x125b7f94acb989db, ; 85: Xamarin.AndroidX.RecyclerView.dll => 289
	i64 u0x12d3b63863d4ab0b, ; 86: lib_System.Threading.Overlapped.dll.so => 144
	i64 u0x134eab1061c395ee, ; 87: System.Transactions => 154
	i64 u0x138567fa954faa55, ; 88: Xamarin.AndroidX.Browser => 237
	i64 u0x13a01de0cbc3f06c, ; 89: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 339
	i64 u0x13beedefb0e28a45, ; 90: lib_System.Xml.XmlDocument.dll.so => 165
	i64 u0x13f1e5e209e91af4, ; 91: lib_Java.Interop.dll.so => 172
	i64 u0x13f1e880c25d96d1, ; 92: he/Microsoft.Maui.Controls.resources => 340
	i64 u0x143d8ea60a6a4011, ; 93: Microsoft.Extensions.DependencyInjection.Abstractions => 194
	i64 u0x1497051b917530bd, ; 94: lib_System.Net.WebSockets.dll.so => 83
	i64 u0x14b78ce3adce0011, ; 95: Microsoft.VisualStudio.DesignTools.TapContract => 367
	i64 u0x14d612a531c79c05, ; 96: Xamarin.JSpecify.dll => 321
	i64 u0x14e68447938213b7, ; 97: Xamarin.AndroidX.Collection.Ktx.dll => 245
	i64 u0x152a448bd1e745a7, ; 98: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 99: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x15bdc156ed462f2f, ; 100: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 101: System.Resources.Writer.dll => 103
	i64 u0x16054fdcb6b3098b, ; 102: Microsoft.Extensions.DependencyModel.dll => 195
	i64 u0x16bf2a22df043a09, ; 103: System.IO.Pipes.dll => 56
	i64 u0x16ea2b318ad2d830, ; 104: System.Security.Cryptography.Algorithms => 122
	i64 u0x16eeae54c7ebcc08, ; 105: System.Reflection.dll => 100
	i64 u0x17125c9a85b4929f, ; 106: lib_netstandard.dll.so => 171
	i64 u0x1716866f7416792e, ; 107: lib_System.Security.AccessControl.dll.so => 120
	i64 u0x174f71c46216e44a, ; 108: Xamarin.KotlinX.Coroutines.Core => 324
	i64 u0x1752c12f1e1fc00c, ; 109: System.Core => 21
	i64 u0x17b56e25558a5d36, ; 110: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 343
	i64 u0x17f9358913beb16a, ; 111: System.Text.Encodings.Web => 139
	i64 u0x1809fb23f29ba44a, ; 112: lib_System.Reflection.TypeExtensions.dll.so => 99
	i64 u0x18402a709e357f3b, ; 113: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 327
	i64 u0x18a9befae51bb361, ; 114: System.Net.WebClient => 79
	i64 u0x18f0ce884e87d89a, ; 115: nb/Microsoft.Maui.Controls.resources.dll => 349
	i64 u0x19777fba3c41b398, ; 116: Xamarin.AndroidX.Startup.StartupRuntime.dll => 296
	i64 u0x19a4c090f14ebb66, ; 117: System.Security.Claims => 121
	i64 u0x1a040febb58bf51e, ; 118: lib_Xamarin.AndroidX.Camera.View.dll.so => 241
	i64 u0x1a91866a319e9259, ; 119: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1aac34d1917ba5d3, ; 120: lib_System.dll.so => 168
	i64 u0x1aad60783ffa3e5b, ; 121: lib-th-Microsoft.Maui.Controls.resources.dll.so => 358
	i64 u0x1aea8f1c3b282172, ; 122: lib_System.Net.Ping.dll.so => 71
	i64 u0x1b4b7a1d0d265fa2, ; 123: Xamarin.Android.Glide.DiskLruCache => 226
	i64 u0x1bbdb16cfa73e785, ; 124: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 273
	i64 u0x1bc766e07b2b4241, ; 125: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 290
	i64 u0x1c292b1598348d77, ; 126: Microsoft.Extensions.Diagnostics.dll => 196
	i64 u0x1c753b5ff15bce1b, ; 127: Mono.Android.Runtime.dll => 174
	i64 u0x1cd47467799d8250, ; 128: System.Threading.Tasks.dll => 148
	i64 u0x1d23eafdc6dc346c, ; 129: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 130: Xamarin.AndroidX.Security.SecurityCrypto.dll => 294
	i64 u0x1db6820994506bf5, ; 131: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dbb0c2c6a999acb, ; 132: System.Diagnostics.StackTrace => 30
	i64 u0x1e3d87657e9659bc, ; 133: Xamarin.AndroidX.Navigation.UI => 285
	i64 u0x1e71143913d56c10, ; 134: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 347
	i64 u0x1e7c31185e2fb266, ; 135: lib_System.Threading.Tasks.Parallel.dll.so => 147
	i64 u0x1ed8fcce5e9b50a0, ; 136: Microsoft.Extensions.Options.dll => 204
	i64 u0x1f055d15d807e1b2, ; 137: System.Xml.XmlSerializer => 166
	i64 u0x1f1ed22c1085f044, ; 138: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f61df9c5b94d2c1, ; 139: lib_System.Numerics.dll.so => 86
	i64 u0x1f750bb5421397de, ; 140: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 298
	i64 u0x20237ea48006d7a8, ; 141: lib_System.Net.WebClient.dll.so => 79
	i64 u0x209375905fcc1bad, ; 142: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20fab3cf2dfbc8df, ; 143: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x2110167c128cba15, ; 144: System.Globalization => 42
	i64 u0x21419508838f7547, ; 145: System.Runtime.CompilerServices.VisualC => 105
	i64 u0x2174319c0d835bc9, ; 146: System.Runtime => 119
	i64 u0x2198e5bc8b7153fa, ; 147: Xamarin.AndroidX.Annotation.Experimental.dll => 231
	i64 u0x219ea1b751a4dee4, ; 148: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 149: System.Reflection.Emit.ILGeneration => 93
	i64 u0x220fd4f2e7c48170, ; 150: th/Microsoft.Maui.Controls.resources => 358
	i64 u0x224538d85ed15a82, ; 151: System.IO.Pipes => 56
	i64 u0x22908438c6bed1af, ; 152: lib_System.Threading.Timer.dll.so => 151
	i64 u0x22fbc14e981e3b45, ; 153: lib_Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll.so => 366
	i64 u0x2347c268e3e4e536, ; 154: Xamarin.GooglePlayServices.Basement.dll => 316
	i64 u0x237be844f1f812c7, ; 155: System.Threading.Thread.dll => 149
	i64 u0x23807c59646ec4f3, ; 156: lib_Microsoft.EntityFrameworkCore.dll.so => 184
	i64 u0x23852b3bdc9f7096, ; 157: System.Resources.ResourceManager => 102
	i64 u0x23986dd7e5d4fc01, ; 158: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x2407aef2bbe8fadf, ; 159: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 160: Xamarin.AndroidX.Core.dll => 250
	i64 u0x247619fe4413f8bf, ; 161: System.Runtime.Serialization.Primitives.dll => 116
	i64 u0x24de8d301281575e, ; 162: Xamarin.Android.Glide => 224
	i64 u0x252073cc3caa62c2, ; 163: fr/Microsoft.Maui.Controls.resources.dll => 339
	i64 u0x256b8d41255f01b1, ; 164: Xamarin.Google.Crypto.Tink.Android => 312
	i64 u0x25a0a7eff76ea08e, ; 165: SQLitePCLRaw.batteries_v2.dll => 218
	i64 u0x25e1850d10cdc8f7, ; 166: lib_Xamarin.AndroidX.Camera.Camera2.dll.so => 238
	i64 u0x2662c629b96b0b30, ; 167: lib_Xamarin.Kotlin.StdLib.dll.so => 322
	i64 u0x268c1439f13bcc29, ; 168: lib_Microsoft.Extensions.Primitives.dll.so => 206
	i64 u0x268f1dca6d06d437, ; 169: Xamarin.AndroidX.Camera.Core => 239
	i64 u0x26a670e154a9c54b, ; 170: System.Reflection.Extensions.dll => 96
	i64 u0x26d077d9678fe34f, ; 171: System.IO.dll => 58
	i64 u0x273f3515de5faf0d, ; 172: id/Microsoft.Maui.Controls.resources.dll => 344
	i64 u0x2742545f9094896d, ; 173: hr/Microsoft.Maui.Controls.resources => 342
	i64 u0x274d85d83ad40513, ; 174: lib_Xamarin.AndroidX.Window.WindowCore.dll.so => 307
	i64 u0x2759af78ab94d39b, ; 175: System.Net.WebSockets => 83
	i64 u0x27b2b16f3e9de038, ; 176: Xamarin.Google.Crypto.Tink.Android.dll => 312
	i64 u0x27b410442fad6cf1, ; 177: Java.Interop.dll => 172
	i64 u0x27b97e0d52c3034a, ; 178: System.Diagnostics.Debug => 26
	i64 u0x2801845a2c71fbfb, ; 179: System.Net.Primitives.dll => 72
	i64 u0x286835e259162700, ; 180: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 288
	i64 u0x28e52865585a1ebe, ; 181: Microsoft.Extensions.Diagnostics.Abstractions.dll => 197
	i64 u0x2949f3617a02c6b2, ; 182: Xamarin.AndroidX.ExifInterface => 260
	i64 u0x29aeab763a527e52, ; 183: lib_Xamarin.AndroidX.Navigation.Common.Android.dll.so => 281
	i64 u0x29f947844fb7fc11, ; 184: Microsoft.Maui.Controls.HotReload.Forms => 365
	i64 u0x2a128783efe70ba0, ; 185: uk/Microsoft.Maui.Controls.resources.dll => 360
	i64 u0x2a3b095612184159, ; 186: lib_System.Net.NetworkInformation.dll.so => 70
	i64 u0x2a6507a5ffabdf28, ; 187: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 188: fi/Microsoft.Maui.Controls.resources => 338
	i64 u0x2ad5d6b13b7a3e04, ; 189: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 190: System.Text.RegularExpressions.dll => 141
	i64 u0x2afc1c4f898552ee, ; 191: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 192: zh-Hant/Microsoft.Maui.Controls.resources.dll => 364
	i64 u0x2b6989d78cba9a15, ; 193: Xamarin.AndroidX.Concurrent.Futures.dll => 246
	i64 u0x2c8bd14bb93a7d82, ; 194: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 351
	i64 u0x2cbd9262ca785540, ; 195: lib_System.Text.Encoding.CodePages.dll.so => 136
	i64 u0x2cc9e1fed6257257, ; 196: lib_System.Reflection.Emit.Lightweight.dll.so => 94
	i64 u0x2cd723e9fe623c7c, ; 197: lib_System.Private.Xml.Linq.dll.so => 90
	i64 u0x2ce03196fe1170d2, ; 198: Microsoft.Maui.Controls.Maps.dll => 212
	i64 u0x2d169d318a968379, ; 199: System.Threading.dll => 152
	i64 u0x2d20145f27cfc1d2, ; 200: Xamarin.AndroidX.Window.WindowCore.Jvm.dll => 308
	i64 u0x2d47774b7d993f59, ; 201: sv/Microsoft.Maui.Controls.resources.dll => 357
	i64 u0x2d5ffcae1ad0aaca, ; 202: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 203: System.Text.Json.dll => 140
	i64 u0x2dcaa0bb15a4117a, ; 204: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e2ced2c3c6a1edc, ; 205: lib_System.Threading.AccessControl.dll.so => 142
	i64 u0x2e5a40c319acb800, ; 206: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 207: el/Microsoft.Maui.Controls.resources.dll => 336
	i64 u0x2ef0c22aee1f75b2, ; 208: lib_AutoMapper.dll.so => 178
	i64 u0x2f02f94df3200fe5, ; 209: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 210: System.Xml.ReaderWriter => 160
	i64 u0x2f5911d9ba814e4e, ; 211: System.Diagnostics.Tracing => 34
	i64 u0x2f84070a459bc31f, ; 212: lib_System.Xml.dll.so => 167
	i64 u0x2feb4d2fcda05cfd, ; 213: Microsoft.Extensions.Caching.Abstractions.dll => 188
	i64 u0x2ff49de6a71764a1, ; 214: lib_Microsoft.Extensions.Http.dll.so => 200
	i64 u0x309ee9eeec09a71e, ; 215: lib_Xamarin.AndroidX.Fragment.dll.so => 261
	i64 u0x309f2bedefa9a318, ; 216: Microsoft.IdentityModel.Abstractions => 207
	i64 u0x30bde19041cd89dd, ; 217: lib_Microsoft.Maui.Maps.dll.so => 217
	i64 u0x30c6dda129408828, ; 218: System.IO.IsolatedStorage => 52
	i64 u0x31195fef5d8fb552, ; 219: _Microsoft.Android.Resource.Designer.dll => 369
	i64 u0x312c8ed623cbfc8d, ; 220: Xamarin.AndroidX.Window.dll => 306
	i64 u0x31496b779ed0663d, ; 221: lib_System.Reflection.DispatchProxy.dll.so => 92
	i64 u0x32243413e774362a, ; 222: Xamarin.AndroidX.CardView.dll => 242
	i64 u0x3235427f8d12dae1, ; 223: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 224: fr/Microsoft.Maui.Controls.resources => 339
	i64 u0x32aa989ff07a84ff, ; 225: lib_System.Xml.ReaderWriter.dll.so => 160
	i64 u0x33829542f112d59b, ; 226: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 227: lib-es-Microsoft.Maui.Controls.resources.dll.so => 337
	i64 u0x341abc357fbb4ebf, ; 228: lib_System.Net.Sockets.dll.so => 78
	i64 u0x346a212343615ac5, ; 229: lib_System.Linq.AsyncEnumerable.dll.so => 59
	i64 u0x3496c1e2dcaf5ecc, ; 230: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 231: Microsoft.Maui => 214
	i64 u0x34e292762d9615df, ; 232: cs/Microsoft.Maui.Controls.resources.dll => 333
	i64 u0x3508234247f48404, ; 233: Microsoft.Maui.Controls => 211
	i64 u0x353590da528c9d22, ; 234: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 235: lib_Xamarin.AndroidX.ViewPager2.dll.so => 305
	i64 u0x355282fc1c909694, ; 236: Microsoft.Extensions.Configuration => 190
	i64 u0x3552fc5d578f0fbf, ; 237: Xamarin.AndroidX.Arch.Core.Common => 235
	i64 u0x355c649948d55d97, ; 238: lib_System.Runtime.Intrinsics.dll.so => 111
	i64 u0x35ea9d1c6834bc8c, ; 239: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 276
	i64 u0x3628ab68db23a01a, ; 240: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x3673b042508f5b6b, ; 241: lib_System.Runtime.Extensions.dll.so => 106
	i64 u0x36740f1a8ecdc6c4, ; 242: System.Numerics => 86
	i64 u0x36b2b50fdf589ae2, ; 243: System.Reflection.Emit.Lightweight => 94
	i64 u0x36cada77dc79928b, ; 244: System.IO.MemoryMappedFiles => 53
	i64 u0x374ef46b06791af6, ; 245: System.Reflection.Primitives.dll => 98
	i64 u0x376bf93e521a5417, ; 246: lib_Xamarin.Jetbrains.Annotations.dll.so => 320
	i64 u0x37bc29f3183003b6, ; 247: lib_System.IO.dll.so => 58
	i64 u0x380134e03b1e160a, ; 248: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 249: System.Runtime.CompilerServices.Unsafe => 104
	i64 u0x385c17636bb6fe6e, ; 250: Xamarin.AndroidX.CustomView.dll => 254
	i64 u0x38869c811d74050e, ; 251: System.Net.NameResolution.dll => 69
	i64 u0x3889cbdca0f2c57c, ; 252: Xamarin.GooglePlayServices.Location.dll => 317
	i64 u0x393c226616977fdb, ; 253: lib_Xamarin.AndroidX.ViewPager.dll.so => 304
	i64 u0x395e37c3334cf82a, ; 254: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 332
	i64 u0x39c3107c28752af1, ; 255: lib_Microsoft.Extensions.FileProviders.Abstractions.dll.so => 198
	i64 u0x3ab5859054645f72, ; 256: System.Security.Cryptography.Primitives.dll => 127
	i64 u0x3ad75090c3fac0e9, ; 257: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 290
	i64 u0x3ae44ac43a1fbdbb, ; 258: System.Runtime.Serialization => 118
	i64 u0x3b860f9932505633, ; 259: lib_System.Text.Encoding.Extensions.dll.so => 137
	i64 u0x3be99b43dd39dd37, ; 260: Xamarin.AndroidX.SavedState.SavedState.Android => 292
	i64 u0x3bea9ebe8c027c01, ; 261: lib_Microsoft.IdentityModel.Tokens.dll.so => 210
	i64 u0x3c3aafb6b3a00bf6, ; 262: lib_System.Security.Cryptography.X509Certificates.dll.so => 128
	i64 u0x3c4049146b59aa90, ; 263: System.Runtime.InteropServices.JavaScript => 108
	i64 u0x3c7c495f58ac5ee9, ; 264: Xamarin.Kotlin.StdLib => 322
	i64 u0x3c7e5ed3d5db71bb, ; 265: System.Security => 133
	i64 u0x3cd9d281d402eb9b, ; 266: Xamarin.AndroidX.Browser.dll => 237
	i64 u0x3ced6a4f3010aa96, ; 267: ZXing.Net.MAUI.Controls => 330
	i64 u0x3d1c50cc001a991e, ; 268: Xamarin.Google.Guava.ListenableFuture.dll => 314
	i64 u0x3d2b1913edfc08d7, ; 269: lib_System.Threading.ThreadPool.dll.so => 150
	i64 u0x3d46f0b995082740, ; 270: System.Xml.Linq => 159
	i64 u0x3d8a8f400514a790, ; 271: Xamarin.AndroidX.Fragment.Ktx.dll => 262
	i64 u0x3d9c2a242b040a50, ; 272: lib_Xamarin.AndroidX.Core.dll.so => 250
	i64 u0x3da7781d6333a8fe, ; 273: SQLitePCLRaw.batteries_v2 => 218
	i64 u0x3dbb6b9f5ab90fa7, ; 274: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 257
	i64 u0x3e5441657549b213, ; 275: Xamarin.AndroidX.ResourceInspection.Annotation => 290
	i64 u0x3e57d4d195c53c2e, ; 276: System.Reflection.TypeExtensions => 99
	i64 u0x3e616ab4ed1f3f15, ; 277: lib_System.Data.dll.so => 24
	i64 u0x3f1d226e6e06db7e, ; 278: Xamarin.AndroidX.SlidingPaneLayout.dll => 295
	i64 u0x3f510adf788828dd, ; 279: System.Threading.Tasks.Extensions => 146
	i64 u0x3f6f5914291cdcf7, ; 280: Microsoft.Extensions.Hosting.Abstractions => 199
	i64 u0x407a10bb4bf95829, ; 281: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 280
	i64 u0x40c98b6bd77346d4, ; 282: Microsoft.VisualBasic.dll => 3
	i64 u0x41833cf766d27d96, ; 283: mscorlib => 170
	i64 u0x41cab042be111c34, ; 284: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 234
	i64 u0x423a9ecc4d905a88, ; 285: lib_System.Resources.ResourceManager.dll.so => 102
	i64 u0x423bf51ae7def810, ; 286: System.Xml.XPath => 164
	i64 u0x42462ff15ddba223, ; 287: System.Resources.Reader.dll => 101
	i64 u0x4291015ff4e5ef71, ; 288: Xamarin.AndroidX.Core.ViewTree.dll => 252
	i64 u0x42a31b86e6ccc3f0, ; 289: System.Diagnostics.Contracts => 25
	i64 u0x430e95b891249788, ; 290: lib_System.Reflection.Emit.dll.so => 95
	i64 u0x43375950ec7c1b6a, ; 291: netstandard.dll => 171
	i64 u0x434c4e1d9284cdae, ; 292: Mono.Android.dll => 175
	i64 u0x43505013578652a0, ; 293: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 229
	i64 u0x437d06c381ed575a, ; 294: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x43950f84de7cc79a, ; 295: pl/Microsoft.Maui.Controls.resources.dll => 351
	i64 u0x43c077442b230f64, ; 296: Xamarin.AndroidX.Tracing.Tracing.Android => 299
	i64 u0x43e8ca5bc927ff37, ; 297: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 259
	i64 u0x4452351a38cdea7f, ; 298: lib_MauiApp1.dll.so => 0
	i64 u0x448bd33429269b19, ; 299: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 300: lib_System.Runtime.Serialization.Primitives.dll.so => 116
	i64 u0x4515080865a951a5, ; 301: Xamarin.Kotlin.StdLib.dll => 322
	i64 u0x453c1277f85cf368, ; 302: lib_Microsoft.EntityFrameworkCore.Abstractions.dll.so => 185
	i64 u0x4545802489b736b9, ; 303: Xamarin.AndroidX.Fragment.Ktx => 262
	i64 u0x454b4d1e66bb783c, ; 304: Xamarin.AndroidX.Lifecycle.Process => 269
	i64 u0x45c40276a42e283e, ; 305: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 306: System.AppContext.dll => 6
	i64 u0x45fcc9fd66f25095, ; 307: Microsoft.Extensions.DependencyModel => 195
	i64 u0x46a4213bc97fe5ae, ; 308: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 355
	i64 u0x47358bd471172e1d, ; 309: lib_System.Xml.Linq.dll.so => 159
	i64 u0x47daf4e1afbada10, ; 310: pt/Microsoft.Maui.Controls.resources => 353
	i64 u0x480c0a47dd42dd81, ; 311: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x49e952f19a4e2022, ; 312: System.ObjectModel => 87
	i64 u0x49f9e6948a8131e4, ; 313: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 303
	i64 u0x4a5667b2462a664b, ; 314: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 285
	i64 u0x4a7a18981dbd56bc, ; 315: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 316: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 268
	i64 u0x4b07a0ed0ab33ff4, ; 317: System.Runtime.Extensions.dll => 106
	i64 u0x4b576d47ac054f3c, ; 318: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b7b6532ded934b7, ; 319: System.Text.Json => 140
	i64 u0x4c2029a97af23a8d, ; 320: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android => 278
	i64 u0x4c7755cf07ad2d5f, ; 321: System.Net.Http.Json.dll => 65
	i64 u0x4c9caee94c082049, ; 322: Xamarin.GooglePlayServices.Maps => 318
	i64 u0x4ca014ceac582c86, ; 323: Microsoft.EntityFrameworkCore.Relational.dll => 186
	i64 u0x4cc5f15266470798, ; 324: lib_Xamarin.AndroidX.Loader.dll.so => 279
	i64 u0x4cf6f67dc77aacd2, ; 325: System.Net.NetworkInformation.dll => 70
	i64 u0x4d3183dd245425d4, ; 326: System.Net.WebSockets.Client.dll => 82
	i64 u0x4d343a9e3a12e594, ; 327: AutoMapper.dll => 178
	i64 u0x4d479f968a05e504, ; 328: System.Linq.Expressions.dll => 60
	i64 u0x4d55a010ffc4faff, ; 329: System.Private.Xml => 91
	i64 u0x4d5cbe77561c5b2e, ; 330: System.Web.dll => 157
	i64 u0x4d77512dbd86ee4c, ; 331: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 235
	i64 u0x4d7793536e79c309, ; 332: System.ServiceProcess => 135
	i64 u0x4d95fccc1f67c7ca, ; 333: System.Runtime.Loader.dll => 112
	i64 u0x4db014bf0ff1c9c1, ; 334: System.Linq.AsyncEnumerable => 59
	i64 u0x4dcf44c3c9b076a2, ; 335: it/Microsoft.Maui.Controls.resources.dll => 345
	i64 u0x4dd9247f1d2c3235, ; 336: Xamarin.AndroidX.Loader.dll => 279
	i64 u0x4e2aeee78e2c4a87, ; 337: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 288
	i64 u0x4e32f00cb0937401, ; 338: Mono.Android.Runtime => 174
	i64 u0x4e5eea4668ac2b18, ; 339: System.Text.Encoding.CodePages => 136
	i64 u0x4ebd0c4b82c5eefc, ; 340: lib_System.Threading.Channels.dll.so => 143
	i64 u0x4ee8eaa9c9c1151a, ; 341: System.Globalization.Calendars => 40
	i64 u0x4f21ee6ef9eb527e, ; 342: ca/Microsoft.Maui.Controls.resources => 332
	i64 u0x4fd5f3ee53d0a4f0, ; 343: SQLitePCLRaw.lib.e_sqlite3.android => 220
	i64 u0x4fdc964ec1888e25, ; 344: lib_Microsoft.Extensions.Configuration.Binder.dll.so => 192
	i64 u0x4ffd65baff757598, ; 345: Microsoft.IdentityModel.Tokens => 210
	i64 u0x5037f0be3c28c7a3, ; 346: lib_Microsoft.Maui.Controls.dll.so => 211
	i64 u0x506203448c473a65, ; 347: Xamarin.Google.AutoValue.Annotations => 310
	i64 u0x50c3a29b21050d45, ; 348: System.Linq.Parallel.dll => 61
	i64 u0x5112ed116d87baf8, ; 349: CommunityToolkit.Mvvm => 181
	i64 u0x5116b21580ae6eb0, ; 350: Microsoft.Extensions.Configuration.Binder.dll => 192
	i64 u0x5131bbe80989093f, ; 351: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 275
	i64 u0x516324a5050a7e3c, ; 352: System.Net.WebProxy => 81
	i64 u0x516d6f0b21a303de, ; 353: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x51bb8a2afe774e32, ; 354: System.Drawing => 36
	i64 u0x5247c5c32a4140f0, ; 355: System.Resources.Reader => 101
	i64 u0x526bb15e3c386364, ; 356: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 272
	i64 u0x526ce79eb8e90527, ; 357: lib_System.Net.Primitives.dll.so => 72
	i64 u0x52829f00b4467c38, ; 358: lib_System.Data.Common.dll.so => 22
	i64 u0x529ffe06f39ab8db, ; 359: Xamarin.AndroidX.Core => 250
	i64 u0x52ff996554dbf352, ; 360: Microsoft.Maui.Graphics => 216
	i64 u0x535f7e40e8fef8af, ; 361: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 356
	i64 u0x53978aac584c666e, ; 362: lib_System.Security.Cryptography.Cng.dll.so => 123
	i64 u0x53a96d5c86c9e194, ; 363: System.Net.NetworkInformation => 70
	i64 u0x53be1038a61e8d44, ; 364: System.Runtime.InteropServices.RuntimeInformation.dll => 109
	i64 u0x53c3014b9437e684, ; 365: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 362
	i64 u0x5435e6f049e9bc37, ; 366: System.Security.Claims.dll => 121
	i64 u0x54795225dd1587af, ; 367: lib_System.Runtime.dll.so => 119
	i64 u0x547a34f14e5f6210, ; 368: Xamarin.AndroidX.Lifecycle.Common.dll => 264
	i64 u0x54b851bc9b470503, ; 369: Xamarin.AndroidX.Navigation.Common.Android => 281
	i64 u0x556e8b63b660ab8b, ; 370: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 265
	i64 u0x5588627c9a108ec9, ; 371: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 372: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55fa0c610fe93bb1, ; 373: lib_System.Security.Cryptography.OpenSsl.dll.so => 126
	i64 u0x56442b99bc64bb47, ; 374: System.Runtime.Serialization.Xml.dll => 117
	i64 u0x56a8b26e1aeae27b, ; 375: System.Threading.Tasks.Dataflow => 145
	i64 u0x56f932d61e93c07f, ; 376: System.Globalization.Extensions => 41
	i64 u0x571c5cfbec5ae8e2, ; 377: System.Private.Uri => 89
	i64 u0x576499c9f52fea31, ; 378: Xamarin.AndroidX.Annotation => 230
	i64 u0x578cd35c91d7b347, ; 379: lib_SQLitePCLRaw.core.dll.so => 219
	i64 u0x579a06fed6eec900, ; 380: System.Private.CoreLib.dll => 177
	i64 u0x57adda3c951abb33, ; 381: Microsoft.Extensions.Hosting.Abstractions.dll => 199
	i64 u0x57c542c14049b66d, ; 382: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 383: System.Threading.Timer => 151
	i64 u0x584ac38e21d2fde1, ; 384: Microsoft.Extensions.Configuration.Binder => 192
	i64 u0x58601b2dda4a27b9, ; 385: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 346
	i64 u0x58688d9af496b168, ; 386: Microsoft.Extensions.DependencyInjection.dll => 193
	i64 u0x588c167a79db6bfb, ; 387: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 313
	i64 u0x5906028ae5151104, ; 388: Xamarin.AndroidX.Activity.Ktx => 229
	i64 u0x595a356d23e8da9a, ; 389: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59f9e60b9475085f, ; 390: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 231
	i64 u0x5a6967a493d5295b, ; 391: MauiApp1.dll => 0
	i64 u0x5a745f5101a75527, ; 392: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 393: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 249
	i64 u0x5a8f6699f4a1caa9, ; 394: lib_System.Threading.dll.so => 152
	i64 u0x5ae9cd33b15841bf, ; 395: System.ComponentModel => 18
	i64 u0x5b54391bdc6fcfe6, ; 396: System.Private.DataContractSerialization => 88
	i64 u0x5b5f0e240a06a2a2, ; 397: da/Microsoft.Maui.Controls.resources.dll => 334
	i64 u0x5b755276902c8414, ; 398: Xamarin.GooglePlayServices.Base => 315
	i64 u0x5b8109e8e14c5e3e, ; 399: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 400: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 268
	i64 u0x5bdf16b09da116ab, ; 401: Xamarin.AndroidX.Collection => 243
	i64 u0x5c019d5266093159, ; 402: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 273
	i64 u0x5c30a4a35f9cc8c4, ; 403: lib_System.Reflection.Extensions.dll.so => 96
	i64 u0x5c393624b8176517, ; 404: lib_Microsoft.Extensions.Logging.dll.so => 201
	i64 u0x5c53c29f5073b0c9, ; 405: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c87463c575c7616, ; 406: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5d0a4a29b02d9d3c, ; 407: System.Net.WebHeaderCollection.dll => 80
	i64 u0x5d1b514fc45c92d4, ; 408: ZXing.Net.MAUI => 329
	i64 u0x5d40c9b15181641f, ; 409: lib_Xamarin.AndroidX.Emoji2.dll.so => 258
	i64 u0x5d6ca10d35e9485b, ; 410: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 246
	i64 u0x5d7ec76c1c703055, ; 411: System.Threading.Tasks.Parallel => 147
	i64 u0x5db0cbbd1028510e, ; 412: lib_System.Runtime.InteropServices.dll.so => 110
	i64 u0x5db30905d3e5013b, ; 413: Xamarin.AndroidX.Collection.Jvm.dll => 244
	i64 u0x5e467bc8f09ad026, ; 414: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 415: System.Runtime.Handles.dll => 107
	i64 u0x5ea92fdb19ec8c4c, ; 416: System.Text.Encodings.Web.dll => 139
	i64 u0x5eb8046dd40e9ac3, ; 417: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 418: System.Security.Cryptography.Csp.dll => 124
	i64 u0x5eee1376d94c7f5e, ; 419: System.Net.HttpListener.dll => 67
	i64 u0x5f36ccf5c6a57e24, ; 420: System.Xml.ReaderWriter.dll => 160
	i64 u0x5f4294b9b63cb842, ; 421: System.Data.Common => 22
	i64 u0x5f7399e166075632, ; 422: lib_SQLitePCLRaw.lib.e_sqlite3.android.dll.so => 220
	i64 u0x5f9a2d823f664957, ; 423: lib-el-Microsoft.Maui.Controls.resources.dll.so => 336
	i64 u0x5fa6da9c3cd8142a, ; 424: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 326
	i64 u0x5fac98e0b37a5b9d, ; 425: System.Runtime.CompilerServices.Unsafe.dll => 104
	i64 u0x609f4b7b63d802d4, ; 426: lib_Microsoft.Extensions.DependencyInjection.dll.so => 193
	i64 u0x60cd4e33d7e60134, ; 427: Xamarin.KotlinX.Coroutines.Core.Jvm => 325
	i64 u0x60f62d786afcf130, ; 428: System.Memory => 64
	i64 u0x61bb78c89f867353, ; 429: System.IO => 58
	i64 u0x61be8d1299194243, ; 430: Microsoft.Maui.Controls.Xaml => 213
	i64 u0x61d2cba29557038f, ; 431: de/Microsoft.Maui.Controls.resources => 335
	i64 u0x61d88f399afb2f45, ; 432: lib_System.Runtime.Loader.dll.so => 112
	i64 u0x622eef6f9e59068d, ; 433: System.Private.CoreLib => 177
	i64 u0x639fb99a7bef11de, ; 434: Xamarin.AndroidX.Navigation.Runtime.Android.dll => 284
	i64 u0x63cdbd66ac39bb46, ; 435: lib_Microsoft.VisualStudio.DesignTools.XamlTapContract.dll.so => 368
	i64 u0x63d5e3aa4ef9b931, ; 436: Xamarin.KotlinX.Coroutines.Android.dll => 323
	i64 u0x63f1f6883c1e23c2, ; 437: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 438: Xamarin.Google.Android.Material.dll => 309
	i64 u0x640e3b14dbd325c2, ; 439: System.Security.Cryptography.Algorithms.dll => 122
	i64 u0x64587004560099b9, ; 440: System.Reflection => 100
	i64 u0x64b1529a438a3c45, ; 441: lib_System.Runtime.Handles.dll.so => 107
	i64 u0x64b61dd9da8a4d57, ; 442: System.Net.ServerSentEvents.dll => 76
	i64 u0x6565fba2cd8f235b, ; 443: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 276
	i64 u0x658f524e4aba7dad, ; 444: CommunityToolkit.Maui.dll => 179
	i64 u0x65ecac39144dd3cc, ; 445: Microsoft.Maui.Controls.dll => 211
	i64 u0x65ece51227bfa724, ; 446: lib_System.Runtime.Numerics.dll.so => 113
	i64 u0x661722438787b57f, ; 447: Xamarin.AndroidX.Annotation.Jvm.dll => 232
	i64 u0x6679b2337ee6b22a, ; 448: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x667c66a03dd97d40, ; 449: System.Linq.AsyncEnumerable.dll => 59
	i64 u0x6692e924eade1b29, ; 450: lib_System.Console.dll.so => 20
	i64 u0x66a4e5c6a3fb0bae, ; 451: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 275
	i64 u0x66d13304ce1a3efa, ; 452: Xamarin.AndroidX.CursorAdapter => 253
	i64 u0x674303f65d8fad6f, ; 453: lib_System.Net.Quic.dll.so => 73
	i64 u0x6756ca4cad62e9d6, ; 454: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 248
	i64 u0x67c0802770244408, ; 455: System.Windows.dll => 158
	i64 u0x68100b69286e27cd, ; 456: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 457: lib-da-Microsoft.Maui.Controls.resources.dll.so => 334
	i64 u0x6872ec7a2e36b1ac, ; 458: System.Drawing.Primitives.dll => 35
	i64 u0x68fbbbe2eb455198, ; 459: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 460: he/Microsoft.Maui.Controls.resources.dll => 340
	i64 u0x699dffb2427a2d71, ; 461: SQLitePCLRaw.lib.e_sqlite3.android.dll => 220
	i64 u0x6a4d7577b2317255, ; 462: System.Runtime.InteropServices.dll => 110
	i64 u0x6ace3b74b15ee4a4, ; 463: nb/Microsoft.Maui.Controls.resources => 349
	i64 u0x6afcedb171067e2b, ; 464: System.Core.dll => 21
	i64 u0x6bef98e124147c24, ; 465: Xamarin.Jetbrains.Annotations => 320
	i64 u0x6ce874bff138ce2b, ; 466: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 274
	i64 u0x6d12bfaa99c72b1f, ; 467: lib_Microsoft.Maui.Graphics.dll.so => 216
	i64 u0x6d70755158ca866e, ; 468: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 469: Microsoft.Extensions.Primitives => 206
	i64 u0x6d7eeca99577fc8b, ; 470: lib_System.Net.WebProxy.dll.so => 81
	i64 u0x6d8515b19946b6a2, ; 471: System.Net.WebProxy.dll => 81
	i64 u0x6d86d56b84c8eb71, ; 472: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 253
	i64 u0x6d9bea6b3e895cf7, ; 473: Microsoft.Extensions.Primitives.dll => 206
	i64 u0x6e25a02c3833319a, ; 474: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 282
	i64 u0x6e79c6bd8627412a, ; 475: Xamarin.AndroidX.SavedState.SavedState.Ktx => 293
	i64 u0x6e838d9a2a6f6c9e, ; 476: lib_System.ValueTuple.dll.so => 155
	i64 u0x6e9965ce1095e60a, ; 477: lib_System.Core.dll.so => 21
	i64 u0x6fd2265da78b93a4, ; 478: lib_Microsoft.Maui.dll.so => 214
	i64 u0x6fdfc7de82c33008, ; 479: cs/Microsoft.Maui.Controls.resources => 333
	i64 u0x6ffc4967cc47ba57, ; 480: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 481: System.IO.FileSystem.dll => 51
	i64 u0x70e99f48c05cb921, ; 482: tr/Microsoft.Maui.Controls.resources.dll => 359
	i64 u0x70fd3deda22442d2, ; 483: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 349
	i64 u0x71485e7ffdb4b958, ; 484: System.Reflection.Extensions => 96
	i64 u0x7162a2fce67a945f, ; 485: lib_Xamarin.Android.Glide.Annotations.dll.so => 225
	i64 u0x717530326f808838, ; 486: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 197
	i64 u0x71a495ea3761dde8, ; 487: lib-it-Microsoft.Maui.Controls.resources.dll.so => 345
	i64 u0x71ad672adbe48f35, ; 488: System.ComponentModel.Primitives.dll => 16
	i64 u0x720f102581a4a5c8, ; 489: Xamarin.AndroidX.Core.ViewTree => 252
	i64 u0x725f5a9e82a45c81, ; 490: System.Security.Cryptography.Encoding => 125
	i64 u0x72b1fb4109e08d7b, ; 491: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 342
	i64 u0x72e0300099accce1, ; 492: System.Xml.XPath.XDocument => 163
	i64 u0x730bfb248998f67a, ; 493: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 494: Xamarin.Google.ErrorProne.Annotations.dll => 313
	i64 u0x734b76fdc0dc05bb, ; 495: lib_GoogleGson.dll.so => 182
	i64 u0x73a6be34e822f9d1, ; 496: lib_System.Runtime.Serialization.dll.so => 118
	i64 u0x73e4ce94e2eb6ffc, ; 497: lib_System.Memory.dll.so => 64
	i64 u0x73f2645914262879, ; 498: lib_Microsoft.EntityFrameworkCore.Sqlite.dll.so => 187
	i64 u0x74327ad47b9741a4, ; 499: Xamarin.AndroidX.NavigationEvent.Android.dll => 287
	i64 u0x743a1eccf080489a, ; 500: WindowsBase.dll => 169
	i64 u0x746cf89b511b4d40, ; 501: lib_Microsoft.Extensions.Diagnostics.dll.so => 196
	i64 u0x755a91767330b3d4, ; 502: lib_Microsoft.Extensions.Configuration.dll.so => 190
	i64 u0x75c326eb821b85c4, ; 503: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 504: lib_Xamarin.AndroidX.SavedState.dll.so => 291
	i64 u0x76ca07b878f44da0, ; 505: System.Runtime.Numerics.dll => 113
	i64 u0x7736c8a96e51a061, ; 506: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 232
	i64 u0x778a805e625329ef, ; 507: System.Linq.Parallel => 61
	i64 u0x77bf40592cd67602, ; 508: Xamarin.Google.AutoValue.Annotations.dll => 310
	i64 u0x77d9074d8f33a303, ; 509: lib_System.Net.ServerSentEvents.dll.so => 76
	i64 u0x77f8a4acc2fdc449, ; 510: System.Security.Cryptography.Cng.dll => 123
	i64 u0x780bc73597a503a9, ; 511: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 348
	i64 u0x782c5d8eb99ff201, ; 512: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 513: th/Microsoft.Maui.Controls.resources.dll => 358
	i64 u0x78a45e51311409b6, ; 514: Xamarin.AndroidX.Fragment.dll => 261
	i64 u0x78ed4ab8f9d800a1, ; 515: Xamarin.AndroidX.Lifecycle.ViewModel => 274
	i64 u0x7a090e7cbb6c0ed1, ; 516: Xamarin.GooglePlayServices.Location => 317
	i64 u0x7a25bdb29108c6e7, ; 517: Microsoft.Extensions.Http => 200
	i64 u0x7a5207a7c82d30b4, ; 518: lib_Xamarin.JSpecify.dll.so => 321
	i64 u0x7a7e7eddf79c5d26, ; 519: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 274
	i64 u0x7a9a57d43b0845fa, ; 520: System.AppContext => 6
	i64 u0x7ad0f4f1e5d08183, ; 521: Xamarin.AndroidX.Collection.dll => 243
	i64 u0x7adb8da2ac89b647, ; 522: fi/Microsoft.Maui.Controls.resources.dll => 338
	i64 u0x7b13d9eaa944ade8, ; 523: Xamarin.AndroidX.DynamicAnimation.dll => 257
	i64 u0x7b150145c0a9058c, ; 524: Microsoft.Data.Sqlite => 183
	i64 u0x7b4927e421291c41, ; 525: Microsoft.IdentityModel.JsonWebTokens.dll => 208
	i64 u0x7bef86a4335c4870, ; 526: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 527: sk/Microsoft.Maui.Controls.resources.dll => 356
	i64 u0x7c2a0bd1e0f988fc, ; 528: lib-de-Microsoft.Maui.Controls.resources.dll.so => 335
	i64 u0x7c41d387501568ba, ; 529: System.Net.WebClient.dll => 79
	i64 u0x7c482cd79bd24b13, ; 530: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 247
	i64 u0x7c60acf6404e96b6, ; 531: Xamarin.AndroidX.Navigation.Common.Android.dll => 281
	i64 u0x7c8cb8cf04bee12b, ; 532: lib_Xamarin.Google.AutoValue.Annotations.dll.so => 310
	i64 u0x7cb95ad2a929d044, ; 533: Xamarin.GooglePlayServices.Basement => 316
	i64 u0x7cc637f941f716d0, ; 534: CommunityToolkit.Maui.Core => 180
	i64 u0x7cd2ec8eaf5241cd, ; 535: System.Security.dll => 133
	i64 u0x7cf9ae50dd350622, ; 536: Xamarin.Jetbrains.Annotations.dll => 320
	i64 u0x7d649b75d580bb42, ; 537: ms/Microsoft.Maui.Controls.resources.dll => 348
	i64 u0x7d8ee2bdc8e3aad1, ; 538: System.Numerics.Vectors => 85
	i64 u0x7df5df8db8eaa6ac, ; 539: Microsoft.Extensions.Logging.Debug => 203
	i64 u0x7dfc3d6d9d8d7b70, ; 540: System.Collections => 12
	i64 u0x7e2e564fa2f76c65, ; 541: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 542: lib_System.Security.Claims.dll.so => 121
	i64 u0x7e4465b3f78ad8d0, ; 543: Xamarin.KotlinX.Serialization.Core.dll => 326
	i64 u0x7e571cad5915e6c3, ; 544: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 269
	i64 u0x7e6ac99e4e8df72f, ; 545: System.IO.Hashing => 176
	i64 u0x7e6b1ca712437d7d, ; 546: Xamarin.AndroidX.Emoji2.ViewsHelper => 259
	i64 u0x7e946809d6008ef2, ; 547: lib_System.ObjectModel.dll.so => 87
	i64 u0x7ea0272c1b4a9635, ; 548: lib_Xamarin.Android.Glide.dll.so => 224
	i64 u0x7eb4f0dc47488736, ; 549: lib_Xamarin.GooglePlayServices.Tasks.dll.so => 319
	i64 u0x7ecc13347c8fd849, ; 550: lib_System.ComponentModel.dll.so => 18
	i64 u0x7f00ddd9b9ca5a13, ; 551: Xamarin.AndroidX.ViewPager.dll => 304
	i64 u0x7f9351cd44b1273f, ; 552: Microsoft.Extensions.Configuration.Abstractions => 191
	i64 u0x7fbd557c99b3ce6f, ; 553: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 267
	i64 u0x8076a9a44a2ca331, ; 554: System.Net.Quic => 73
	i64 u0x80b7e726b0280681, ; 555: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 366
	i64 u0x80da183a87731838, ; 556: System.Reflection.Metadata => 97
	i64 u0x80fa55b6d1b0be99, ; 557: SQLitePCLRaw.provider.e_sqlite3 => 221
	i64 u0x812c069d5cdecc17, ; 558: System.dll => 168
	i64 u0x81381be520a60adb, ; 559: Xamarin.AndroidX.Interpolator.dll => 263
	i64 u0x81657cec2b31e8aa, ; 560: System.Net => 84
	i64 u0x81ab745f6c0f5ce6, ; 561: zh-Hant/Microsoft.Maui.Controls.resources => 364
	i64 u0x8277f2be6b5ce05f, ; 562: Xamarin.AndroidX.AppCompat => 233
	i64 u0x828f06563b30bc50, ; 563: lib_Xamarin.AndroidX.CardView.dll.so => 242
	i64 u0x82b399cb01b531c4, ; 564: lib_System.Web.dll.so => 157
	i64 u0x82df8f5532a10c59, ; 565: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 566: lib_System.Transactions.dll.so => 154
	i64 u0x82f6403342e12049, ; 567: uk/Microsoft.Maui.Controls.resources => 360
	i64 u0x83a7afd2c49adc86, ; 568: lib_Microsoft.IdentityModel.Abstractions.dll.so => 207
	i64 u0x83c14ba66c8e2b8c, ; 569: zh-Hans/Microsoft.Maui.Controls.resources => 363
	i64 u0x844ac8f64fd78edc, ; 570: Xamarin.AndroidX.Camera.View.dll => 241
	i64 u0x846ce984efea52c7, ; 571: System.Threading.Tasks.Parallel.dll => 147
	i64 u0x84ae73148a4557d2, ; 572: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 573: System.Runtime.Serialization.Json.dll => 115
	i64 u0x84cd5cdec0f54bcc, ; 574: lib_Microsoft.EntityFrameworkCore.Relational.dll.so => 186
	i64 u0x850c5ba0b57ce8e7, ; 575: lib_Xamarin.AndroidX.Collection.dll.so => 243
	i64 u0x851d02edd334b044, ; 576: Xamarin.AndroidX.VectorDrawable => 301
	i64 u0x85c919db62150978, ; 577: Xamarin.AndroidX.Transition.dll => 300
	i64 u0x8662aaeb94fef37f, ; 578: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 579: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 364
	i64 u0x86b3e00c36b84509, ; 580: Microsoft.Extensions.Configuration.dll => 190
	i64 u0x86b62cb077ec4fd7, ; 581: System.Runtime.Serialization.Xml => 117
	i64 u0x8706ffb12bf3f53d, ; 582: Xamarin.AndroidX.Annotation.Experimental => 231
	i64 u0x872a5b14c18d328c, ; 583: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 584: Xamarin.Android.Glide.Annotations.dll => 225
	i64 u0x87c4b8a492b176ad, ; 585: Microsoft.EntityFrameworkCore.Abstractions => 185
	i64 u0x87c69b87d9283884, ; 586: lib_System.Threading.Thread.dll.so => 149
	i64 u0x87f6569b25707834, ; 587: System.IO.Compression.Brotli.dll => 43
	i64 u0x8842b3a5d2d3fb36, ; 588: Microsoft.Maui.Essentials => 215
	i64 u0x88926583efe7ee86, ; 589: Xamarin.AndroidX.Activity.Ktx.dll => 229
	i64 u0x88ba6bc4f7762b03, ; 590: lib_System.Reflection.dll.so => 100
	i64 u0x88bda98e0cffb7a9, ; 591: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 325
	i64 u0x8930322c7bd8f768, ; 592: netstandard => 171
	i64 u0x897a606c9e39c75f, ; 593: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x898a5c6bc9e47ec1, ; 594: lib_Xamarin.AndroidX.SavedState.SavedState.Android.dll.so => 292
	i64 u0x89911a22005b92b7, ; 595: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 596: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 109
	i64 u0x8a19e3dc71b34b2c, ; 597: System.Reflection.TypeExtensions.dll => 99
	i64 u0x8a399a706fcbce4b, ; 598: Microsoft.Extensions.Caching.Abstractions => 188
	i64 u0x8ad229ea26432ee2, ; 599: Xamarin.AndroidX.Loader => 279
	i64 u0x8b4ff5d0fdd5faa1, ; 600: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 601: System.Security.Principal.Windows => 130
	i64 u0x8b8d01333a96d0b5, ; 602: System.Diagnostics.Process.dll => 29
	i64 u0x8b9ceca7acae3451, ; 603: lib-he-Microsoft.Maui.Controls.resources.dll.so => 340
	i64 u0x8c575135aa1ccef4, ; 604: Microsoft.Extensions.FileProviders.Abstractions => 198
	i64 u0x8cb8f612b633affb, ; 605: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 293
	i64 u0x8cdfdb4ce85fb925, ; 606: lib_System.Security.Principal.Windows.dll.so => 130
	i64 u0x8cdfe7b8f4caa426, ; 607: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 608: Xamarin.AndroidX.CursorAdapter.dll => 253
	i64 u0x8d52a25632e81824, ; 609: Microsoft.EntityFrameworkCore.Sqlite.dll => 187
	i64 u0x8d52f7ea2796c531, ; 610: Xamarin.AndroidX.Emoji2.dll => 258
	i64 u0x8d7b8ab4b3310ead, ; 611: System.Threading => 152
	i64 u0x8da188285aadfe8e, ; 612: System.Collections.Concurrent => 8
	i64 u0x8e623fec9635e28f, ; 613: Syncfusion.Maui.Toolkit.resources.dll => 223
	i64 u0x8e8f269ad1e1ff94, ; 614: lib_Xamarin.AndroidX.Tracing.Tracing.Android.dll.so => 299
	i64 u0x8ed807bfe9858dfc, ; 615: Xamarin.AndroidX.Navigation.Common => 280
	i64 u0x8ee08b8194a30f48, ; 616: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 341
	i64 u0x8ef7601039857a44, ; 617: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 354
	i64 u0x8ef9414937d93a0a, ; 618: SQLitePCLRaw.core.dll => 219
	i64 u0x8efbc0801a122264, ; 619: Xamarin.GooglePlayServices.Tasks.dll => 319
	i64 u0x8f32c6f611f6ffab, ; 620: pt/Microsoft.Maui.Controls.resources.dll => 353
	i64 u0x8f44b45eb046bbd1, ; 621: System.ServiceModel.Web.dll => 134
	i64 u0x8f8829d21c8985a4, ; 622: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 352
	i64 u0x8fbf5b0114c6dcef, ; 623: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 624: Xamarin.KotlinX.Serialization.Core => 326
	i64 u0x8fd27d934d7b3a55, ; 625: SQLitePCLRaw.core => 219
	i64 u0x90263f8448b8f572, ; 626: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x903101b46fb73a04, ; 627: _Microsoft.Android.Resource.Designer => 369
	i64 u0x90393bd4865292f3, ; 628: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 629: System.Threading.Tasks.Extensions.dll => 146
	i64 u0x90634f86c5ebe2b5, ; 630: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 275
	i64 u0x907b636704ad79ef, ; 631: lib_Microsoft.Maui.Controls.Xaml.dll.so => 213
	i64 u0x90e9efbfd68593e0, ; 632: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 266
	i64 u0x91418dc638b29e68, ; 633: lib_Xamarin.AndroidX.CustomView.dll.so => 254
	i64 u0x9157bd523cd7ed36, ; 634: lib_System.Text.Json.dll.so => 140
	i64 u0x91a74f07b30d37e2, ; 635: System.Linq.dll => 63
	i64 u0x91cb86ea3b17111d, ; 636: System.ServiceModel.Web => 134
	i64 u0x91fa41a87223399f, ; 637: ca/Microsoft.Maui.Controls.resources.dll => 332
	i64 u0x92054e486c0c7ea7, ; 638: System.IO.FileSystem.DriveInfo => 48
	i64 u0x928614058c40c4cd, ; 639: lib_System.Xml.XPath.XDocument.dll.so => 163
	i64 u0x92b138fffca2b01e, ; 640: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 236
	i64 u0x92dd6c6033393bf7, ; 641: Syncfusion.Maui.Toolkit.resources => 223
	i64 u0x92dfc2bfc6c6a888, ; 642: Xamarin.AndroidX.Lifecycle.LiveData => 266
	i64 u0x933da2c779423d68, ; 643: Xamarin.Android.Glide.Annotations => 225
	i64 u0x9388aad9b7ae40ce, ; 644: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 264
	i64 u0x93cfa73ab28d6e35, ; 645: ms/Microsoft.Maui.Controls.resources => 348
	i64 u0x941c00d21e5c0679, ; 646: lib_Xamarin.AndroidX.Transition.dll.so => 300
	i64 u0x944077d8ca3c6580, ; 647: System.IO.Compression.dll => 46
	i64 u0x948cffedc8ed7960, ; 648: System.Xml => 167
	i64 u0x948d746a7702861f, ; 649: Microsoft.IdentityModel.Logging.dll => 209
	i64 u0x94bbeab0d4764588, ; 650: System.IO.Hashing.dll => 176
	i64 u0x94c8990839c4bdb1, ; 651: lib_Xamarin.AndroidX.Interpolator.dll.so => 263
	i64 u0x9564283c37ed59a9, ; 652: lib_Microsoft.IdentityModel.Logging.dll.so => 209
	i64 u0x95c6b36f5f5d7039, ; 653: Xamarin.AndroidX.Camera.Camera2 => 238
	i64 u0x95d757769563d0d3, ; 654: Xamarin.AndroidX.Camera.Lifecycle.dll => 240
	i64 u0x967fc325e09bfa8c, ; 655: es/Microsoft.Maui.Controls.resources => 337
	i64 u0x9686161486d34b81, ; 656: lib_Xamarin.AndroidX.ExifInterface.dll.so => 260
	i64 u0x9732d8dbddea3d9a, ; 657: id/Microsoft.Maui.Controls.resources => 344
	i64 u0x978be80e5210d31b, ; 658: Microsoft.Maui.Graphics.dll => 216
	i64 u0x979ab54025cc1c7f, ; 659: lib_Xamarin.GooglePlayServices.Base.dll.so => 315
	i64 u0x97b8c771ea3e4220, ; 660: System.ComponentModel.dll => 18
	i64 u0x97e144c9d3c6976e, ; 661: System.Collections.Concurrent.dll => 8
	i64 u0x984184e3c70d4419, ; 662: GoogleGson => 182
	i64 u0x9843944103683dd3, ; 663: Xamarin.AndroidX.Core.Core.Ktx => 251
	i64 u0x98b05cc81e6f333c, ; 664: Xamarin.AndroidX.SavedState.SavedState.Android.dll => 292
	i64 u0x98d720cc4597562c, ; 665: System.Security.Cryptography.OpenSsl => 126
	i64 u0x99052c1297204af4, ; 666: lib_Xamarin.AndroidX.Camera.Core.dll.so => 239
	i64 u0x991d510397f92d9d, ; 667: System.Linq.Expressions => 60
	i64 u0x996ceeb8a3da3d67, ; 668: System.Threading.Overlapped.dll => 144
	i64 u0x999cb19e1a04ffd3, ; 669: CommunityToolkit.Mvvm.dll => 181
	i64 u0x99a00ca5270c6878, ; 670: Xamarin.AndroidX.Navigation.Runtime => 283
	i64 u0x99cdc6d1f2d3a72f, ; 671: ko/Microsoft.Maui.Controls.resources.dll => 347
	i64 u0x9a01b1da98b6ee10, ; 672: Xamarin.AndroidX.Lifecycle.Runtime.dll => 270
	i64 u0x9a5ccc274fd6e6ee, ; 673: Jsr305Binding.dll => 311
	i64 u0x9ae6940b11c02876, ; 674: lib_Xamarin.AndroidX.Window.dll.so => 306
	i64 u0x9b211a749105beac, ; 675: System.Transactions.Local => 153
	i64 u0x9b8734714671022d, ; 676: System.Threading.Tasks.Dataflow.dll => 145
	i64 u0x9bc6aea27fbf034f, ; 677: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 324
	i64 u0x9c244ac7cda32d26, ; 678: System.Security.Cryptography.X509Certificates.dll => 128
	i64 u0x9c465f280cf43733, ; 679: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 323
	i64 u0x9c8f6872beab6408, ; 680: System.Xml.XPath.XDocument.dll => 163
	i64 u0x9ce01cf91101ae23, ; 681: System.Xml.XmlDocument => 165
	i64 u0x9d128180c81d7ce6, ; 682: Xamarin.AndroidX.CustomView.PoolingContainer => 255
	i64 u0x9d5dbcf5a48583fe, ; 683: lib_Xamarin.AndroidX.Activity.dll.so => 228
	i64 u0x9d74dee1a7725f34, ; 684: Microsoft.Extensions.Configuration.Abstractions.dll => 191
	i64 u0x9dd0e195825d65c6, ; 685: lib_Xamarin.AndroidX.Navigation.Runtime.Android.dll.so => 284
	i64 u0x9e4534b6adaf6e84, ; 686: nl/Microsoft.Maui.Controls.resources => 350
	i64 u0x9e4b95dec42769f7, ; 687: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 688: Xamarin.AndroidX.Navigation.Common.dll => 280
	i64 u0x9ef542cf1f78c506, ; 689: Xamarin.AndroidX.Lifecycle.LiveData.Core => 267
	i64 u0x9ff334e3cf272fd6, ; 690: lib_Xamarin.AndroidX.Camera.Lifecycle.dll.so => 240
	i64 u0xa00832eb975f56a8, ; 691: lib_System.Net.dll.so => 84
	i64 u0xa0ad78236b7b267f, ; 692: Xamarin.AndroidX.Window => 306
	i64 u0xa0d8259f4cc284ec, ; 693: lib_System.Security.Cryptography.dll.so => 129
	i64 u0xa0e17ca50c77a225, ; 694: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 312
	i64 u0xa0ff9b3e34d92f11, ; 695: lib_System.Resources.Writer.dll.so => 103
	i64 u0xa12fbfb4da97d9f3, ; 696: System.Threading.Timer.dll => 151
	i64 u0xa1440773ee9d341e, ; 697: Xamarin.Google.Android.Material => 309
	i64 u0xa18c39c44cdc3465, ; 698: Xamarin.AndroidX.Window.WindowCore => 307
	i64 u0xa1b9d7c27f47219f, ; 699: Xamarin.AndroidX.Navigation.UI.dll => 285
	i64 u0xa2572680829d2c7c, ; 700: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 701: System.Xml.XmlDocument.dll => 165
	i64 u0xa308401900e5bed3, ; 702: lib_mscorlib.dll.so => 170
	i64 u0xa395572e7da6c99d, ; 703: lib_System.Security.dll.so => 133
	i64 u0xa3e683f24b43af6f, ; 704: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 705: Xamarin.AndroidX.VectorDrawable.Animated => 302
	i64 u0xa46aa1eaa214539b, ; 706: ko/Microsoft.Maui.Controls.resources => 347
	i64 u0xa4a372eecb9e4df0, ; 707: Microsoft.Extensions.Diagnostics => 196
	i64 u0xa4d20d2ff0563d26, ; 708: lib_CommunityToolkit.Mvvm.dll.so => 181
	i64 u0xa4edc8f2ceae241a, ; 709: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 710: System.Runtime.Serialization.Formatters.dll => 114
	i64 u0xa54b74df83dce92b, ; 711: System.Reflection.DispatchProxy => 92
	i64 u0xa5b7152421ed6d98, ; 712: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 713: lib_System.Linq.Parallel.dll.so => 61
	i64 u0xa5ce5c755bde8cb8, ; 714: lib_System.Security.Cryptography.Csp.dll.so => 124
	i64 u0xa5e599d1e0524750, ; 715: System.Numerics.Vectors.dll => 85
	i64 u0xa5f1ba49b85dd355, ; 716: System.Security.Cryptography.dll => 129
	i64 u0xa61975a5a37873ea, ; 717: lib_System.Xml.XmlSerializer.dll.so => 166
	i64 u0xa6593e21584384d2, ; 718: lib_Jsr305Binding.dll.so => 311
	i64 u0xa66cbee0130865f7, ; 719: lib_WindowsBase.dll.so => 169
	i64 u0xa67dbee13e1df9ca, ; 720: Xamarin.AndroidX.SavedState.dll => 291
	i64 u0xa684b098dd27b296, ; 721: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 294
	i64 u0xa68a420042bb9b1f, ; 722: Xamarin.AndroidX.DrawerLayout.dll => 256
	i64 u0xa6d26156d1cacc7c, ; 723: Xamarin.Android.Glide.dll => 224
	i64 u0xa75386b5cb9595aa, ; 724: Xamarin.AndroidX.Lifecycle.Runtime.Android => 271
	i64 u0xa763fbb98df8d9fb, ; 725: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa78ce3745383236a, ; 726: Xamarin.AndroidX.Lifecycle.Common.Jvm => 265
	i64 u0xa7c31b56b4dc7b33, ; 727: hu/Microsoft.Maui.Controls.resources => 343
	i64 u0xa7eab29ed44b4e7a, ; 728: Mono.Android.Export => 173
	i64 u0xa8195217cbf017b7, ; 729: Microsoft.VisualBasic.Core => 2
	i64 u0xa843f6095f0d247d, ; 730: Xamarin.GooglePlayServices.Base.dll => 315
	i64 u0xa859a95830f367ff, ; 731: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 276
	i64 u0xa8b52f21e0dbe690, ; 732: System.Runtime.Serialization.dll => 118
	i64 u0xa8c84ce526c2b4bd, ; 733: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 368
	i64 u0xa8e6320dd07580ef, ; 734: lib_Microsoft.IdentityModel.JsonWebTokens.dll.so => 208
	i64 u0xa8ee4ed7de2efaee, ; 735: Xamarin.AndroidX.Annotation.dll => 230
	i64 u0xa95590e7c57438a4, ; 736: System.Configuration => 19
	i64 u0xa964304b5631e28a, ; 737: CommunityToolkit.Maui.Core.dll => 180
	i64 u0xaa2219c8e3449ff5, ; 738: Microsoft.Extensions.Logging.Abstractions => 202
	i64 u0xaa443ac34067eeef, ; 739: System.Private.Xml.dll => 91
	i64 u0xaa52de307ef5d1dd, ; 740: System.Net.Http => 66
	i64 u0xaa9a7b0214a5cc5c, ; 741: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 742: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 194
	i64 u0xaaf84bb3f052a265, ; 743: el/Microsoft.Maui.Controls.resources => 336
	i64 u0xab9af77b5b67a0b8, ; 744: Xamarin.AndroidX.ConstraintLayout.Core => 248
	i64 u0xab9c1b2687d86b0b, ; 745: lib_System.Linq.Expressions.dll.so => 60
	i64 u0xac2af3fa195a15ce, ; 746: System.Runtime.Numerics => 113
	i64 u0xac5376a2a538dc10, ; 747: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 267
	i64 u0xac5acae88f60357e, ; 748: System.Diagnostics.Tools.dll => 32
	i64 u0xac79c7e46047ad98, ; 749: System.Security.Principal.Windows.dll => 130
	i64 u0xac98d31068e24591, ; 750: System.Xml.XDocument => 162
	i64 u0xacd46e002c3ccb97, ; 751: ro/Microsoft.Maui.Controls.resources => 354
	i64 u0xacdd9e4180d56dda, ; 752: Xamarin.AndroidX.Concurrent.Futures => 246
	i64 u0xacf42eea7ef9cd12, ; 753: System.Threading.Channels => 143
	i64 u0xad89c07347f1bad6, ; 754: nl/Microsoft.Maui.Controls.resources.dll => 350
	i64 u0xadbb53caf78a79d2, ; 755: System.Web.HttpUtility => 156
	i64 u0xadc90ab061a9e6e4, ; 756: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 757: Xamarin.AndroidX.Collection.Ktx => 245
	i64 u0xadd8eda2edf396ad, ; 758: Xamarin.Android.Glide.GifDecoder => 227
	i64 u0xadf4cf30debbeb9a, ; 759: System.Net.ServicePoint.dll => 77
	i64 u0xadf511667bef3595, ; 760: System.Net.Security => 75
	i64 u0xae0aaa94fdcfce0f, ; 761: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 762: Java.Interop => 172
	i64 u0xae53579c90db1107, ; 763: System.ObjectModel.dll => 87
	i64 u0xaf12fb8133ac3fbb, ; 764: Microsoft.EntityFrameworkCore.Sqlite => 187
	i64 u0xaf4829c0b3e740ae, ; 765: lib_Syncfusion.Maui.Toolkit.resources.dll.so => 223
	i64 u0xaf732d0b2193b8f5, ; 766: System.Security.Cryptography.OpenSsl.dll => 126
	i64 u0xafdb94dbccd9d11c, ; 767: Xamarin.AndroidX.Lifecycle.LiveData.dll => 266
	i64 u0xafe29f45095518e7, ; 768: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 277
	i64 u0xb03ae931fb25607e, ; 769: Xamarin.AndroidX.ConstraintLayout => 247
	i64 u0xb05cc42cd94c6d9d, ; 770: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 357
	i64 u0xb0ac21bec8f428c5, ; 771: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 273
	i64 u0xb0bb43dc52ea59f9, ; 772: System.Diagnostics.Tracing.dll => 34
	i64 u0xb1dd05401aa8ee63, ; 773: System.Security.AccessControl => 120
	i64 u0xb220631954820169, ; 774: System.Text.RegularExpressions => 141
	i64 u0xb2376e1dbf8b4ed7, ; 775: System.Security.Cryptography.Csp => 124
	i64 u0xb2a1959fe95c5402, ; 776: lib_System.Runtime.InteropServices.JavaScript.dll.so => 108
	i64 u0xb2a3f67f3bf29fce, ; 777: da/Microsoft.Maui.Controls.resources => 334
	i64 u0xb3011a0a57f7ffb2, ; 778: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 366
	i64 u0xb34f09b530d826d6, ; 779: Xamarin.AndroidX.NavigationEvent.dll => 286
	i64 u0xb3874072ee0ecf8c, ; 780: Xamarin.AndroidX.VectorDrawable.Animated.dll => 302
	i64 u0xb3f0a0fcda8d3ebc, ; 781: Xamarin.AndroidX.CardView => 242
	i64 u0xb46be1aa6d4fff93, ; 782: hi/Microsoft.Maui.Controls.resources => 341
	i64 u0xb477491be13109d8, ; 783: ar/Microsoft.Maui.Controls.resources => 331
	i64 u0xb4bd7015ecee9d86, ; 784: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 785: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 786: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb54092076b15e062, ; 787: System.Threading.AccessControl => 142
	i64 u0xb5c38bf497a4cfe2, ; 788: lib_System.Threading.Tasks.dll.so => 148
	i64 u0xb5c7fcdafbc67ee4, ; 789: Microsoft.Extensions.Logging.Abstractions.dll => 202
	i64 u0xb5ea31d5244c6626, ; 790: System.Threading.ThreadPool.dll => 150
	i64 u0xb6096b3bc026be32, ; 791: MauiApp1 => 0
	i64 u0xb7212c4683a94afe, ; 792: System.Drawing.Primitives => 35
	i64 u0xb7b7753d1f319409, ; 793: sv/Microsoft.Maui.Controls.resources => 357
	i64 u0xb81a2c6e0aee50fe, ; 794: lib_System.Private.CoreLib.dll.so => 177
	i64 u0xb872c26142d22aa9, ; 795: Microsoft.Extensions.Http.dll => 200
	i64 u0xb8c60af47c08d4da, ; 796: System.Net.ServicePoint => 77
	i64 u0xb8e68d20aad91196, ; 797: lib_System.Xml.XPath.dll.so => 164
	i64 u0xb9185c33a1643eed, ; 798: Microsoft.CSharp.dll => 1
	i64 u0xb960d6b2200ba320, ; 799: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll => 278
	i64 u0xb9b19a3eb1924681, ; 800: lib_Microsoft.Maui.Controls.Maps.dll.so => 212
	i64 u0xb9b8001adf4ed7cc, ; 801: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 295
	i64 u0xb9f64d3b230def68, ; 802: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 353
	i64 u0xb9fc3c8a556e3691, ; 803: ja/Microsoft.Maui.Controls.resources => 346
	i64 u0xba4670aa94a2b3c6, ; 804: lib_System.Xml.XDocument.dll.so => 162
	i64 u0xba48785529705af9, ; 805: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 806: lib_System.Windows.dll.so => 158
	i64 u0xbb286883bc35db36, ; 807: System.Transactions.dll => 154
	i64 u0xbb65706fde942ce3, ; 808: System.Net.Sockets => 78
	i64 u0xbb6dc0b35452c1a0, ; 809: ZXing.Net.MAUI.dll => 329
	i64 u0xbba28979413cad9e, ; 810: lib_System.Runtime.CompilerServices.VisualC.dll.so => 105
	i64 u0xbba8707b914a6755, ; 811: AutoMapper => 178
	i64 u0xbbd180354b67271a, ; 812: System.Runtime.Serialization.Formatters => 114
	i64 u0xbc22a245dab70cb4, ; 813: lib_SQLitePCLRaw.provider.e_sqlite3.dll.so => 221
	i64 u0xbc260cdba33291a3, ; 814: Xamarin.AndroidX.Arch.Core.Common.dll => 235
	i64 u0xbd0e2c0d55246576, ; 815: System.Net.Http.dll => 66
	i64 u0xbd3fbd85b9e1cb29, ; 816: lib_System.Net.HttpListener.dll.so => 67
	i64 u0xbd437a2cdb333d0d, ; 817: Xamarin.AndroidX.ViewPager2 => 305
	i64 u0xbd4f572d2bd0a789, ; 818: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 819: lib_Xamarin.AndroidX.Browser.dll.so => 237
	i64 u0xbd877b14d0b56392, ; 820: System.Runtime.Intrinsics.dll => 111
	i64 u0xbe08e3083025c53d, ; 821: ZXing.Net.MAUI.Controls.dll => 330
	i64 u0xbe532a80075c3dc8, ; 822: Xamarin.AndroidX.Camera.Core.dll => 239
	i64 u0xbe65a49036345cf4, ; 823: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 824: Xamarin.AndroidX.AppCompat.AppCompatResources => 234
	i64 u0xbef9919db45b4ca7, ; 825: System.IO.Pipes.AccessControl => 55
	i64 u0xbf0fa68611139208, ; 826: lib_Xamarin.AndroidX.Annotation.dll.so => 230
	i64 u0xbfc1e1fb3095f2b3, ; 827: lib_System.Net.Http.Json.dll.so => 65
	i64 u0xc040a4ab55817f58, ; 828: ar/Microsoft.Maui.Controls.resources.dll => 331
	i64 u0xc07cadab29efeba0, ; 829: Xamarin.AndroidX.Core.Core.Ktx.dll => 251
	i64 u0xc0d928351ab5ca77, ; 830: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 831: System.Runtime.Intrinsics => 111
	i64 u0xc111030af54d7191, ; 832: System.Resources.Writer => 103
	i64 u0xc12b8b3afa48329c, ; 833: lib_System.Linq.dll.so => 63
	i64 u0xc1347413e524ff69, ; 834: lib_Syncfusion.Maui.Toolkit.dll.so => 222
	i64 u0xc183ca0b74453aa9, ; 835: lib_System.Threading.Tasks.Dataflow.dll.so => 145
	i64 u0xc1c2cb7af77b8858, ; 836: Microsoft.EntityFrameworkCore => 184
	i64 u0xc1ff9ae3cdb6e1e6, ; 837: Xamarin.AndroidX.Activity.dll => 228
	i64 u0xc26c064effb1dea9, ; 838: System.Buffers.dll => 7
	i64 u0xc278de356ad8a9e3, ; 839: Microsoft.IdentityModel.Logging => 209
	i64 u0xc28c50f32f81cc73, ; 840: ja/Microsoft.Maui.Controls.resources.dll => 346
	i64 u0xc2902f6cf5452577, ; 841: lib_Mono.Android.Export.dll.so => 173
	i64 u0xc2a3bca55b573141, ; 842: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 843: Xamarin.AndroidX.ViewPager2.dll => 305
	i64 u0xc30b52815b58ac2c, ; 844: lib_System.Runtime.Serialization.Xml.dll.so => 117
	i64 u0xc3492f8f90f96ce4, ; 845: lib_Microsoft.Extensions.DependencyModel.dll.so => 195
	i64 u0xc36d7d89c652f455, ; 846: System.Threading.Overlapped => 144
	i64 u0xc396b285e59e5493, ; 847: GoogleGson.dll => 182
	i64 u0xc3c86c1e5e12f03d, ; 848: WindowsBase => 169
	i64 u0xc3f0e03e56ce7b69, ; 849: zxing => 328
	i64 u0xc421b61fd853169d, ; 850: lib_System.Net.WebSockets.Client.dll.so => 82
	i64 u0xc463e077917aa21d, ; 851: System.Runtime.Serialization.Json => 115
	i64 u0xc472ce300460ccb6, ; 852: Microsoft.EntityFrameworkCore.dll => 184
	i64 u0xc4d3858ed4d08512, ; 853: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 277
	i64 u0xc4d69851fe06342f, ; 854: lib_Microsoft.Extensions.Caching.Memory.dll.so => 189
	i64 u0xc50fded0ded1418c, ; 855: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 856: lib_System.Net.Requests.dll.so => 74
	i64 u0xc5293b19e4dc230e, ; 857: Xamarin.AndroidX.Navigation.Fragment => 282
	i64 u0xc5325b2fcb37446f, ; 858: lib_System.Private.Xml.dll.so => 91
	i64 u0xc535cb9a21385d9b, ; 859: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 226
	i64 u0xc5a0f4b95a699af7, ; 860: lib_System.Private.Uri.dll.so => 89
	i64 u0xc5cdcd5b6277579e, ; 861: lib_System.Security.Cryptography.Algorithms.dll.so => 122
	i64 u0xc5ec286825cb0bf4, ; 862: Xamarin.AndroidX.Tracing.Tracing => 298
	i64 u0xc64f6952cef5d09f, ; 863: Microsoft.Maui.Maps.dll => 217
	i64 u0xc6706bc8aa7fe265, ; 864: Xamarin.AndroidX.Annotation.Jvm => 232
	i64 u0xc68e480c8069e1f7, ; 865: Microsoft.Maui.Maps => 217
	i64 u0xc74d70d4aa96cef3, ; 866: Xamarin.AndroidX.Navigation.Runtime.Android => 284
	i64 u0xc7c01e7d7c93a110, ; 867: System.Text.Encoding.Extensions.dll => 137
	i64 u0xc7ce851898a4548e, ; 868: lib_System.Web.HttpUtility.dll.so => 156
	i64 u0xc809d4089d2556b2, ; 869: System.Runtime.InteropServices.JavaScript.dll => 108
	i64 u0xc858a28d9ee5a6c5, ; 870: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 871: System.Reflection.DispatchProxy.dll => 92
	i64 u0xc9c62c8f354ac568, ; 872: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xc9e54b32fc19baf3, ; 873: lib_CommunityToolkit.Maui.dll.so => 179
	i64 u0xca32340d8d54dcd5, ; 874: Microsoft.Extensions.Caching.Memory.dll => 189
	i64 u0xca3a723e7342c5b6, ; 875: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 359
	i64 u0xca5801070d9fccfb, ; 876: System.Text.Encoding => 138
	i64 u0xcab3493c70141c2d, ; 877: pl/Microsoft.Maui.Controls.resources => 351
	i64 u0xcacfddc9f7c6de76, ; 878: ro/Microsoft.Maui.Controls.resources.dll => 354
	i64 u0xcadbc92899a777f0, ; 879: Xamarin.AndroidX.Startup.StartupRuntime => 296
	i64 u0xcb45618372c47127, ; 880: Microsoft.EntityFrameworkCore.Relational => 186
	i64 u0xcba1cb79f45292b5, ; 881: Xamarin.Android.Glide.GifDecoder.dll => 227
	i64 u0xcbb5f80c7293e696, ; 882: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 883: lib__Microsoft.Android.Resource.Designer.dll.so => 369
	i64 u0xcc15da1e07bbd994, ; 884: Xamarin.AndroidX.SlidingPaneLayout => 295
	i64 u0xcc2876b32ef2794c, ; 885: lib_System.Text.RegularExpressions.dll.so => 141
	i64 u0xcc5c3bb714c4561e, ; 886: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 325
	i64 u0xcc76886e09b88260, ; 887: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 327
	i64 u0xcc9fa2923aa1c9ef, ; 888: System.Diagnostics.Contracts.dll => 25
	i64 u0xccae9bb73e2326bd, ; 889: lib_System.IO.Hashing.dll.so => 176
	i64 u0xccf25c4b634ccd3a, ; 890: zh-Hans/Microsoft.Maui.Controls.resources.dll => 363
	i64 u0xcd10a42808629144, ; 891: System.Net.Requests => 74
	i64 u0xcdca1b920e9f53ba, ; 892: Xamarin.AndroidX.Interpolator => 263
	i64 u0xcdd0c48b6937b21c, ; 893: Xamarin.AndroidX.SwipeRefreshLayout => 297
	i64 u0xcde1fa22dc303670, ; 894: Microsoft.VisualStudio.DesignTools.XamlTapContract => 368
	i64 u0xcf23d8093f3ceadf, ; 895: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 896: System.Net.Mail.dll => 68
	i64 u0xcf8fc898f98b0d34, ; 897: System.Private.Xml.Linq => 90
	i64 u0xcfb21487d9cb358b, ; 898: Xamarin.GooglePlayServices.Maps.dll => 318
	i64 u0xd04b5f59ed596e31, ; 899: System.Reflection.Metadata.dll => 97
	i64 u0xd063299fcfc0c93f, ; 900: lib_System.Runtime.Serialization.Json.dll.so => 115
	i64 u0xd0de8a113e976700, ; 901: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 902: System.Runtime.Extensions => 106
	i64 u0xd1194e1d8a8de83c, ; 903: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 265
	i64 u0xd12beacdfc14f696, ; 904: System.Dynamic.Runtime => 37
	i64 u0xd16fd7fb9bbcd43e, ; 905: Microsoft.Extensions.Diagnostics.Abstractions => 197
	i64 u0xd198e7ce1b6a8344, ; 906: System.Net.Quic.dll => 73
	i64 u0xd3144156a3727ebe, ; 907: Xamarin.Google.Guava.ListenableFuture => 314
	i64 u0xd333d0af9e423810, ; 908: System.Runtime.InteropServices => 110
	i64 u0xd33a415cb4278969, ; 909: System.Security.Cryptography.Encoding.dll => 125
	i64 u0xd3426d966bb704f5, ; 910: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 234
	i64 u0xd3651b6fc3125825, ; 911: System.Private.Uri.dll => 89
	i64 u0xd373685349b1fe8b, ; 912: Microsoft.Extensions.Logging.dll => 201
	i64 u0xd3801faafafb7698, ; 913: System.Private.DataContractSerialization.dll => 88
	i64 u0xd3e4c8d6a2d5d470, ; 914: it/Microsoft.Maui.Controls.resources => 345
	i64 u0xd3edcc1f25459a50, ; 915: System.Reflection.Emit => 95
	i64 u0xd42655883bb8c19f, ; 916: Microsoft.EntityFrameworkCore.Abstractions.dll => 185
	i64 u0xd4645626dffec99d, ; 917: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 194
	i64 u0xd4fa0abb79079ea9, ; 918: System.Security.Principal.dll => 131
	i64 u0xd5507e11a2b2839f, ; 919: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 277
	i64 u0xd567f168deeeaf3c, ; 920: lib_zxing.dll.so => 328
	i64 u0xd5d04bef8478ea19, ; 921: Xamarin.AndroidX.Tracing.Tracing.dll => 298
	i64 u0xd60815f26a12e140, ; 922: Microsoft.Extensions.Logging.Debug.dll => 203
	i64 u0xd63b432ec9306914, ; 923: zxing.dll => 328
	i64 u0xd65786d27a4ad960, ; 924: lib_Microsoft.Maui.Controls.HotReload.Forms.dll.so => 365
	i64 u0xd6694f8359737e4e, ; 925: Xamarin.AndroidX.SavedState => 291
	i64 u0xd6949e129339eae5, ; 926: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 251
	i64 u0xd6d21782156bc35b, ; 927: Xamarin.AndroidX.SwipeRefreshLayout.dll => 297
	i64 u0xd6de019f6af72435, ; 928: Xamarin.AndroidX.ConstraintLayout.Core.dll => 248
	i64 u0xd6f6d366d4896aef, ; 929: lib_Xamarin.AndroidX.NavigationEvent.Android.dll.so => 287
	i64 u0xd70956d1e6deefb9, ; 930: Jsr305Binding => 311
	i64 u0xd72329819cbbbc44, ; 931: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 191
	i64 u0xd72c760af136e863, ; 932: System.Xml.XmlSerializer.dll => 166
	i64 u0xd753f071e44c2a03, ; 933: lib_System.Security.SecureString.dll.so => 132
	i64 u0xd7b3764ada9d341d, ; 934: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 202
	i64 u0xd7f0088bc5ad71f2, ; 935: Xamarin.AndroidX.VersionedParcelable => 303
	i64 u0xd824ef6ab33f8f7a, ; 936: Xamarin.AndroidX.Window.WindowCore.dll => 307
	i64 u0xd8fb25e28ae30a12, ; 937: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 288
	i64 u0xd9d04d95a2671e29, ; 938: lib_ZXing.Net.MAUI.Controls.dll.so => 330
	i64 u0xda1dfa4c534a9251, ; 939: Microsoft.Extensions.DependencyInjection => 193
	i64 u0xdad05a11827959a3, ; 940: System.Collections.NonGeneric.dll => 10
	i64 u0xdaefdfe71aa53cf9, ; 941: System.IO.FileSystem.Primitives => 49
	i64 u0xdb5383ab5865c007, ; 942: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 361
	i64 u0xdb58816721c02a59, ; 943: lib_System.Reflection.Emit.ILGeneration.dll.so => 93
	i64 u0xdbeda89f832aa805, ; 944: vi/Microsoft.Maui.Controls.resources.dll => 361
	i64 u0xdbf2a779fbc3ac31, ; 945: System.Transactions.Local.dll => 153
	i64 u0xdbf9607a441b4505, ; 946: System.Linq => 63
	i64 u0xdbfc90157a0de9b0, ; 947: lib_System.Text.Encoding.dll.so => 138
	i64 u0xdc75032002d1a212, ; 948: lib_System.Transactions.Local.dll.so => 153
	i64 u0xdca8be7403f92d4f, ; 949: lib_System.Linq.Queryable.dll.so => 62
	i64 u0xdce2c53525640bf3, ; 950: Microsoft.Extensions.Logging => 201
	i64 u0xdd2b722d78ef5f43, ; 951: System.Runtime.dll => 119
	i64 u0xdd67031857c72f96, ; 952: lib_System.Text.Encodings.Web.dll.so => 139
	i64 u0xdd70765ad6162057, ; 953: Xamarin.JSpecify => 321
	i64 u0xdd92e229ad292030, ; 954: System.Numerics.dll => 86
	i64 u0xdde30e6b77aa6f6c, ; 955: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 363
	i64 u0xde110ae80fa7c2e2, ; 956: System.Xml.XDocument.dll => 162
	i64 u0xde4726fcdf63a198, ; 957: Xamarin.AndroidX.Transition => 300
	i64 u0xde572c2b2fb32f93, ; 958: lib_System.Threading.Tasks.Extensions.dll.so => 146
	i64 u0xde8769ebda7d8647, ; 959: hr/Microsoft.Maui.Controls.resources.dll => 342
	i64 u0xdee075f3477ef6be, ; 960: Xamarin.AndroidX.ExifInterface.dll => 260
	i64 u0xdf4b773de8fb1540, ; 961: System.Net.dll => 84
	i64 u0xdf9c7682560a9629, ; 962: System.Net.ServerSentEvents => 76
	i64 u0xdfa254ebb4346068, ; 963: System.Net.Ping => 71
	i64 u0xe0142572c095a480, ; 964: Xamarin.AndroidX.AppCompat.dll => 233
	i64 u0xe020c74e3723dc6f, ; 965: Syncfusion.Maui.Toolkit.dll => 222
	i64 u0xe021eaa401792a05, ; 966: System.Text.Encoding.dll => 138
	i64 u0xe02f89350ec78051, ; 967: Xamarin.AndroidX.CoordinatorLayout.dll => 249
	i64 u0xe0496b9d65ef5474, ; 968: Xamarin.Android.Glide.DiskLruCache.dll => 226
	i64 u0xe10b760bb1462e7a, ; 969: lib_System.Security.Cryptography.Primitives.dll.so => 127
	i64 u0xe1566bbdb759c5af, ; 970: Microsoft.Maui.Controls.HotReload.Forms.dll => 365
	i64 u0xe192a588d4410686, ; 971: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 972: System.Runtime.Loader => 112
	i64 u0xe1a77eb8831f7741, ; 973: System.Security.SecureString.dll => 132
	i64 u0xe1b52f9f816c70ef, ; 974: System.Private.Xml.Linq.dll => 90
	i64 u0xe1e199c8ab02e356, ; 975: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 976: System.Net.Security.dll => 75
	i64 u0xe2252a80fe853de4, ; 977: lib_System.Security.Principal.dll.so => 131
	i64 u0xe22fa4c9c645db62, ; 978: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe24095a7afddaab3, ; 979: lib_Microsoft.Extensions.Hosting.Abstractions.dll.so => 199
	i64 u0xe2420585aeceb728, ; 980: System.Net.Requests.dll => 74
	i64 u0xe26692647e6bcb62, ; 981: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 272
	i64 u0xe29b73bc11392966, ; 982: lib-id-Microsoft.Maui.Controls.resources.dll.so => 344
	i64 u0xe2ad448dee50fbdf, ; 983: System.Xml.Serialization => 161
	i64 u0xe2d920f978f5d85c, ; 984: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 985: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe332bacb3eb4a806, ; 986: Mono.Android.Export.dll => 173
	i64 u0xe3811d68d4fe8463, ; 987: pt-BR/Microsoft.Maui.Controls.resources.dll => 352
	i64 u0xe3b7cbae5ad66c75, ; 988: lib_System.Security.Cryptography.Encoding.dll.so => 125
	i64 u0xe4292b48f3224d5b, ; 989: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 252
	i64 u0xe4507486c308efd4, ; 990: lib_Xamarin.GooglePlayServices.Location.dll.so => 317
	i64 u0xe494f7ced4ecd10a, ; 991: hu/Microsoft.Maui.Controls.resources.dll => 343
	i64 u0xe4a9b1e40d1e8917, ; 992: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 338
	i64 u0xe4dd7925d04f7f6b, ; 993: lib_Xamarin.AndroidX.NavigationEvent.dll.so => 286
	i64 u0xe4f74a0b5bf9703f, ; 994: System.Runtime.Serialization.Primitives => 116
	i64 u0xe5434e8a119ceb69, ; 995: lib_Mono.Android.dll.so => 175
	i64 u0xe55703b9ce5c038a, ; 996: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 997: Microsoft.VisualBasic => 3
	i64 u0xe62913cc36bc07ec, ; 998: System.Xml.dll => 167
	i64 u0xe7bea09c4900a191, ; 999: Xamarin.AndroidX.VectorDrawable.dll => 301
	i64 u0xe7e03cc18dcdeb49, ; 1000: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 1001: lib_System.Configuration.dll.so => 19
	i64 u0xe7ef84b36819564e, ; 1002: Xamarin.AndroidX.NavigationEvent.Android => 287
	i64 u0xe8397cf3948e7cb7, ; 1003: lib_Microsoft.Extensions.Options.ConfigurationExtensions.dll.so => 205
	i64 u0xe86b0df4ba9e5db8, ; 1004: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 271
	i64 u0xe896622fe0902957, ; 1005: System.Reflection.Emit.dll => 95
	i64 u0xe89a2a9ef110899b, ; 1006: System.Drawing.dll => 36
	i64 u0xe8c5f8c100b5934b, ; 1007: Microsoft.Win32.Registry => 5
	i64 u0xe98163eb702ae5c5, ; 1008: Xamarin.AndroidX.Arch.Core.Runtime => 236
	i64 u0xe994f23ba4c143e5, ; 1009: Xamarin.KotlinX.Coroutines.Android => 323
	i64 u0xe9b9c8c0458fd92a, ; 1010: System.Windows => 158
	i64 u0xe9d166d87a7f2bdb, ; 1011: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 296
	i64 u0xea008206567504c4, ; 1012: Syncfusion.Maui.Toolkit => 222
	i64 u0xea5a4efc2ad81d1b, ; 1013: Xamarin.Google.ErrorProne.Annotations => 313
	i64 u0xeb2313fe9d65b785, ; 1014: Xamarin.AndroidX.ConstraintLayout.dll => 247
	i64 u0xed19c616b3fcb7eb, ; 1015: Xamarin.AndroidX.VersionedParcelable.dll => 303
	i64 u0xed60c6fa891c051a, ; 1016: lib_Microsoft.VisualStudio.DesignTools.TapContract.dll.so => 367
	i64 u0xedc4817167106c23, ; 1017: System.Net.Sockets.dll => 78
	i64 u0xedc632067fb20ff3, ; 1018: System.Memory.dll => 64
	i64 u0xedc8e4ca71a02a8b, ; 1019: Xamarin.AndroidX.Navigation.Runtime.dll => 283
	i64 u0xee27c952ed6d058b, ; 1020: Microsoft.Maui.Controls.Maps => 212
	i64 u0xee81f5b3f1c4f83b, ; 1021: System.Threading.ThreadPool => 150
	i64 u0xeeb7ebb80150501b, ; 1022: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 244
	i64 u0xeefc635595ef57f0, ; 1023: System.Security.Cryptography.Cng => 123
	i64 u0xef03b1b5a04e9709, ; 1024: System.Text.Encoding.CodePages.dll => 136
	i64 u0xef5bcbe61622ee5f, ; 1025: Xamarin.AndroidX.Tracing.Tracing.Android.dll => 299
	i64 u0xef602c523fe2e87a, ; 1026: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 314
	i64 u0xef72742e1bcca27a, ; 1027: Microsoft.Maui.Essentials.dll => 215
	i64 u0xefd1e0c4e5c9b371, ; 1028: System.Resources.ResourceManager.dll => 102
	i64 u0xefe8f8d5ed3c72ea, ; 1029: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 1030: Xamarin.AndroidX.Activity => 228
	i64 u0xeff59cbde4363ec3, ; 1031: System.Threading.AccessControl.dll => 142
	i64 u0xf00c29406ea45e19, ; 1032: es/Microsoft.Maui.Controls.resources.dll => 337
	i64 u0xf09e47b6ae914f6e, ; 1033: System.Net.NameResolution => 69
	i64 u0xf0ac2b489fed2e35, ; 1034: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 1035: lib_System.Net.ServicePoint.dll.so => 77
	i64 u0xf0c16dff90fbf5d6, ; 1036: Xamarin.AndroidX.Window.WindowCore.Jvm => 308
	i64 u0xf0de2537ee19c6ca, ; 1037: lib_System.Net.WebHeaderCollection.dll.so => 80
	i64 u0xf1138779fa181c68, ; 1038: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 270
	i64 u0xf11b621fc87b983f, ; 1039: Microsoft.Maui.Controls.Xaml.dll => 213
	i64 u0xf161f4f3c3b7e62c, ; 1040: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 1041: System.ValueTuple => 155
	i64 u0xf1c4b4005493d871, ; 1042: System.Formats.Asn1.dll => 38
	i64 u0xf22514cfad2d598b, ; 1043: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll.so => 278
	i64 u0xf238bd79489d3a96, ; 1044: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 350
	i64 u0xf2feea356ba760af, ; 1045: Xamarin.AndroidX.Arch.Core.Runtime.dll => 236
	i64 u0xf300e085f8acd238, ; 1046: lib_System.ServiceProcess.dll.so => 135
	i64 u0xf33cd2d62dbe52f9, ; 1047: Xamarin.AndroidX.NavigationEvent => 286
	i64 u0xf34e52b26e7e059d, ; 1048: System.Runtime.CompilerServices.VisualC.dll => 105
	i64 u0xf37221fda4ef8830, ; 1049: lib_Xamarin.Google.Android.Material.dll.so => 309
	i64 u0xf3ad9b8fb3eefd12, ; 1050: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3ddfe05336abf29, ; 1051: System => 168
	i64 u0xf408654b2a135055, ; 1052: System.Reflection.Emit.ILGeneration.dll => 93
	i64 u0xf4103170a1de5bd0, ; 1053: System.Linq.Queryable.dll => 62
	i64 u0xf42d20c23173d77c, ; 1054: lib_System.ServiceModel.Web.dll.so => 134
	i64 u0xf4c1dd70a5496a17, ; 1055: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 1056: System.ServiceProcess.dll => 135
	i64 u0xf4eeeaa566e9b970, ; 1057: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 255
	i64 u0xf518f63ead11fcd1, ; 1058: System.Threading.Tasks => 148
	i64 u0xf5fc7602fe27b333, ; 1059: System.Net.WebHeaderCollection => 80
	i64 u0xf6077741019d7428, ; 1060: Xamarin.AndroidX.CoordinatorLayout => 249
	i64 u0xf61ade9836ad4692, ; 1061: Microsoft.IdentityModel.Tokens.dll => 210
	i64 u0xf6742cbf457c450b, ; 1062: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 271
	i64 u0xf6c0e7d55a7a4e4f, ; 1063: Microsoft.IdentityModel.JsonWebTokens => 208
	i64 u0xf6e8de2aebcbb422, ; 1064: lib_Xamarin.AndroidX.Window.WindowCore.Jvm.dll.so => 308
	i64 u0xf6f893f692f8cb43, ; 1065: Microsoft.Extensions.Options.ConfigurationExtensions.dll => 205
	i64 u0xf70c0a7bf8ccf5af, ; 1066: System.Web => 157
	i64 u0xf77b20923f07c667, ; 1067: de/Microsoft.Maui.Controls.resources.dll => 335
	i64 u0xf7e2cac4c45067b3, ; 1068: lib_System.Numerics.Vectors.dll.so => 85
	i64 u0xf7e74930e0e3d214, ; 1069: zh-HK/Microsoft.Maui.Controls.resources.dll => 362
	i64 u0xf84773b5c81e3cef, ; 1070: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 360
	i64 u0xf8aac5ea82de1348, ; 1071: System.Linq.Queryable => 62
	i64 u0xf8abd63acd77d37b, ; 1072: Xamarin.AndroidX.Camera.View => 241
	i64 u0xf8b77539b362d3ba, ; 1073: lib_System.Reflection.Primitives.dll.so => 98
	i64 u0xf8e045dc345b2ea3, ; 1074: lib_Xamarin.AndroidX.RecyclerView.dll.so => 289
	i64 u0xf915dc29808193a1, ; 1075: System.Web.HttpUtility.dll => 156
	i64 u0xf96c777a2a0686f4, ; 1076: hi/Microsoft.Maui.Controls.resources.dll => 341
	i64 u0xf9be54c8bcf8ff3b, ; 1077: System.Security.AccessControl.dll => 120
	i64 u0xf9eec5bb3a6aedc6, ; 1078: Microsoft.Extensions.Options => 204
	i64 u0xfa0e82300e67f913, ; 1079: lib_System.AppContext.dll.so => 6
	i64 u0xfa2fdb27e8a2c8e8, ; 1080: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 1081: lib_System.Net.Security.dll.so => 75
	i64 u0xfa504dfa0f097d72, ; 1082: Microsoft.Extensions.FileProviders.Abstractions.dll => 198
	i64 u0xfa5ed7226d978949, ; 1083: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 331
	i64 u0xfa645d91e9fc4cba, ; 1084: System.Threading.Thread => 149
	i64 u0xfad4d2c770e827f9, ; 1085: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfb022853d73b7fa5, ; 1086: lib_SQLitePCLRaw.batteries_v2.dll.so => 218
	i64 u0xfb06dd2338e6f7c4, ; 1087: System.Net.Ping.dll => 71
	i64 u0xfb087abe5365e3b7, ; 1088: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb846e949baff5ea, ; 1089: System.Xml.Serialization.dll => 161
	i64 u0xfbad3e4ce4b98145, ; 1090: System.Security.Cryptography.X509Certificates => 128
	i64 u0xfbf0a31c9fc34bc4, ; 1091: lib_System.Net.Http.dll.so => 66
	i64 u0xfc6b7527cc280b3f, ; 1092: lib_System.Runtime.Serialization.Formatters.dll.so => 114
	i64 u0xfc719aec26adf9d9, ; 1093: Xamarin.AndroidX.Navigation.Fragment.dll => 282
	i64 u0xfc82690c2fe2735c, ; 1094: Xamarin.AndroidX.Lifecycle.Process.dll => 269
	i64 u0xfc93fc307d279893, ; 1095: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 1096: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfd22f00870e40ae0, ; 1097: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 256
	i64 u0xfd49b3c1a76e2748, ; 1098: System.Runtime.InteropServices.RuntimeInformation => 109
	i64 u0xfd536c702f64dc47, ; 1099: System.Text.Encoding.Extensions => 137
	i64 u0xfd583f7657b6a1cb, ; 1100: Xamarin.AndroidX.Fragment => 261
	i64 u0xfd8dd91a2c26bd5d, ; 1101: Xamarin.AndroidX.Lifecycle.Runtime => 270
	i64 u0xfda36abccf05cf5c, ; 1102: System.Net.WebSockets.Client => 82
	i64 u0xfdbe4710aa9beeff, ; 1103: CommunityToolkit.Maui => 179
	i64 u0xfddbe9695626a7f5, ; 1104: Xamarin.AndroidX.Lifecycle.Common => 264
	i64 u0xfeae9952cf03b8cb, ; 1105: tr/Microsoft.Maui.Controls.resources => 359
	i64 u0xfebe1950717515f9, ; 1106: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 268
	i64 u0xff270a55858bac8d, ; 1107: System.Security.Principal => 131
	i64 u0xff9b54613e0d2cc8, ; 1108: System.Net.Http.Json => 65
	i64 u0xffdb7a971be4ec73 ; 1109: System.ValueTuple.dll => 155
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1110 x i32] [
	i32 42, i32 324, i32 297, i32 13, i32 329, i32 283, i32 180, i32 205,
	i32 107, i32 189, i32 174, i32 48, i32 233, i32 7, i32 221, i32 88,
	i32 355, i32 333, i32 361, i32 207, i32 257, i32 72, i32 319, i32 289,
	i32 183, i32 12, i32 214, i32 104, i32 183, i32 362, i32 159, i32 19,
	i32 262, i32 244, i32 164, i32 259, i32 301, i32 170, i32 355, i32 10,
	i32 203, i32 302, i32 98, i32 255, i32 256, i32 13, i32 204, i32 10,
	i32 316, i32 129, i32 97, i32 188, i32 143, i32 39, i32 356, i32 327,
	i32 304, i32 352, i32 240, i32 318, i32 175, i32 227, i32 5, i32 215,
	i32 68, i32 294, i32 132, i32 293, i32 258, i32 69, i32 245, i32 67,
	i32 57, i32 254, i32 52, i32 43, i32 127, i32 238, i32 68, i32 83,
	i32 272, i32 367, i32 161, i32 94, i32 101, i32 289, i32 144, i32 154,
	i32 237, i32 339, i32 165, i32 172, i32 340, i32 194, i32 83, i32 367,
	i32 321, i32 245, i32 4, i32 5, i32 51, i32 103, i32 195, i32 56,
	i32 122, i32 100, i32 171, i32 120, i32 324, i32 21, i32 343, i32 139,
	i32 99, i32 327, i32 79, i32 349, i32 296, i32 121, i32 241, i32 8,
	i32 168, i32 358, i32 71, i32 226, i32 273, i32 290, i32 196, i32 174,
	i32 148, i32 40, i32 294, i32 47, i32 30, i32 285, i32 347, i32 147,
	i32 204, i32 166, i32 28, i32 86, i32 298, i32 79, i32 43, i32 29,
	i32 42, i32 105, i32 119, i32 231, i32 45, i32 93, i32 358, i32 56,
	i32 151, i32 366, i32 316, i32 149, i32 184, i32 102, i32 49, i32 20,
	i32 250, i32 116, i32 224, i32 339, i32 312, i32 218, i32 238, i32 322,
	i32 206, i32 239, i32 96, i32 58, i32 344, i32 342, i32 307, i32 83,
	i32 312, i32 172, i32 26, i32 72, i32 288, i32 197, i32 260, i32 281,
	i32 365, i32 360, i32 70, i32 33, i32 338, i32 14, i32 141, i32 38,
	i32 364, i32 246, i32 351, i32 136, i32 94, i32 90, i32 212, i32 152,
	i32 308, i32 357, i32 24, i32 140, i32 57, i32 142, i32 51, i32 336,
	i32 178, i32 29, i32 160, i32 34, i32 167, i32 188, i32 200, i32 261,
	i32 207, i32 217, i32 52, i32 369, i32 306, i32 92, i32 242, i32 35,
	i32 339, i32 160, i32 9, i32 337, i32 78, i32 59, i32 55, i32 214,
	i32 333, i32 211, i32 13, i32 305, i32 190, i32 235, i32 111, i32 276,
	i32 32, i32 106, i32 86, i32 94, i32 53, i32 98, i32 320, i32 58,
	i32 9, i32 104, i32 254, i32 69, i32 317, i32 304, i32 332, i32 198,
	i32 127, i32 290, i32 118, i32 137, i32 292, i32 210, i32 128, i32 108,
	i32 322, i32 133, i32 237, i32 330, i32 314, i32 150, i32 159, i32 262,
	i32 250, i32 218, i32 257, i32 290, i32 99, i32 24, i32 295, i32 146,
	i32 199, i32 280, i32 3, i32 170, i32 234, i32 102, i32 164, i32 101,
	i32 252, i32 25, i32 95, i32 171, i32 175, i32 229, i32 3, i32 351,
	i32 299, i32 259, i32 0, i32 1, i32 116, i32 322, i32 185, i32 262,
	i32 269, i32 33, i32 6, i32 195, i32 355, i32 159, i32 353, i32 53,
	i32 87, i32 303, i32 285, i32 44, i32 268, i32 106, i32 47, i32 140,
	i32 278, i32 65, i32 318, i32 186, i32 279, i32 70, i32 82, i32 178,
	i32 60, i32 91, i32 157, i32 235, i32 135, i32 112, i32 59, i32 345,
	i32 279, i32 288, i32 174, i32 136, i32 143, i32 40, i32 332, i32 220,
	i32 192, i32 210, i32 211, i32 310, i32 61, i32 181, i32 192, i32 275,
	i32 81, i32 25, i32 36, i32 101, i32 272, i32 72, i32 22, i32 250,
	i32 216, i32 356, i32 123, i32 70, i32 109, i32 362, i32 121, i32 119,
	i32 264, i32 281, i32 265, i32 11, i32 2, i32 126, i32 117, i32 145,
	i32 41, i32 89, i32 230, i32 219, i32 177, i32 199, i32 27, i32 151,
	i32 192, i32 346, i32 193, i32 313, i32 229, i32 1, i32 231, i32 0,
	i32 44, i32 249, i32 152, i32 18, i32 88, i32 334, i32 315, i32 41,
	i32 268, i32 243, i32 273, i32 96, i32 201, i32 28, i32 41, i32 80,
	i32 329, i32 258, i32 246, i32 147, i32 110, i32 244, i32 11, i32 107,
	i32 139, i32 16, i32 124, i32 67, i32 160, i32 22, i32 220, i32 336,
	i32 326, i32 104, i32 193, i32 325, i32 64, i32 58, i32 213, i32 335,
	i32 112, i32 177, i32 284, i32 368, i32 323, i32 9, i32 309, i32 122,
	i32 100, i32 107, i32 76, i32 276, i32 179, i32 211, i32 113, i32 232,
	i32 49, i32 59, i32 20, i32 275, i32 253, i32 73, i32 248, i32 158,
	i32 39, i32 334, i32 35, i32 38, i32 340, i32 220, i32 110, i32 349,
	i32 21, i32 320, i32 274, i32 216, i32 15, i32 206, i32 81, i32 81,
	i32 253, i32 206, i32 282, i32 293, i32 155, i32 21, i32 214, i32 333,
	i32 50, i32 51, i32 359, i32 349, i32 96, i32 225, i32 197, i32 345,
	i32 16, i32 252, i32 125, i32 342, i32 163, i32 45, i32 313, i32 182,
	i32 118, i32 64, i32 187, i32 287, i32 169, i32 196, i32 190, i32 14,
	i32 291, i32 113, i32 232, i32 61, i32 310, i32 76, i32 123, i32 348,
	i32 2, i32 358, i32 261, i32 274, i32 317, i32 200, i32 321, i32 274,
	i32 6, i32 243, i32 338, i32 257, i32 183, i32 208, i32 17, i32 356,
	i32 335, i32 79, i32 247, i32 281, i32 310, i32 316, i32 180, i32 133,
	i32 320, i32 348, i32 85, i32 203, i32 12, i32 34, i32 121, i32 326,
	i32 269, i32 176, i32 259, i32 87, i32 224, i32 319, i32 18, i32 304,
	i32 191, i32 267, i32 73, i32 366, i32 97, i32 221, i32 168, i32 263,
	i32 84, i32 364, i32 233, i32 242, i32 157, i32 36, i32 154, i32 360,
	i32 207, i32 363, i32 241, i32 147, i32 56, i32 115, i32 186, i32 243,
	i32 301, i32 300, i32 37, i32 364, i32 190, i32 117, i32 231, i32 14,
	i32 225, i32 185, i32 149, i32 43, i32 215, i32 229, i32 100, i32 325,
	i32 171, i32 16, i32 292, i32 48, i32 109, i32 99, i32 188, i32 279,
	i32 27, i32 130, i32 29, i32 340, i32 198, i32 293, i32 130, i32 44,
	i32 253, i32 187, i32 258, i32 152, i32 8, i32 223, i32 299, i32 280,
	i32 341, i32 354, i32 219, i32 319, i32 353, i32 134, i32 352, i32 42,
	i32 326, i32 219, i32 33, i32 369, i32 46, i32 146, i32 275, i32 213,
	i32 266, i32 254, i32 140, i32 63, i32 134, i32 332, i32 48, i32 163,
	i32 236, i32 223, i32 266, i32 225, i32 264, i32 348, i32 300, i32 46,
	i32 167, i32 209, i32 176, i32 263, i32 209, i32 238, i32 240, i32 337,
	i32 260, i32 344, i32 216, i32 315, i32 18, i32 8, i32 182, i32 251,
	i32 292, i32 126, i32 239, i32 60, i32 144, i32 181, i32 283, i32 347,
	i32 270, i32 311, i32 306, i32 153, i32 145, i32 324, i32 128, i32 323,
	i32 163, i32 165, i32 255, i32 228, i32 191, i32 284, i32 350, i32 26,
	i32 280, i32 267, i32 240, i32 84, i32 306, i32 129, i32 312, i32 103,
	i32 151, i32 309, i32 307, i32 285, i32 54, i32 165, i32 170, i32 133,
	i32 37, i32 302, i32 347, i32 196, i32 181, i32 22, i32 114, i32 92,
	i32 50, i32 61, i32 124, i32 85, i32 129, i32 166, i32 311, i32 169,
	i32 291, i32 294, i32 256, i32 224, i32 271, i32 4, i32 265, i32 343,
	i32 173, i32 2, i32 315, i32 276, i32 118, i32 368, i32 208, i32 230,
	i32 19, i32 180, i32 202, i32 91, i32 66, i32 30, i32 194, i32 336,
	i32 248, i32 60, i32 113, i32 267, i32 32, i32 130, i32 162, i32 354,
	i32 246, i32 143, i32 350, i32 156, i32 17, i32 245, i32 227, i32 77,
	i32 75, i32 15, i32 172, i32 87, i32 187, i32 223, i32 126, i32 266,
	i32 277, i32 247, i32 357, i32 273, i32 34, i32 120, i32 141, i32 124,
	i32 108, i32 334, i32 366, i32 286, i32 302, i32 242, i32 341, i32 331,
	i32 54, i32 47, i32 28, i32 142, i32 148, i32 202, i32 150, i32 0,
	i32 35, i32 357, i32 177, i32 200, i32 77, i32 164, i32 1, i32 278,
	i32 212, i32 295, i32 353, i32 346, i32 162, i32 12, i32 158, i32 154,
	i32 78, i32 329, i32 105, i32 178, i32 114, i32 221, i32 235, i32 66,
	i32 67, i32 305, i32 45, i32 237, i32 111, i32 330, i32 239, i32 7,
	i32 234, i32 55, i32 230, i32 65, i32 331, i32 251, i32 20, i32 111,
	i32 103, i32 63, i32 222, i32 145, i32 184, i32 228, i32 7, i32 209,
	i32 346, i32 173, i32 50, i32 305, i32 117, i32 195, i32 144, i32 182,
	i32 169, i32 328, i32 82, i32 115, i32 184, i32 277, i32 189, i32 17,
	i32 74, i32 282, i32 91, i32 226, i32 89, i32 122, i32 298, i32 217,
	i32 232, i32 217, i32 284, i32 137, i32 156, i32 108, i32 11, i32 92,
	i32 31, i32 179, i32 189, i32 359, i32 138, i32 351, i32 354, i32 296,
	i32 186, i32 227, i32 40, i32 369, i32 295, i32 141, i32 325, i32 327,
	i32 25, i32 176, i32 363, i32 74, i32 263, i32 297, i32 368, i32 27,
	i32 68, i32 90, i32 318, i32 97, i32 115, i32 31, i32 106, i32 265,
	i32 37, i32 197, i32 73, i32 314, i32 110, i32 125, i32 234, i32 89,
	i32 201, i32 88, i32 345, i32 95, i32 185, i32 194, i32 131, i32 277,
	i32 328, i32 298, i32 203, i32 328, i32 365, i32 291, i32 251, i32 297,
	i32 248, i32 287, i32 311, i32 191, i32 166, i32 132, i32 202, i32 303,
	i32 307, i32 288, i32 330, i32 193, i32 10, i32 49, i32 361, i32 93,
	i32 361, i32 153, i32 63, i32 138, i32 153, i32 62, i32 201, i32 119,
	i32 139, i32 321, i32 86, i32 363, i32 162, i32 300, i32 146, i32 342,
	i32 260, i32 84, i32 76, i32 71, i32 233, i32 222, i32 138, i32 249,
	i32 226, i32 127, i32 365, i32 54, i32 112, i32 132, i32 90, i32 23,
	i32 75, i32 131, i32 31, i32 199, i32 74, i32 272, i32 344, i32 161,
	i32 23, i32 4, i32 173, i32 352, i32 125, i32 252, i32 317, i32 343,
	i32 338, i32 286, i32 116, i32 175, i32 32, i32 3, i32 167, i32 301,
	i32 30, i32 19, i32 287, i32 205, i32 271, i32 95, i32 36, i32 5,
	i32 236, i32 323, i32 158, i32 296, i32 222, i32 313, i32 247, i32 303,
	i32 367, i32 78, i32 64, i32 283, i32 212, i32 150, i32 244, i32 123,
	i32 136, i32 299, i32 314, i32 215, i32 102, i32 39, i32 228, i32 142,
	i32 337, i32 69, i32 26, i32 77, i32 308, i32 80, i32 270, i32 213,
	i32 24, i32 155, i32 38, i32 278, i32 350, i32 236, i32 135, i32 286,
	i32 105, i32 309, i32 57, i32 168, i32 93, i32 62, i32 134, i32 46,
	i32 135, i32 255, i32 148, i32 80, i32 249, i32 210, i32 271, i32 208,
	i32 308, i32 205, i32 157, i32 335, i32 85, i32 362, i32 360, i32 62,
	i32 241, i32 98, i32 289, i32 156, i32 341, i32 120, i32 204, i32 6,
	i32 15, i32 75, i32 198, i32 331, i32 149, i32 52, i32 218, i32 71,
	i32 23, i32 161, i32 128, i32 66, i32 114, i32 282, i32 269, i32 55,
	i32 53, i32 256, i32 109, i32 137, i32 261, i32 270, i32 82, i32 179,
	i32 264, i32 359, i32 268, i32 131, i32 65, i32 155
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.mm.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.mm.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/10.0.1xx @ 350a375fc202f0072ac4191624986d8c642b93fa"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
