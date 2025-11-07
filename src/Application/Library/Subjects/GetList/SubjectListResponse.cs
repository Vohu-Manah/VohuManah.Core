namespace Application.Library.Subjects.GetList;

public sealed record SubjectListResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}

