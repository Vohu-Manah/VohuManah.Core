using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

using VohuManah.Application.Library.Settings;

[RequireRole(EndpointNames.Manuscripts.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/list", async (
            [AsParameters] ManuscriptListRequest request,
            IQueryHandler<GetManuscriptListQuery, PagedList<ManuscriptListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptListQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                Year = request.Year,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };

            Result<PagedList<ManuscriptListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

internal sealed record ManuscriptListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
