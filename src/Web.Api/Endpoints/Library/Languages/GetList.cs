using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(PermissionNames.Languages.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages/list", async (
            IQueryHandler<GetLanguageListQuery, List<LanguageListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLanguageListQuery();

            Result<List<LanguageListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

