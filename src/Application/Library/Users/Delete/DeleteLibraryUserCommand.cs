using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Users.Delete;

public sealed record DeleteLibraryUserCommand(
    [Display(Name = "شناسه")] long Id) : ICommand;
