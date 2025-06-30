using Microsoft.JSInterop;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class UiHelperService(IJSRuntime jsRuntime, IUiNotificationService uiNotificationService) : IUiHelperService
{
    public async Task CopyToClipboardAsync(string text)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            await uiNotificationService.PushSuccessAsync("Copied to clipboard");
        }
        catch (Exception)
        {
            await uiNotificationService.PushErrorAsync("Failed to copy content.");
        }
    }
}