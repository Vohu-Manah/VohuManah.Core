using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Settings.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    [Display(Name = "شناسه نقش")] int RoleId,
    [Display(Name = "نام های endpoint")] IReadOnlyCollection<string> PermissionNames) : ICommand;

