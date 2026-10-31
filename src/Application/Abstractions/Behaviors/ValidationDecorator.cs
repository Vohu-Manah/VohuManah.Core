using Application.Abstractions.Messaging;
using FluentValidation;
using FluentValidation.Results;
using SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            List<ValidationFailure> typeValidationFailures = ValidateDataTypes(command);

            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            ValidationFailure[] allFailures = [.. typeValidationFailures, .. validationFailures];

            if (allFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError<TCommand>(allFailures));
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            List<ValidationFailure> typeValidationFailures = ValidateDataTypes(command);

            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            ValidationFailure[] allFailures = [.. typeValidationFailures, .. validationFailures];

            if (allFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure(CreateValidationError<TCommand>(allFailures));
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationError CreateValidationError<TCommand>(ValidationFailure[] validationFailures)
    {
        Type commandType = typeof(TCommand);
        Error[] errors = validationFailures.Select(f =>
        {
            string displayName = GetDisplayName(f.PropertyName, commandType);
            string errorMessage = f.ErrorMessage;
            if (!string.IsNullOrWhiteSpace(displayName) && displayName != f.PropertyName)
            {
                errorMessage = $"{displayName}: {errorMessage}";
            }
            return Error.Problem(f.PropertyName, errorMessage);
        }).ToArray();

        return new ValidationError(errors);
    }

    private static string GetDisplayName(string propertyName, Type type)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || type == null)
        {
            return propertyName;
        }

        PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property != null)
        {
            DisplayAttribute? displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name))
            {
                return displayAttribute.Name;
            }

            DisplayNameAttribute? displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttribute != null && !string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName))
            {
                return displayNameAttribute.DisplayName;
            }
        }

        return propertyName;
    }


    private static List<ValidationFailure> ValidateDataTypes<TCommand>(TCommand command)
    {
        List<ValidationFailure> failures = new();
        Type commandType = typeof(TCommand);
        PropertyInfo[] properties = commandType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            object? value = property.GetValue(command);
            Type propertyType = property.PropertyType;
            string propertyName = property.Name;

            if (value == null)
            {
                continue;
            }

            Type valueType = value.GetType();

            if (!propertyType.IsAssignableFrom(valueType))
            {
                string displayName = GetDisplayName(propertyName, commandType);
                string expectedTypeName = GetTypeName(propertyType);
                string actualTypeName = GetTypeName(valueType);
                string errorMessage = $"نوع داده نامعتبر است. انتظار می رود: {expectedTypeName}، اما دریافت شده: {actualTypeName}";

                if (!string.IsNullOrWhiteSpace(displayName) && displayName != propertyName)
                {
                    errorMessage = $"{displayName}: {errorMessage}";
                }

                failures.Add(new ValidationFailure(propertyName, errorMessage)
                {
                    ErrorCode = "InvalidType"
                });
            }
        }

        return failures;
    }

    private static string GetTypeName(Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.Name switch
        {
            "String" => "متن",
            "Int32" => "عدد صحیح",
            "Int64" => "عدد صحیح بزرگ",
            "Double" => "عدد اعشاری",
            "Decimal" => "عدد اعشاری",
            "Boolean" => "بله/خیر",
            "DateTime" => "تاریخ و زمان",
            "DateOnly" => "تاریخ",
            _ => underlyingType.Name
        };
    }
}
