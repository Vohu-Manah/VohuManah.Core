using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.Delete;

internal sealed class DeleteMenuItemCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteMenuItemCommand>
{
    public async Task<Result> Handle(DeleteMenuItemCommand command, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

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
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cacheManager.Remove(GetAllMenuItemsQueryHandler.MenuItemsAllKey);

        return Result.Success();
    }
}

