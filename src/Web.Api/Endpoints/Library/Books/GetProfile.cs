using Application.Abstractions.Messaging;
using Application.Library.Books.GetProfile;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

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

