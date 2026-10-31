using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Settings.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(
    [Display(Name = "شناسه کاربر")] long UserId,
    [Display(Name = "شناسه نقش")] int RoleId) : ICommand;

