using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.GetNames;
using VohuManah.Application.Library.Shared;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(PermissionNames.Languages.GetNames)]
internal sealed class GetNames : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages/names", async (
            bool? addAll,
            IQueryHandler<GetLanguageNamesQuery, List<SelectItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLanguageNamesQuery(addAll ?? false);
            Result<List<SelectItemResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}


