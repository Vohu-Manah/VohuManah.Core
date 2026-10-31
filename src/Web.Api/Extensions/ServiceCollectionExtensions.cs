namespace Web.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(type =>
            {
                if (type.IsGenericType)
                {
                    
                    
                    
                    string genericArgs = string.Join(
                        "_",
                        type.GenericTypeArguments.Select(a => a.FullName!.Replace('+', '-')));
                    string typeName = type.Name.Split('`')[0];
                    return $"{typeName}Of{genericArgs}";
                }

                return type.FullName!.Replace('+', '-');
            });
        });

        return services;
    }
}
