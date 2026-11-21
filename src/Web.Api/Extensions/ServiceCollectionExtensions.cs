namespace VohuManah.Web.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static WebApplicationBuilder ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 150L * 1024 * 1024 * 1024;
        });

        return builder;
    }

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
