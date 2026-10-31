using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Users.Login;

public sealed record LoginLibraryUserCommand(
    [Display(Name = "نام کاربری")] string UserName,
    [Display(Name = "رمز عبور")] string Password,
    string? HumanVerifyToken) : ICommand<LoginLibraryUserResponse>;


