using SharedKernel;

namespace Domain.Library;

public static class UserErrors
{
    public static Error NotFound(string userName) => Error.NotFound(
        "Users.NotFound",
        $"کاربر با نام کاربری '{userName}' یافت نشد");

    public static Error NotFound(long userId) => Error.NotFound(
        "Users.NotFound",
        $"کاربر با شناسه '{userId}' یافت نشد");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "شما مجاز به انجام این عملیات نیستید.");

    public static readonly Error UserNameNotUnique = Error.Conflict(
        "Users.UserNameNotUnique",
        "نام کاربری ارائه شده یکتا نیست");

    public static readonly Error InvalidCredentials = Error.Failure(
        "Users.InvalidCredentials",
        "نام کاربری یا رمز عبور نامعتبر است");

    public static readonly Error CannotDeleteLastUser = Error.Failure(
        "Users.CannotDeleteLastUser",
        "نمی توان آخرین کاربر را حذف کرد");
}

