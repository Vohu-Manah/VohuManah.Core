using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetAllRoles;

internal sealed class GetAllRolesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllRolesQuery, List<RoleResponse>>
{
    private const string RolesAllKey = "sdp.roles.all";

    public Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Role> roles = unitOfWork.Set<Role>();
        
        List<Role> allRoles = cacheManager.Get(RolesAllKey, () => roles.ToList());
        
        List<RoleResponse> response = allRoles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

