using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using StockWise.App.Models;

namespace StockWise.App.Services;

public class ToastService
{
    public ObservableCollection<ToastNotification> Toasts { get; } = new();

    public void Show(ToastType type, string message, int durationMs = 3000)
    {
        var toast = new ToastNotification(type, message);
        Dispatcher.UIThread.Post(() => Toasts.Add(toast));

        _ = RemoveAfterDelay(toast, durationMs);
    }

    private async Task RemoveAfterDelay(ToastNotification toast, int delayMs)
    {
        await Task.Delay(delayMs);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            toast.IsVisible = false;
            Toasts.Remove(toast);
        });
    }

    public void Success(string message, int durationMs = 3000) => Show(ToastType.Success, message, durationMs);
    public void Error(string message, int durationMs = 4000) => Show(ToastType.Error, message, durationMs);
    public void Warning(string message, int durationMs = 3500) => Show(ToastType.Warning, message, durationMs);
    public void Info(string message, int durationMs = 3000) => Show(ToastType.Info, message, durationMs);
}
