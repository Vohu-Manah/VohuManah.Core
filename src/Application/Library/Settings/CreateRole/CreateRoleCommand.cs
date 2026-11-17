using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetAllRoles;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Settings.CreateRole;

public sealed record CreateRoleCommand([Display(Name = "نام نقش")] string Name) : ICommand<RoleResponse>;

