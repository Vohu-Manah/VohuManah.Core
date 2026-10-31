using FluentValidation;

namespace Application.Library.Cities.Create;

internal sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام شهر الزامی است")
            .MaximumLength(50).WithMessage("نام شهر باید حداکثر ۵۰ کاراکتر باشد");
    }
}


