using CommunityToolkit.Mvvm.ComponentModel;
using TelemetryMobile.Localization;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public abstract partial class RemotePageModel : ObservableObject
{
    private CancellationTokenSource? loading;
    private Task currentLoad = Task.CompletedTask;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ShowEmpty))] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasError))] [NotifyPropertyChangedFor(nameof(ShowEmpty))] private string errorMessage = string.Empty;
    public bool HasError => ErrorMessage.Length > 0;
    public bool ShowEmpty => !IsBusy && !HasError;

    public void CancelLoading() => loading?.Cancel();

    protected Task RunAsync(Func<CancellationToken, Task> action)
    {
        if (IsBusy)
        {
            if (loading?.IsCancellationRequested == true) return ResumeAsync(action, currentLoad);
            IsRefreshing = false;
            return Task.CompletedTask;
        }
        currentLoad = ExecuteAsync(action);
        return currentLoad;
    }

    private async Task ResumeAsync(Func<CancellationToken, Task> action, Task previous)
    {
        await previous;
        await RunAsync(action);
    }

    private async Task ExecuteAsync(Func<CancellationToken, Task> action)
    {
        using var cancellation = new CancellationTokenSource();
        loading = cancellation;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try { await action(cancellation.Token); }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
        catch (Exception error) when (error is HttpRequestException or OperationCanceledException or System.Text.Json.JsonException or TelemetryApiException)
        {
            System.Diagnostics.Debug.WriteLine(error);
            ErrorMessage = DescribeError(error);
        }
        finally
        {
            loading = null;
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    public static string DescribeError(Exception error)
    {
        var loc = LocalizationManager.Instance;
        if (error is TelemetryApiException api)
        {
            if (api.RetryAfter is { } delay)
                return string.Format(loc["RetryAfter"], Math.Max(1, (int)Math.Ceiling(delay.TotalSeconds)));
            return loc[api.Code switch
            {
                "customer.not_found" => "CustomerNotFound",
                "device.not_found" => "DeviceNotFound",
                _ => "ServerError"
            }];
        }
        return loc[error is System.Text.Json.JsonException ? "InvalidResponse" : "ConnectionError"];
    }
}
