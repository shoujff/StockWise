using CommunityToolkit.Mvvm.ComponentModel;

namespace StockWise.App.Models;

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

public partial class ToastNotification : ObservableObject
{
    [ObservableProperty]
    private ToastType _type;

    [ObservableProperty]
    private string _message = "";

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private string _icon = "";

    public ToastNotification(ToastType type, string message)
    {
        Type = type;
        Message = message;
        Icon = type switch
        {
            ToastType.Success => "✅",
            ToastType.Error => "❌",
            ToastType.Warning => "⚠️",
            ToastType.Info => "ℹ️",
            _ => ""
        };
    }
}
