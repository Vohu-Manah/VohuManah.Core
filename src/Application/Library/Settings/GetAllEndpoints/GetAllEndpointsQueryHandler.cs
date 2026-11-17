using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetAllEndpoints;

internal sealed class GetAllEndpointsQueryHandler : IQueryHandler<GetAllEndpointsQuery, Dictionary<string, List<string>>>
{
    public Task<Result<Dictionary<string, List<string>>>> Handle(GetAllEndpointsQuery query, CancellationToken cancellationToken)
    {
        var result = PermissionCatalog.GetAllEndpointsGrouped();
        return Task.FromResult(Result.Success(result));
    }
}


