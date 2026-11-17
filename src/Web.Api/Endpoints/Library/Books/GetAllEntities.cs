using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetAllEntities;
using VohuManah.Application.Library.Settings;
using VohuManah.Domain.Books;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetAllEntities)]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/entities", async (
            IQueryHandler<GetAllBookEntitiesQuery, List<Book>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllBookEntitiesQuery();

            Result<List<Book>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

