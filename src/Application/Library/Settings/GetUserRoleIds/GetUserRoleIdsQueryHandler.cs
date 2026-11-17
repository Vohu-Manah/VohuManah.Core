using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetUserRoleIds;

internal sealed class GetUserRoleIdsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetUserRoleIdsQuery, List<int>>
{
    public async Task<Result<List<int>>> Handle(GetUserRoleIdsQuery query, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.GetDbSet<UserRole>();
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        VohuManah.Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<int>>(VohuManah.Domain.Library.UserErrors.NotFound(query.UserId));
        }

        List<int> roleIds = await userRoles
            .Where(ur => ur.UserId == query.UserId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        return Result.Success(roleIds);
    }
}

