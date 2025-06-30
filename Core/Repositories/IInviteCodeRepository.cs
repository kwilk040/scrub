using Core.Model;

namespace Core.Repositories;

public interface IInviteCodeRepository
{
    public Task AddAsync(InviteCode inviteCode);
}