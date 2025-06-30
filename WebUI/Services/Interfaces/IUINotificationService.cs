namespace WebUI.Services.Interfaces;

public interface IUiNotificationService
{
    Task PushSuccessAsync(string message);

    Task PushErrorAsync(string message);
}