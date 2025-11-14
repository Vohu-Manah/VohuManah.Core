using Application.Abstractions.Messaging;
using Application.Library.Books.GetCountByPublishPlace;
using Application.Library._Shared;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole("Library.Books.GetCountByPublishPlace")]
internal sealed class GetCountByPublishPlace : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/count-by-publish-place", async (
            IQueryHandler<GetBookCountByPublishPlaceQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookCountByPublishPlaceQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

