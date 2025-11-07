using SharedKernel;

namespace Domain.Library;

public static class BookErrors
{
    public static Error NotFound(long id) => Error.NotFound(
        "Books.NotFound",
        $"The book with the Id = '{id}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Books.NameNotUnique",
        "A book with this name already exists");
}


