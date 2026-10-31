using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Settings.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(
    [Display(Name = "شناسه کاربر")] long UserId,
    [Display(Name = "شناسه نقش")] int RoleId) : ICommand;

