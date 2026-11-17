using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(EndpointNames.Languages.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages", async (
            IQueryHandler<GetAllLanguagesQuery, List<LanguageResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllLanguagesQuery();

            Result<List<LanguageResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

