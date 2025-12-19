namespace SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base(
            "Validation.General",
            "یک یا چند خطای اعتبارسنجی رخ داده است",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
