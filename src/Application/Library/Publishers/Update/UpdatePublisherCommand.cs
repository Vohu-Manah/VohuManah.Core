using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Publishers.Update;

public sealed record UpdatePublisherCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "نام ناشر")] string Name,
    [Display(Name = "نام مدیر")] string ManagerName,
    [Display(Name = "شهر")] int PlaceId,
    [Display(Name = "آدرس")] string Address,
    [Display(Name = "تلفن")] string Telephone,
    [Display(Name = "وبسایت")] string Website,
    [Display(Name = "ایمیل")] string Email) : ICommand;


