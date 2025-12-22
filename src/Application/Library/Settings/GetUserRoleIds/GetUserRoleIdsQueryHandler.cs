using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetUserRoleIds;

internal sealed class GetUserRoleIdsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetUserRoleIdsQuery, List<int>>
{
    public async Task<Result<List<int>>> Handle(GetUserRoleIdsQuery query, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.Set<UserRole>();
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();

        Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<int>>(Domain.Library.UserErrors.NotFound(query.UserId));
        }

        List<int> roleIds = await userRoles
            .Where(ur => ur.UserId == query.UserId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        return Result.Success(roleIds);
    }
}

