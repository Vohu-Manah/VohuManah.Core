using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(PermissionNames.Gaps.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/{id:int}", async (
            int id,
            IQueryHandler<GetGapByIdQuery, GapResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapByIdQuery(id);

            Result<GapResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

