using Application.Abstractions.Messaging;
using Application.Library.Settings;
using SharedKernel;

namespace Application.Library.Settings.GetAllEndpoints;

internal sealed class GetAllEndpointsQueryHandler : IQueryHandler<GetAllEndpointsQuery, Dictionary<string, List<string>>>
{
    public Task<Result<Dictionary<string, List<string>>>> Handle(GetAllEndpointsQuery query, CancellationToken cancellationToken)
    {
        var result = EndpointCatalog.GetAllEndpointsGrouped();
        return Task.FromResult(Result.Success(result));
    }
}


