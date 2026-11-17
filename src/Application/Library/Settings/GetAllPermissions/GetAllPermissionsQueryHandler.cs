using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetAllPermissions;

internal sealed class GetAllPermissionsQueryHandler : IQueryHandler<GetAllPermissionsQuery, Dictionary<string, List<string>>>
{
    public Task<Result<Dictionary<string, List<string>>>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken)
    {
        var result = PermissionCatalog.GetAllEndpointsGrouped();
        return Task.FromResult(Result.Success(result));
    }
}


