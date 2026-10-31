using Application.Abstractions.Messaging;
using Application.Library.Books.GetBarcodeData;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;






[RequireRole(EndpointNames.Books.GetAll)]
internal sealed class GetBarcodeData : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/barcodes", async (
            [AsParameters] BarcodeBatchRequest request,
            IQueryHandler<GetBookBarcodeQuery, List<BookBarcodeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookBarcodeQuery
            {
                AfterId = request.AfterId,
                Take = request.Take,
                Year = request.Year,
                Search = request.Search,
            };

            Result<List<BookBarcodeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

internal sealed record BarcodeBatchRequest
{
    
    public long AfterId { get; init; }

    public int Take { get; init; } = 500;
    public string? Year { get; init; }
    public string? Search { get; init; }
}
