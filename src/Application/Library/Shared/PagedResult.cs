namespace Application.Library.Shared;





public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = new List<T>();

    
    public long TotalCount { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;
}
