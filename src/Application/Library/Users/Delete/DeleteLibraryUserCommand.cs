using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Users.Delete;

public sealed record DeleteLibraryUserCommand(
    [Display(Name = "شناسه")] long Id) : ICommand;
