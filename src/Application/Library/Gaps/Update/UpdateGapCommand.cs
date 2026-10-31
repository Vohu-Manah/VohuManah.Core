using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Gaps.Update;

public sealed record UpdateGapCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "عنوان فاصله")] string Title,
    [Display(Name = "یادداشت")] string Note,
    [Display(Name = "وضعیت")] bool State) : ICommand;

