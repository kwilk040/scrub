namespace Application.Services.Interfaces;

public interface IUserSessionService
{
    UserSessionData GetUserSessionData();

    void SetUserSessionData(UserSessionData userSessionData);
}