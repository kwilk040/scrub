using Application.Services.Interfaces;

namespace Application.Services;

public class UserSessionService : IUserSessionService
{
    private UserSessionData? userSessionData;

    public UserSessionData GetUserSessionData() => userSessionData!;

    public void SetUserSessionData(UserSessionData getUserSessionData) => userSessionData = getUserSessionData;
}