using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Visits.Record;

public sealed record RecordVisitCommand(
    [Display(Name = "مسیر")] string Path) : ICommand;
