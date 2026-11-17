using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.GetNames;
using VohuManah.Application.Library.Shared;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(PermissionNames.Gaps.GetNames)]
internal sealed class GetNames : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/names", async (
            bool? addAll,
            IQueryHandler<GetGapNamesQuery, List<SelectItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapNamesQuery(addAll ?? false);
            Result<List<SelectItemResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}


