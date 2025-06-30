namespace WebUI.Components.Features.UserAside;

public class UserAsideService
{
    public event Func<Task>? OnOpenRequested;
    public event Func<Task>? OnCloseRequested;

    public async Task RequestOpenAsync()
    {
        if (OnOpenRequested is not null)
            await OnOpenRequested.Invoke();
    }

    public async Task RequestCloseAsync()
    {
        if (OnCloseRequested is not null)
            await OnCloseRequested.Invoke();
    }
}