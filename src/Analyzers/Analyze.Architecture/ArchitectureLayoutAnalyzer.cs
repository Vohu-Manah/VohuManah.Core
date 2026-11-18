using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyze.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArchitectureLayoutAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "Architecture";
    private const string MessagingNamespace = "VohuManah.Application.Abstractions.Messaging";
    private const string EfNamespace = "Microsoft.EntityFrameworkCore";

    private static DiagnosticDescriptor Create(string id, string title, string message) =>
        new(id, title, message, Category, DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UseCaseLocation = Create("ARCH001", "Use-case type in the wrong folder",
        "'{0}' is a {1} and must live in Application/Library/<Feature>/<UseCase>/ (found: {2})");
    public static readonly DiagnosticDescriptor UseCaseNaming = Create("ARCH002", "Use-case type has the wrong name",
        "'{0}' is a {1}; its name must end with '{2}'");
    public static readonly DiagnosticDescriptor UseCaseShape = Create("ARCH003", "Use-case type has the wrong shape",
        "'{0}' ({1}) must be declared 'internal sealed'");
    public static readonly DiagnosticDescriptor FileName = Create("ARCH004", "File name must match the type name",
        "File '{0}.cs' must be named after the type it declares: '{1}.cs'");
    public static readonly DiagnosticDescriptor EndpointShape = Create("ARCH005", "Endpoint in the wrong place or shape",
        "Endpoint '{0}' {1}");
    public static readonly DiagnosticDescriptor DomainLayout = Create("ARCH006", "Domain type in the wrong place or shape",
        "Domain type '{0}' {1}");
    public static readonly DiagnosticDescriptor InfrastructureVisibility = Create("ARCH007", "Infrastructure type is too visible",
        "Infrastructure type '{0}' {1}");
    public static readonly DiagnosticDescriptor EfConfiguration = Create("ARCH008", "EF configuration in the wrong place or shape",
        "EF configuration '{0}' {1}");
    public static readonly DiagnosticDescriptor ForbiddenFolder = Create("ARCH009", "Dumping-ground folder",
        "Folder '{0}' is not allowed (Helper, Utils, Utilities, Common, Misc, Manager); name folders after what they contain");
    public static readonly DiagnosticDescriptor DuplicatePermission = Create("ARCH010", "Duplicate permission value",
        "Permission value '{0}' is already defined by '{1}'");
    public static readonly DiagnosticDescriptor ForbiddenReference = Create("ARCH011", "Forbidden project dependency",
        "Project '{0}' must not depend on '{1}' (layer dependency rule)");

    private static readonly string[] DumpingGroundFolders =
        { "Helper", "Helpers", "Util", "Utils", "Utility", "Utilities", "Common", "Misc", "Manager", "Managers" };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        UseCaseLocation, UseCaseNaming, UseCaseShape, FileName, EndpointShape, DomainLayout,
        InfrastructureVisibility, EfConfiguration, ForbiddenFolder, DuplicatePermission, ForbiddenReference);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(start =>
        {
            string? layer = LayerOf(start.Compilation.AssemblyName);
            if (layer is null)
            {
                return;
            }

            string? projectDirectory = null;
            if (start.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.MSBuildProjectDirectory", out string? dir))
            {
                projectDirectory = dir;
            }

            start.RegisterSymbolAction(c => AnalyzeType(c, layer, projectDirectory), SymbolKind.NamedType);
            start.RegisterCompilationEndAction(c => AnalyzeReferences(c, layer));
            if (layer == "Application")
            {
                start.RegisterCompilationEndAction(AnalyzePermissionNames);
            }
        });
    }

    private static string? LayerOf(string? assemblyName) => assemblyName switch
    {
        "VohuManah.SharedKernel" => "SharedKernel",
        "VohuManah.Domain" => "Domain",
        "VohuManah.Application" => "Application",
        "VohuManah.Infrastructure" => "Infrastructure",
        "VohuManah.Web.Api" => "Web.Api",
        _ => null,
    };

    private static void AnalyzeReferences(CompilationAnalysisContext context, string layer)
    {
        string[] allowed = layer switch
        {
            "SharedKernel" => Array.Empty<string>(),
            "Domain" => new[] { "SharedKernel" },
            "Application" => new[] { "Domain", "SharedKernel" },
            "Infrastructure" => new[] { "Application", "Domain", "SharedKernel", "External.ReportViewer" },
            _ => new[] { "Application", "Infrastructure", "Domain", "SharedKernel", "External.ReportViewer" },
        };

        const string prefix = "VohuManah.";
        foreach (AssemblyIdentity reference in context.Compilation.ReferencedAssemblyNames)
        {
            if (!reference.Name.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            string other = reference.Name.Substring(prefix.Length);
            if (Array.IndexOf(allowed, other) < 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(ForbiddenReference, Location.None, layer, other));
            }
        }
    }

    private static void AnalyzePermissionNames(CompilationAnalysisContext context)
    {
        INamedTypeSymbol? root = context.Compilation.GetTypeByMetadataName("VohuManah.Application.Abstractions.Authorization.PermissionNames");
        if (root is null)
        {
            return;
        }

        var seen = new Dictionary<string, IFieldSymbol>(StringComparer.Ordinal);

        void Visit(INamedTypeSymbol type)
        {
            foreach (ISymbol member in type.GetMembers())
            {
                if (member is IFieldSymbol { IsConst: true, ConstantValue: string value } field)
                {
                    if (seen.TryGetValue(value, out IFieldSymbol? first))
                    {
                        if (field.Locations.Length > 0 && field.Locations[0].IsInSource)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(DuplicatePermission, field.Locations[0], value,
                                first.ContainingType.Name + "." + first.Name));
                        }
                    }
                    else
                    {
                        seen[value] = field;
                    }
                }
                else if (member is INamedTypeSymbol nested)
                {
                    Visit(nested);
                }
            }
        }

        Visit(root);
    }

    private static void AnalyzeType(SymbolAnalysisContext context, string layer, string? projectDirectory)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.ContainingType is not null || type.IsImplicitlyDeclared || type.Locations.Length == 0)
        {
            return;
        }

        Location location = type.Locations[0];
        if (!location.IsInSource || location.SourceTree is null)
        {
            return;
        }

        string path = location.SourceTree.FilePath.Replace('\\', '/');
        if (IsGenerated(path))
        {
            return;
        }

        string[] segments = RelativeSegments(path, projectDirectory, layer);
        if (segments.Length == 0)
        {
            return;
        }

        string fileName = Path.GetFileNameWithoutExtension(segments[segments.Length - 1]);
        for (int i = 0; i < segments.Length - 1; i++)
        {
            if (Array.IndexOf(DumpingGroundFolders, segments[i]) >= 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(ForbiddenFolder, location, segments[i]));
                break;
            }
        }

        switch (layer)
        {
            case "Application":
                AnalyzeApplication(context, type, location, segments, fileName);
                break;
            case "Domain":
                AnalyzeDomain(context, type, location, segments, fileName);
                break;
            case "Infrastructure":
                AnalyzeInfrastructure(context, type, location, segments);
                break;
            case "Web.Api":
                AnalyzeWebApi(context, type, location, segments);
                break;
        }
    }

    private static void AnalyzeApplication(SymbolAnalysisContext context, INamedTypeSymbol type, Location location, string[] segments, string fileName)
    {
        string role;
        string suffix;
        bool strictShape = false;
        if (ImplementsAny(type, MessagingNamespace, "ICommandHandler", "IQueryHandler"))
        {
            role = "handler";
            suffix = "Handler";
            strictShape = true;
        }
        else if (DerivesFrom(type, "FluentValidation", "AbstractValidator"))
        {
            role = "validator";
            suffix = "Validator";
            strictShape = true;
        }
        else if (ImplementsAny(type, MessagingNamespace, "IQuery"))
        {
            role = "query";
            suffix = "Query";
        }
        else if (ImplementsAny(type, MessagingNamespace, "ICommand"))
        {
            role = "command";
            suffix = "Command";
        }
        else
        {
            return;
        }

        if (segments.Length < 4 || segments[0] != "Library")
        {
            context.ReportDiagnostic(Diagnostic.Create(UseCaseLocation, location, type.Name, role, string.Join("/", segments)));
        }

        if (!type.Name.EndsWith(suffix, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(UseCaseNaming, location, type.Name, role, suffix));
        }

        if (strictShape && !(type.DeclaredAccessibility == Accessibility.Internal && type.IsSealed))
        {
            context.ReportDiagnostic(Diagnostic.Create(UseCaseShape, location, type.Name, role));
        }

        if (!string.Equals(fileName, type.Name, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(FileName, location, fileName, type.Name));
        }
    }

    private static void AnalyzeDomain(SymbolAnalysisContext context, INamedTypeSymbol type, Location location, string[] segments, string fileName)
    {
        if (segments.Length < 2)
        {
            context.ReportDiagnostic(Diagnostic.Create(DomainLayout, location, type.Name,
                "must live in an aggregate folder (Domain/<Aggregate>/), not in the project root"));
        }

        if (type.TypeKind == TypeKind.Class)
        {
            if (type.Name.EndsWith("Errors", StringComparison.Ordinal))
            {
                if (!type.IsStatic)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DomainLayout, location, type.Name, "must be a static class"));
                }

                string entity = type.Name.Substring(0, type.Name.Length - "Errors".Length);
                if (context.Compilation.GetTypeByMetadataName(type.ContainingNamespace.ToDisplayString() + "." + entity) is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DomainLayout, location, type.Name,
                        "must sit in the same folder/namespace as its entity '" + entity + "'"));
                }
            }
            else if (!type.IsStatic && !type.IsAbstract && !type.IsSealed)
            {
                context.ReportDiagnostic(Diagnostic.Create(DomainLayout, location, type.Name, "must be sealed"));
            }
        }

        if (!string.Equals(fileName, type.Name, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(FileName, location, fileName, type.Name));
        }
    }

    private static void AnalyzeInfrastructure(SymbolAnalysisContext context, INamedTypeSymbol type, Location location, string[] segments)
    {
        if (type.TypeKind == TypeKind.Interface && type.DeclaredAccessibility == Accessibility.Public)
        {
            context.ReportDiagnostic(Diagnostic.Create(InfrastructureVisibility, location, type.Name,
                "is a public interface: ports belong in Application/Abstractions, Infrastructure-only contracts must be internal"));
        }
        else if (type.TypeKind == TypeKind.Class && !type.IsStatic && type.DeclaredAccessibility == Accessibility.Public
                 && !DerivesFrom(type, EfNamespace, "DbContext")
                 && type.AllInterfaces.Any(i => i.ContainingNamespace.ToDisplayString().StartsWith("VohuManah", StringComparison.Ordinal)))
        {
            context.ReportDiagnostic(Diagnostic.Create(InfrastructureVisibility, location, type.Name,
                "implements an application port and must be 'internal sealed'"));
        }

        INamedTypeSymbol? configuration = type.AllInterfaces.FirstOrDefault(i => i.Name == "IEntityTypeConfiguration"
            && i.ContainingNamespace.ToDisplayString() == EfNamespace);
        if (configuration is null || configuration.TypeArguments.Length == 0)
        {
            return;
        }

        string entity = configuration.TypeArguments[0].Name;
        if (!(type.DeclaredAccessibility == Accessibility.Internal && type.IsSealed))
        {
            context.ReportDiagnostic(Diagnostic.Create(EfConfiguration, location, type.Name, "must be 'internal sealed'"));
        }

        if (!string.Equals(type.Name, entity + "Configuration", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(EfConfiguration, location, type.Name, "must be named '" + entity + "Configuration'"));
        }

        if (segments.Length < 3 || segments[0] != "Database" || segments[1] != "Configurations")
        {
            context.ReportDiagnostic(Diagnostic.Create(EfConfiguration, location, type.Name, "must live in Infrastructure/Database/Configurations"));
        }
    }

    private static void AnalyzeWebApi(SymbolAnalysisContext context, INamedTypeSymbol type, Location location, string[] segments)
    {
        if (type.Name == "BaseEndpoint" || !DerivesFrom(type, "VohuManah.Web.Api.Endpoints", "BaseEndpoint"))
        {
            return;
        }

        if (!(type.DeclaredAccessibility == Accessibility.Internal && type.IsSealed))
        {
            context.ReportDiagnostic(Diagnostic.Create(EndpointShape, location, type.Name, "must be declared 'internal sealed'"));
        }

        if (segments[0] != "Endpoints")
        {
            context.ReportDiagnostic(Diagnostic.Create(EndpointShape, location, type.Name, "must live under Web.Api/Endpoints/"));
        }
    }

    private static bool IsGenerated(string path) =>
        path.IndexOf("/obj/", StringComparison.OrdinalIgnoreCase) >= 0
        || path.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) >= 0
        || path.IndexOf("/Migrations/", StringComparison.OrdinalIgnoreCase) >= 0
        || path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase);

    private static string[] RelativeSegments(string path, string? projectDirectory, string layerFolder)
    {
        string relative;
        string normalizedDirectory = projectDirectory is null ? string.Empty : projectDirectory.Replace('\\', '/').TrimEnd('/');
        if (normalizedDirectory.Length > 0 && path.StartsWith(normalizedDirectory + "/", StringComparison.OrdinalIgnoreCase))
        {
            relative = path.Substring(normalizedDirectory.Length + 1);
        }
        else
        {
            int index = path.LastIndexOf("/" + layerFolder + "/", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return Array.Empty<string>();
            }

            relative = path.Substring(index + layerFolder.Length + 2);
        }

        return relative.Split('/');
    }

    private static bool ImplementsAny(INamedTypeSymbol type, string ns, params string[] names) =>
        type.AllInterfaces.Any(i => Array.IndexOf(names, i.Name) >= 0 && i.ContainingNamespace.ToDisplayString() == ns);

    private static bool DerivesFrom(INamedTypeSymbol type, string ns, string name)
    {
        for (INamedTypeSymbol? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            if (baseType.Name == name && baseType.ContainingNamespace.ToDisplayString() == ns)
            {
                return true;
            }
        }

        return false;
    }
}
