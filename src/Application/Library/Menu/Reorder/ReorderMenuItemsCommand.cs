using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Menu.Reorder;

public sealed record MenuItemOrder(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "ترتیب نمایش")] int DisplayOrder,
    [Display(Name = "منوی والد")] int? ParentId);

public sealed record ReorderMenuItemsCommand(
    [Display(Name = "ترتیب منوها")] List<MenuItemOrder> Items) : ICommand;

