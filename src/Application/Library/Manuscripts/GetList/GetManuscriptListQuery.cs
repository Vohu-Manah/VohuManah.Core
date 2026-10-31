using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Library.Manuscripts.GetList;

public sealed record GetManuscriptListQuery : IQuery<PagedList<ManuscriptListResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
