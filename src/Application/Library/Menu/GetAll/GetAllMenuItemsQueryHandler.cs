using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.GetAll;

internal sealed class GetAllMenuItemsQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllMenuItemsQuery, List<MenuItemResponse>>
{
    public const string MenuItemsAllKey = "sdp.menu.all";

    public async Task<Result<List<MenuItemResponse>>> Handle(GetAllMenuItemsQuery query, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        List<MenuItem> allItems = await menuItems
            .Include(m => m.Children)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        cacheManager.Store(MenuItemsAllKey, allItems, 60);

        
        List<MenuItemResponse> rootItems = allItems
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => MapToResponse(m, allItems))
            .ToList();

        return Result.Success(rootItems);
    }

    private static MenuItemResponse MapToResponse(MenuItem item, List<MenuItem> allItems)
    {
        var children = allItems
            .Where(m => m.ParentId == item.Id)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => MapToResponse(m, allItems))
            .ToList();

        return new MenuItemResponse
        {
            Id = item.Id,
            Label = item.Label,
            Path = item.Path,
            Icon = item.Icon,
            ParentId = item.ParentId,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            IsSystem = item.IsSystem,
            Children = children.Count > 0 ? children : null
        };
    }
}

