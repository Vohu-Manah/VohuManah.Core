namespace Web.Api.Endpoints.Attributes;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RequireRoleAttribute : Attribute
{
    public string EndpointName { get; }
    public bool SkipDatabaseCheck { get; set; }

#pragma warning disable S1133
    [Obsolete("استفاده از Roles منسوخ شده است. از EndpointName استفاده کنید.")]
    public string[] Roles { get; }
#pragma warning restore S1133 


#pragma warning disable S1133 
    [Obsolete("استفاده از RequireAll منسوخ شده است.")]
    public bool RequireAll { get; set; }
#pragma warning restore S1133 


    public RequireRoleAttribute(string endpointName)
    {
        EndpointName = endpointName ?? throw new ArgumentNullException(nameof(endpointName));
#pragma warning disable CS0618 
        Roles = Array.Empty<string>();
#pragma warning restore CS0618 
    }

#pragma warning disable S1133 
    [Obsolete("از constructor با endpointName استفاده کنید.")]
    public RequireRoleAttribute(params string[] roles)
#pragma warning restore S1133 
    {
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        EndpointName = string.Empty;
    }
}

