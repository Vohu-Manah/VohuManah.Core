using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Menu.Create;

internal sealed class CreateMenuItemCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateMenuItemCommand, MenuItemResponse>
{
    public async Task<Result<MenuItemResponse>> Handle(CreateMenuItemCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        
        if (command.ParentId.HasValue)
        {
            bool parentExists = await menuItems.AnyAsync(
                m => m.Id == command.ParentId.Value,
                cancellationToken).ConfigureAwait(false);

            if (!parentExists)
            {
                return Result.Failure<MenuItemResponse>(
                    Error.NotFound("Menu.ParentNotFound", "منوی والد یافت نشد"));
            }
        }

        var menuItem = new MenuItem
        {
            Label = command.Label.Trim(),
            Path = command.Path?.Trim(),
            Icon = command.Icon?.Trim(),
            ParentId = command.ParentId,
            DisplayOrder = command.DisplayOrder,
            IsActive = command.IsActive,
            IsSystem = false
        };

        menuItems.Add(menuItem);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success(new MenuItemResponse
        {
            Id = menuItem.Id,
            Label = menuItem.Label,
            Path = menuItem.Path,
            Icon = menuItem.Icon,
            ParentId = menuItem.ParentId,
            DisplayOrder = menuItem.DisplayOrder,
            IsActive = menuItem.IsActive,
            IsSystem = menuItem.IsSystem
        });
    }
}

