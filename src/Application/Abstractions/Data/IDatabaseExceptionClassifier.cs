namespace VohuManah.Application.Abstractions.Data;

public interface IDatabaseExceptionClassifier
{
    DatabaseFailureKind Classify(Exception exception);
}
