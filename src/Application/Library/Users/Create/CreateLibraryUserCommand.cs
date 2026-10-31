using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Users.Create;

public sealed record CreateLibraryUserCommand(
    [Display(Name = "نام کاربری")] string UserName,
    [Display(Name = "رمز عبور")] string Password,
    [Display(Name = "نام")] string Name,
    [Display(Name = "نام خانوادگی")] string LastName) : ICommand<string>;


