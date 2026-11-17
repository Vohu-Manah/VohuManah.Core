using VohuManah.Application.Abstractions.Messaging;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetList;

public sealed record GetPublicationListQuery : IQuery<PagedList<PublicationListResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
