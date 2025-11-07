namespace Application.Library.Subjects.GetById;

public sealed record SubjectResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}

