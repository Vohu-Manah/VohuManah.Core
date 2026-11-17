using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Domain.Menu;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Menu.Reorder;

internal sealed class ReorderMenuItemsCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<ReorderMenuItemsCommand>
{
    public async Task<Result> Handle(ReorderMenuItemsCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = dbContext.MenuItems;

        
        List<int> itemIds = command.Items.Select(i => i.Id).ToList();
        List<MenuItem> existingItems = await menuItems
            .Where(m => itemIds.Contains(m.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingItems.Count != itemIds.Count)
        {
            return Result.Failure(Error.Validation("Menu.ItemsNotFound", "برخي از آيتم هاي منو يافت نشدند"));
        }

        
        foreach (MenuItemOrder order in command.Items)
        {
            MenuItem? item = existingItems.FirstOrDefault(m => m.Id == order.Id);
            if (item != null)
            {
                item.DisplayOrder = order.DisplayOrder;
                item.ParentId = order.ParentId;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success();
    }
}

