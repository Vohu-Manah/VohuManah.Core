using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetRolePermissions;

internal sealed class GetRolePermissionsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetRolePermissionsQuery, List<string>>
{
    public async Task<Result<List<string>>> Handle(GetRolePermissionsQuery query, CancellationToken cancellationToken)
    {
        DbSet<RolePermission> rolePermissions = unitOfWork.Set<RolePermission>();
        DbSet<Role> roles = unitOfWork.Set<Role>();

        Role? role = await roles.FirstOrDefaultAsync(r => r.Id == query.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure<List<string>>(Error.NotFound("Role.NotFound", "نقش یافت نشد"));
        }

        List<string> endpointNames = await rolePermissions
            .Where(rp => rp.RoleId == query.RoleId)
            .Select(rp => rp.EndpointName)
            .OrderBy(ep => ep)
            .ToListAsync(cancellationToken);

        return Result.Success(endpointNames);
    }
}

