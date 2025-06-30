using Microsoft.AspNetCore.Components;

namespace WebUI.Components;

public class DynamicStateComponentBase : ComponentBase
{
    protected bool IsLoading { get; private set; }

    protected async Task<T> ExecuteLoadingActionAsync<T>(Func<Task<T>> func)
    {
        IsLoading = true;
        try
        {
            return await func.Invoke();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected async Task ExecuteLoadingActionAsync(Func<Task> func)
    {
        IsLoading = true;
        try
        {
            await func.Invoke();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}