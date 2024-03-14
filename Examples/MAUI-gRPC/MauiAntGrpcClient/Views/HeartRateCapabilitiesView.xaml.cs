using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views;

public partial class HeartRateCapabilitiesView : ContentView
{
    public static readonly BindableProperty HeartRateViewModelProperty =
    BindableProperty.Create(nameof(HeartRateViewModel), typeof(HeartRateViewModel), typeof(HeartRateCapabilitiesView));
    public HeartRateViewModel HeartRateViewModel
    {
        get => (HeartRateViewModel)GetValue(HeartRateViewModelProperty);
        set => SetValue(HeartRateViewModelProperty, value);
    }

    public HeartRateCapabilitiesView()
    {
        InitializeComponent();
    }
}