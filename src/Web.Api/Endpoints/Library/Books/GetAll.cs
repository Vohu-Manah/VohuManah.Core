using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetAll;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(PermissionNames.Books.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books", async (
            IQueryHandler<GetAllBooksQuery, List<BookResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllBooksQuery();

            Result<List<BookResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}
