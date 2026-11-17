using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetProfile;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetProfile)]
internal sealed class GetProfile : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/{id:long}/profile", async (
            long id,
            IQueryHandler<GetBookProfileQuery, BookProfileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookProfileQuery(id);

            Result<BookProfileResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

