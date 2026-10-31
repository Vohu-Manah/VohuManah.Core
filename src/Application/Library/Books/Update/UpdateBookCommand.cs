using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Books.Update;

public sealed record UpdateBookCommand(
    [Display(Name = "شناسه")] long Id,
    [Display(Name = "نام کتاب")] string Name,
    [Display(Name = "نویسنده")] string Author,
    [Display(Name = "مترجم")] string Translator,
    [Display(Name = "ویراستار")] string Editor,
    [Display(Name = "مصحح")] string Corrector,
    [Display(Name = "ناشر")] int PublisherId,
    [Display(Name = "محل چاپ")] int PublishPlaceId,
    [Display(Name = "سال انتشار")] string PublishYear,
    [Display(Name = "ترتیب انتشار")] string PublishOrder,
    [Display(Name = "شابک")] string Isbn,
    [Display(Name = "شماره")] string No,
    [Display(Name = "تعداد جلد")] int VolumeCount,
    [Display(Name = "زبان")] int LanguageId,
    [Display(Name = "موضوع")] int SubjectId,
    [Display(Name = "ردیف قفسه")] string BookShelfRow) : ICommand;

