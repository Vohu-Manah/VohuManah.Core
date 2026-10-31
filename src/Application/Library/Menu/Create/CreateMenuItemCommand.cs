using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Menu.Create;

public sealed record CreateMenuItemCommand(
    [Display(Name = "عنوان")] string Label,
    [Display(Name = "مسیر")] string? Path,
    [Display(Name = "آیکن")] string? Icon,
    [Display(Name = "منوی والد")] int? ParentId,
    [Display(Name = "ترتیب نمایش")] int DisplayOrder,
    [Display(Name = "فعال")] bool IsActive = true) : ICommand<MenuItemResponse>;

