using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Settings.Update;

public sealed record UpdateSettingsCommand(
    [Display(Name = "نام تنظیمات")] string Name,
    [Display(Name = "مقدار تنظیمات")] string Value) : ICommand;

