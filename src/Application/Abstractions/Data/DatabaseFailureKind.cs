namespace VohuManah.Application.Abstractions.Data;

public enum DatabaseFailureKind
{
    None = 0,
    FkReferenced = 1,
    DuplicateKey = 2,
}
