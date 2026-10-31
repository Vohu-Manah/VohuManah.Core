using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Application.Library.Menu.Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.Reset;

internal sealed class ResetMenuItemsCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<ResetMenuItemsCommand>
{
    public async Task<Result> Handle(ResetMenuItemsCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        List<MenuItem> existing = await menuItems
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        menuItems.RemoveRange(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await InsertAsync(unitOfWork, DefaultMenuItems.Root, parentId: null, cancellationToken)
            .ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success();
    }

    
    
    
    
    private static async Task InsertAsync(
        IUnitOfWork unitOfWork,
        IReadOnlyList<DefaultMenuItem> templates,
        int? parentId,
        CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        foreach (DefaultMenuItem template in templates)
        {
            var menuItem = new MenuItem
            {
                Label = template.Label,
                Path = template.Path,
                Icon = template.Icon,
                ParentId = parentId,
                DisplayOrder = template.DisplayOrder,
                IsActive = true,
                IsSystem = template.IsSystem
            };

            await menuItems.AddAsync(menuItem, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            if (template.Children.Count > 0)
            {
                await InsertAsync(unitOfWork, template.Children, menuItem.Id, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}