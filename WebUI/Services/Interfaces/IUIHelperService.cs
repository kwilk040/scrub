namespace WebUI.Services.Interfaces;

public interface IUiHelperService
{
    Task CopyToClipboardAsync(string text);
}