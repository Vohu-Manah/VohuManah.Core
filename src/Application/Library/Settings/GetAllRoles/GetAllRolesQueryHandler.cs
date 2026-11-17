using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetAllRoles;

internal sealed class GetAllRolesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllRolesQuery, List<RoleResponse>>
{
    internal const string RolesAllKey = "sdp.roles.all";

    public Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Role> roles = unitOfWork.GetDbSet<Role>();

        List<Role> allRoles = cacheManager.Retrieve(RolesAllKey, () => roles.ToList());

        List<RoleResponse> response = allRoles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

