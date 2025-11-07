using SharedKernel;

namespace Domain.Library;

public static class UserErrors
{
    public static Error NotFound(string userName) => Error.NotFound(
        "Users.NotFound",
        $"The user with the UserName = '{userName}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error UserNameNotUnique = Error.Conflict(
        "Users.UserNameNotUnique",
        "The provided username is not unique");

    public static readonly Error InvalidCredentials = Error.Failure(
        "Users.InvalidCredentials",
        "Invalid username or password");

    public static readonly Error CannotDeleteLastUser = Error.Failure(
        "Users.CannotDeleteLastUser",
        "Cannot delete the last user");
}

