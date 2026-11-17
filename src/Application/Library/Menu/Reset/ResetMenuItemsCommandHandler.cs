using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Application.Library.Menu.Shared;
using VohuManah.Domain.Menu;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Menu.Reset;

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