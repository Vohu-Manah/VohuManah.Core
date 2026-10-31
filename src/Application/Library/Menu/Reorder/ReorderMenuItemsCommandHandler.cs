using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.Reorder;

internal sealed class ReorderMenuItemsCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<ReorderMenuItemsCommand>
{
    public async Task<Result> Handle(ReorderMenuItemsCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        
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

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success();
    }
}

