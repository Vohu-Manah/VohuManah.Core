using SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Api.Infrastructure;

public static class ModelBindingValidator
{

    public static ValidationError? ValidateRequestTypes<TRequest>(TRequest request)
        where TRequest : class
    {
        List<Error> errors = new();
        Type requestType = typeof(TRequest);
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            object? value = property.GetValue(request);
            Type propertyType = property.PropertyType;
            string propertyName = property.Name;

            if (value == null)
            {
                continue;
            }

            Type valueType = value.GetType();

            if (!IsTypeCompatible(propertyType, valueType))
            {
                string displayName = GetDisplayName(propertyName, requestType);
                string expectedTypeName = GetTypeName(propertyType);
                string actualTypeName = GetTypeName(valueType);
                string errorMessage = $"نوع داده نامعتبر است. انتظار می رود: {expectedTypeName}، اما دریافت شده: {actualTypeName}";

                if (!string.IsNullOrWhiteSpace(displayName) && displayName != propertyName)
                {
                    errorMessage = $"{displayName}: {errorMessage}";
                }

                errors.Add(Error.Validation(propertyName, errorMessage));
            }
        }

        return errors.Count > 0 ? new ValidationError(errors.ToArray()) : null;
    }

    private static bool IsTypeCompatible(Type expectedType, Type actualType)
    {
        if (expectedType == actualType)
        {
            return true;
        }

        Type underlyingExpectedType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
        Type underlyingActualType = Nullable.GetUnderlyingType(actualType) ?? actualType;

        if (underlyingExpectedType == underlyingActualType)
        {
            return true;
        }

        if (underlyingExpectedType.IsAssignableFrom(underlyingActualType))
        {
            return true;
        }

        if (IsNumericType(underlyingExpectedType) && IsNumericType(underlyingActualType))
        {
            return CanConvertNumeric(underlyingActualType, underlyingExpectedType);
        }

        return false;
    }
    private static bool IsNumericType(Type type)
    {
        return type == typeof(int) ||
               type == typeof(long) ||
               type == typeof(short) ||
               type == typeof(byte) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(decimal);
    }


    private static bool CanConvertNumeric(Type from, Type to)
    {
        if (from == typeof(int) && (to == typeof(long) || to == typeof(double) || to == typeof(decimal)))
        {
            return true;
        }

        if (from == typeof(short) && (to == typeof(int) || to == typeof(long)))
        {
            return true;
        }

        if (from == typeof(byte) && (to == typeof(short) || to == typeof(int) || to == typeof(long)))
        {
            return true;
        }

        return false;
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
        }

        return propertyName;
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
            "Single" => "عدد اعشاری",
            "Boolean" => "بله/خیر",
            "DateTime" => "تاریخ و زمان",
            "DateOnly" => "تاریخ",
            "TimeOnly" => "زمان",
            _ => underlyingType.Name
        };
    }
}

