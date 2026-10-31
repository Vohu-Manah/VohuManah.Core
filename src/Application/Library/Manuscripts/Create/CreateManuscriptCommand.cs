using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Manuscripts.Create;

public sealed record CreateManuscriptCommand(
    [Display(Name = "نام نسخه خطی")] string Name,
    [Display(Name = "نویسنده")] string Author,
    [Display(Name = "محل نگارش")] int WritingPlaceId,
    [Display(Name = "سال")] string Year,
    [Display(Name = "تعداد صفحات")] int PageCount,
    [Display(Name = "اندازه")] string Size,
    [Display(Name = "فاصله")] int GapId,
    [Display(Name = "شماره نسخه")] string VersionNo,
    [Display(Name = "زبان")] int LanguageId,
    [Display(Name = "موضوع")] int SubjectId) : ICommand<int>;

