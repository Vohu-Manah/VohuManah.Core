using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Visits.Record;

public sealed record RecordVisitCommand(
    [Display(Name = "مسیر")] string Path) : ICommand;
