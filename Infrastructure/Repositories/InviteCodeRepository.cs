using Core.Model;
using Core.Repositories;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class InviteCodeRepository(ApplicationDbContext dbContext) : IInviteCodeRepository
{
    public async Task AddAsync(InviteCode inviteCode)
    {
        await dbContext.InviteCodes.AddAsync(inviteCode);
        await dbContext.SaveChangesAsync();
    }
}