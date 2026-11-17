using VohuManah.Application.Abstractions.Messaging;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetList;

public sealed record GetBookListQuery : IQuery<PagedList<BookListResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
