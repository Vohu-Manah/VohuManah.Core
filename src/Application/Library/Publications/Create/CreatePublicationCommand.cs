using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Publications.Create;

public sealed record CreatePublicationCommand(
    [Display(Name = "نام نشریه")] string Name,
    [Display(Name = "نوع نشریه")] int TypeId,
    [Display(Name = "امتیازدار")] string Concessionaire,
    [Display(Name = "مدیر مسئول")] string ResponsibleDirector,
    [Display(Name = "ویراستار")] string Editor,
    [Display(Name = "سال")] string Year,
    [Display(Name = "شماره مجله")] string JournalNo,
    [Display(Name = "تاریخ انتشار")] string PublishDate,
    [Display(Name = "محل چاپ")] int PublishPlaceId,
    [Display(Name = "شماره")] string No,
    [Display(Name = "دوره")] string Period,
    [Display(Name = "زبان")] int LanguageId,
    [Display(Name = "موضوع")] int SubjectId) : ICommand<int>;

