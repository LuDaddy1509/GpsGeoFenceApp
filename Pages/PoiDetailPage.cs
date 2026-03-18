// PSEUDOCODE / PLAN:
// - Provide a minimal concrete implementation of `PoiDetailPage` in namespace `GpsGeoFence.Pages`
// - Make the class public so the DI registrations in `MauiProgram` compile.
// - Add a constructor that accepts `PoiDetailViewModel` so the container can inject the view model.
// - Set `BindingContext` to the injected view model.
// - Build a simple code-based UI (no XAML, so no InitializeComponent dependency).
// - Bind UI elements to common view-model property names (e.g. "Title", "Description").
// - Keep implementation minimal and safe: does not assume other project files exist.
//
// This file fixes CS0246 (missing 'PoiDetailPage') by providing the missing type.

using Microsoft.Maui.Controls;
using GpsGeoFence.ViewModels;

namespace GpsGeoFence.Pages
{
    public partial class PoiDetailPage : ContentPage
    {
        public PoiDetailPage(PoiDetailViewModel viewModel)
        {
            BindingContext = viewModel;
            Title = "POI Details";

            var titleLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 20
            };
            titleLabel.SetBinding(Label.TextProperty, "Title");

            var descriptionLabel = new Label
            {
                LineBreakMode = LineBreakMode.WordWrap
            };
            descriptionLabel.SetBinding(Label.TextProperty, "Description");

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(16),
                    Spacing = 12,
                    Children =
                    {
                        titleLabel,
                        descriptionLabel
                    }
                }
            };
        }
    }
}