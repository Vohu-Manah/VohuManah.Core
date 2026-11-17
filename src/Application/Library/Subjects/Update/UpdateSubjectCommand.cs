using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Subjects.Update;

public sealed record UpdateSubjectCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "عنوان موضوع")] string Title) : ICommand;


