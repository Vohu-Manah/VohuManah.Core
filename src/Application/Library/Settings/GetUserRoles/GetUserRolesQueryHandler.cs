using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetUserRoles;

internal sealed class GetUserRolesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetUserRolesQuery, List<UserRoleResponse>>
{
    public async Task<Result<List<UserRoleResponse>>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.GetDbSet<UserRole>();
        DbSet<RoleEndpoint> rolePermissions = unitOfWork.GetDbSet<RoleEndpoint>();
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        VohuManah.Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<UserRoleResponse>>(VohuManah.Domain.Library.UserErrors.NotFound(query.UserId));
        }

        var endpointPermissions = await userRoles
            .Where(ur => ur.UserId == query.UserId)
            .Join(
                rolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.EndpointName)
            .Distinct()
            .OrderBy(ep => ep)
            .Select(ep => new UserRoleResponse
            {
                EndpointName = ep
            })
            .ToListAsync(cancellationToken);

        return Result.Success(endpointPermissions);
    }
}

