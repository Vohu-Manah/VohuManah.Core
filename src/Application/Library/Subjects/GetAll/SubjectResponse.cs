namespace Application.Library.Subjects.GetAll;

public sealed record SubjectResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}


