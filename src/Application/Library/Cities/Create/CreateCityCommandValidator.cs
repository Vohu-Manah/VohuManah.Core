using FluentValidation;

namespace Application.Library.Cities.Create;

internal sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(50);
    }
}


