using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Gaps.Create;

public sealed record CreateGapCommand(
    [Display(Name = "عنوان فاصله")] string Title,
    [Display(Name = "یادداشت")] string Note,
    [Display(Name = "وضعیت")] bool State) : ICommand<int>;

