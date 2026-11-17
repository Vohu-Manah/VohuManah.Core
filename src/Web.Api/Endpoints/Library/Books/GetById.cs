using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetById;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/{id:long}", async (
            long id,
            IQueryHandler<GetBookByIdQuery, BookResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookByIdQuery(id);

            Result<BookResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

