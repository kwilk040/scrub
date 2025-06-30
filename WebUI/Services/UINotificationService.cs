using Microsoft.JSInterop;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class UiNotificationService(
    IJSRuntime jsRuntime)
    : IUiNotificationService
{
    public async Task PushSuccessAsync(string message) => await this.PushMessageAsync(new NotificationMessage()
    {
        Message = message,
        Type = Success,
    });

    public async Task PushErrorAsync(string message) => await this.PushMessageAsync(new NotificationMessage()
    {
        Message = message,
        Type = Error,
    });

    private const string Success = "success";
    private const string Error = "error";

    private IJSObjectReference? _module;

    private async Task PushMessageAsync(NotificationMessage message)
    {
        _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/notifier.js");

        await _module.InvokeVoidAsync("pushMessage", message);
    }

    public record NotificationMessage
    {
        public required string Message { get; init; }
        public required string Type { get; init; }
    }
}