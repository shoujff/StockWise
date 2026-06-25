using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StockWise.App.ViewModels;

public partial class DialogViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _message = "";

    [ObservableProperty]
    private string _confirmText = "OK";

    [ObservableProperty]
    private string _cancelText = "Отмена";

    [ObservableProperty]
    private bool _showCancel = true;

    private Action<bool>? _callback;

    public void Show(string title, string message,
        string confirmText = "OK", string cancelText = "Отмена",
        bool showCancel = true, Action<bool>? callback = null)
    {
        Title = title;
        Message = message;
        ConfirmText = confirmText;
        CancelText = cancelText;
        ShowCancel = showCancel;
        _callback = callback;
        IsVisible = true;
    }

    [RelayCommand]
    private void Confirm()
    {
        IsVisible = false;
        _callback?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        IsVisible = false;
        _callback?.Invoke(false);
    }
}
