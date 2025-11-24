using System;
using System.Collections.Generic;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.UpdateRolePermissions;

internal sealed class UpdateRolePermissionsCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateRolePermissionsCommand>
{
    public async Task<Result> Handle(UpdateRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        DbSet<Role> roles = unitOfWork.Set<Role>();
        DbSet<RolePermission> rolePermissions = unitOfWork.Set<RolePermission>();

        Role? role = await roles.FirstOrDefaultAsync(r => r.Id == command.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(Error.NotFound("Role.NotFound", "نقش یافت نشد"));
        }

        var normalizedEndpoints = (command.EndpointNames ?? Array.Empty<string>())
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint))
            .Select(endpoint => endpoint.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        List<RolePermission> existingPermissions = await rolePermissions
            .Where(rp => rp.RoleId == command.RoleId)
            .ToListAsync(cancellationToken);

        var endpointSet = new HashSet<string>(normalizedEndpoints, StringComparer.OrdinalIgnoreCase);

        // حذف مجوزهایی که دیگر وجود ندارند
        List<RolePermission> toRemove = existingPermissions
            .Where(rp => !endpointSet.Contains(rp.EndpointName))
            .ToList();

        if (toRemove.Count > 0)
        {
            rolePermissions.RemoveRange(toRemove);
        }

        // اضافه کردن مجوزهای جدید
        var existingEndpointSet = existingPermissions
            .Select(rp => rp.EndpointName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string endpoint in endpointSet)
        {
            if (existingEndpointSet.Contains(endpoint))
            {
                continue;
            }

            rolePermissions.Add(new RolePermission
            {
                RoleId = command.RoleId,
                EndpointName = endpoint
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

