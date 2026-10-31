using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAllRoles;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Settings.CreateRole;

public sealed record CreateRoleCommand([Display(Name = "نام نقش")] string Name) : ICommand<RoleResponse>;

