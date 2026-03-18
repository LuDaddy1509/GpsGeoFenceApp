// PSEUDOCODE / PLAN (detailed):
// - Problem: MauiProgram.cs registers `SettingsPage` but the type is missing, causing CS0246.
// - Solution: Add a minimal `SettingsPage` class in the `GpsGeoFence.Pages` namespace so the DI registration compiles.
// - Requirements:
//   - The page should derive from `ContentPage` (MAUI).
//   - Accept `SettingsViewModel` via constructor (it is registered in DI as a singleton).
//   - Set `BindingContext` to the injected view model.
//   - Provide a minimal UI in C# (no XAML) so the project builds without additional files.
//   - Keep the class public so it is discoverable by the DI registration in `MauiProgram`.
// - Implementation steps:
//   1. Create `Pages/SettingsPage.cs` with appropriate using directives.
//   2. Define `public class SettingsPage : ContentPage`.
//   3. Add constructor `SettingsPage(SettingsViewModel viewModel)`.
//   4. Set `BindingContext = viewModel` and build simple content (label/placeholder).
//   5. Ensure no other project changes are required to compile this fix.

using System;
using Microsoft.Maui.Controls;
using GpsGeoFence.ViewModels;

namespace GpsGeoFence.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage(SettingsViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            BindingContext = _viewModel;
            Title = "Settings";

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 16,
                    Spacing = 12,
                    Children =
                    {
                        new Label
                        {
                            Text = "Settings",
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        // Placeholder area: bind real controls to _viewModel properties as needed.
                        new Label
                        {
                            Text = "Configure your app settings here.",
                            FontSize = 14
                        }
                    }
                }
            };
        }
    }
}