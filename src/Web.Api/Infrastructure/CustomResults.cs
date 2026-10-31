using SharedKernel;

namespace Web.Api.Infrastructure;

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        string? detail = result.Error is ValidationError ? null : GetDetail(result.Error);

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: detail,
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetExtensions(result));

        static string GetTitle(Error error) =>
            error.Type switch
            {
                ErrorType.Validation => error.Code,
                ErrorType.Problem => error.Code,
                ErrorType.NotFound => error.Code,
                ErrorType.Conflict => error.Code,
                _ => "خطای سرور"
            };

        static string GetDetail(Error error) =>
            string.IsNullOrWhiteSpace(error.Description)
                ? "یک خطای غیرمنتظره رخ داده است"
                : error.Description;

        static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

        static Dictionary<string, object?> GetExtensions(Result result)
        {
            var extensions = new Dictionary<string, object?>();

            if (result.Error is ValidationError validationError)
            {

                Dictionary<string, List<string>> errorsDict = new Dictionary<string, List<string>>();
                foreach (var error in validationError.Errors)
                {
                    string fieldName = error.Code;
                    if (!string.IsNullOrWhiteSpace(fieldName))
                    {
                        string camelCaseField = char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
                        if (!errorsDict.TryGetValue(camelCaseField, out List<string>? errorList))
                        {
                            errorList = new List<string>();
                            errorsDict[camelCaseField] = errorList;
                        }
                        errorList.Add(error.Description);
                    }
                }
                Dictionary<string, string[]> finalErrorsDict = errorsDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
                extensions["errors"] = finalErrorsDict;
            }
            else
            {
                extensions["errorMessage"] = GetDetail(result.Error);
            }

            return extensions;
        }
    }

    public static IResult Problem(string title, string detail, int? statusCode = null)
    {
        return Results.Problem(
            title: title,
            detail: detail,
            statusCode: statusCode ?? StatusCodes.Status500InternalServerError);
    }
}

