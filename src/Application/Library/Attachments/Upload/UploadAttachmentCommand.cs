using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Attachments.Upload;

public sealed record UploadAttachmentCommand(
    [Display(Name = "نوع موجودیت")] string EntityType,
    [Display(Name = "شناسه موجودیت")] long EntityId,
    [Display(Name = "نام فایل")] string FileName,
    [Display(Name = "نوع محتوا")] string ContentType,
    [Display(Name = "حجم فایل")] long FileSize,
    [Display(Name = "فایل")] Stream FileStream,
    [Display(Name = "توضیحات")] string? Description,
    [Display(Name = "ایجادکننده")] string CreatedBy,
    [Display(Name = "پیوست اصلی")] bool IsMain = false) : ICommand<long>;


