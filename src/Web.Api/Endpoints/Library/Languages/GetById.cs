using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(PermissionNames.Languages.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages/{id:int}", async (
            int id,
            IQueryHandler<GetLanguageByIdQuery, LanguageResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLanguageByIdQuery(id);

            Result<LanguageResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

