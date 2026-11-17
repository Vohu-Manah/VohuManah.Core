using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

using VohuManah.Application.Abstractions.Authorization;

[RequireRole(PermissionNames.Publications.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/list", async (
            [AsParameters] PublicationListRequest request,
            IQueryHandler<GetPublicationListQuery, PagedList<PublicationListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationListQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                Year = request.Year,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };

            Result<PagedList<PublicationListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

internal sealed record PublicationListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
