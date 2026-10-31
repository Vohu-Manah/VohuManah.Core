namespace SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(IReadOnlyList<Error> errors)
        : base(
            "Validation.General",
            "یک یا چند خطای اعتبارسنجی رخ داده است",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public IReadOnlyList<Error> Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToList());
}
