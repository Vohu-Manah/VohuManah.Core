using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Domain.Menu;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Menu.Delete;

internal sealed class DeleteMenuItemCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<DeleteMenuItemCommand>
{
    public async Task<Result> Handle(DeleteMenuItemCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = dbContext.MenuItems;

        MenuItem? menuItem = await menuItems
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == command.Id, cancellationToken)
            .ConfigureAwait(false);

        if (menuItem == null)
        {
            return Result.Failure(Error.NotFound("Menu.NotFound", "آیتم منو یافت نشد"));
        }

        if (menuItem.IsSystem)
        {
            return Result.Failure(Error.Validation("Menu.SystemItem", "نمي توان منوي سيستمي را حذف كرد"));
        }

        
        if (menuItem.Children.Count > 0)
        {
            return Result.Failure(Error.Validation("Menu.HasChildren", "نمي توان منويي كه داراي زيرمنو است را حذف كرد. ابتدا زيرمنوها را حذف كنيد"));
        }

        menuItems.Remove(menuItem);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success();
    }
}

