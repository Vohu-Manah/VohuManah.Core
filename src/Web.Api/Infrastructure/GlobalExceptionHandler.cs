using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Infrastructure;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "استثنای مدیریت نشده رخ داد");

        var (statusCode, title, detail, errorMessage) = GetErrorDetails(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = title,
            Detail = detail,
            Extensions = { ["errorMessage"] = errorMessage }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail, string ErrorMessage) GetErrorDetails(Exception exception)
    {
        if (exception is DbUpdateException dbUpdateException)
        {
            var sqlException = dbUpdateException.InnerException as SqlException;

            if (sqlException != null)
            {
                if (sqlException.Number == 547)
                {
                    return (
                        StatusCodes.Status409Conflict,
                        "خطای وابستگی",
                        "این مورد در بخش های دیگر برنامه استفاده شده است و قابل حذف نیست.",
                        "این مورد در بخش های دیگر برنامه استفاده شده است و قابل حذف نیست."
                    );
                }

                if (sqlException.Number is 2601 or 2627)
                {
                    return (
                        StatusCodes.Status409Conflict,
                        "خطای یکتا بودن",
                        "یکی از مقادیر ارسال شده از قبل در سیستم ثبت شده است.",
                        "یکی از مقادیر ارسال شده از قبل در سیستم ثبت شده است."
                    );
                }
            }
            }

        if (exception.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            return (
                StatusCodes.Status409Conflict,
                "خطای وابستگی",
                "این مورد در بخش های دیگر برنامه استفاده شده است و قابل حذف نیست.",
                "این مورد در بخش های دیگر برنامه استفاده شده است و قابل حذف نیست."
            );
        }

        return (
            StatusCodes.Status500InternalServerError,
            "خطای سرور",
            exception.Message,
            exception.Message



        );
    }
}
