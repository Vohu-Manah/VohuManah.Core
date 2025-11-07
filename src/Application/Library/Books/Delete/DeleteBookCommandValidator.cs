using FluentValidation;

namespace Application.Library.Books.Delete;

internal sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}

