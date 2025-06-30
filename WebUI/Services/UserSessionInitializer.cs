using System.Security.Claims;
using Application;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class UserSessionInitializer(
    IUserSessionService userSessionService,
    AuthenticationStateProvider authenticationStateProvider,
    IJSRuntime jsRuntime)
    : IUserSessionInitializer
{
    public async Task InitializeUserSession()
    { 
         var auth = await authenticationStateProvider.GetAuthenticationStateAsync();
         var user = auth.User;

         if (user.Identity?.IsAuthenticated == false)
         {
             // TODO: redirect to error page with message 
             return;
         }

         var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;

         if (string.IsNullOrEmpty(userIdClaim))
         {
             Console.WriteLine("Cannot restore user ID.");
             return;
         }

         var userId = Guid.Parse(userIdClaim);
         var timeZone = TimeZoneInfo.FindSystemTimeZoneById(await jsRuntime.InvokeAsync<string>("getUserTimeZone"));

         userSessionService.SetUserSessionData(new UserSessionData
         {
             UserId = userId,
             TimeZoneInfo = timeZone,
         });
    }
}