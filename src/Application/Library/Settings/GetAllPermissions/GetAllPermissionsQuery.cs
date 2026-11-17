using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAllPermissions;

public sealed record GetAllPermissionsQuery : IQuery<Dictionary<string, List<string>>>;


