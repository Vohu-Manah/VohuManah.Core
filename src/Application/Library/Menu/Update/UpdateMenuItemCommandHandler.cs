using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.Update;

internal sealed class UpdateMenuItemCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateMenuItemCommand, MenuItemResponse>
{
    public async Task<Result<MenuItemResponse>> Handle(UpdateMenuItemCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        MenuItem? menuItem = await menuItems
            .FirstOrDefaultAsync(m => m.Id == command.Id, cancellationToken)
            .ConfigureAwait(false);

        if (menuItem == null)
        {
            return Result.Failure<MenuItemResponse>(
                Error.NotFound("Menu.NotFound", "آیتم منو یافت نشد"));
        }

        if (menuItem.IsSystem)
        {
            return Result.Failure<MenuItemResponse>(
                Error.Validation("Menu.SystemItem", "نمي توان منوي سيستمي را ويرايش كرد"));
        }

        
        if (command.ParentId.HasValue && command.ParentId.Value == command.Id)
        {
            return Result.Failure<MenuItemResponse>(
                Error.Validation("Menu.CircularReference", "منو نمي تواند والد خودش باشد"));
        }

        
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

            
            bool isDescendant = await IsDescendantAsync(menuItems, command.ParentId.Value, command.Id, cancellationToken);
            if (isDescendant)
            {
                return Result.Failure<MenuItemResponse>(
                    Error.Validation("Menu.CircularReference", "منوي والد نمي تواند از زيرمنوهاي اين منو باشد"));
            }
        }

        menuItem.Label = command.Label.Trim();
        menuItem.Path = command.Path?.Trim();
        menuItem.Icon = command.Icon?.Trim();
        menuItem.ParentId = command.ParentId;
        menuItem.DisplayOrder = command.DisplayOrder;
        menuItem.IsActive = command.IsActive;

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

    private static async Task<bool> IsDescendantAsync(
        DbSet<MenuItem> menuItems,
        int potentialParentId,
        int itemId,
        CancellationToken cancellationToken)
    {
        MenuItem? current = await menuItems
            .FirstOrDefaultAsync(m => m.Id == potentialParentId, cancellationToken)
            .ConfigureAwait(false);

        while (current != null)
        {
            if (current.Id == itemId)
            {
                return true;
            }

            if (!current.ParentId.HasValue)
            {
                break;
            }

            current = await menuItems
                .FirstOrDefaultAsync(m => m.Id == current.ParentId.Value, cancellationToken)
                .ConfigureAwait(false);
        }

        return false;
    }
}

