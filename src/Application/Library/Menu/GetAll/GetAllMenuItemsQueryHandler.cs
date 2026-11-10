using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Authorization;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Menu.GetAll;

internal sealed class GetAllMenuItemsQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager,
    IRolePermissionService rolePermissionService) : IQueryHandler<GetAllMenuItemsQuery, List<MenuItemResponse>>
{
    public const string MenuItemsAllKey = "sdp.menu.all";

    
    private static readonly Dictionary<string, string> PathPermission = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/"] = "Library.Visits.GetStats",
        ["/books"] = "Library.Books.GetList",
        ["/books/search"] = "Library.Books.Search",
        ["/books/statistics"] = "Library.Books.GetStatistics",
        ["/publications"] = "Library.Publications.GetList",
        ["/publications/search"] = "Library.Publications.Search",
        ["/publications/statistics"] = "Library.Publications.GetStatistics",
        ["/manuscripts"] = "Library.Manuscripts.GetList",
        ["/manuscripts/search"] = "Library.Manuscripts.Search",
        ["/manuscripts/statistics"] = "Library.Manuscripts.GetStatistics",
        ["/cities"] = "Library.Cities.GetAll",
        ["/languages"] = "Library.Languages.GetAll",
        ["/subjects"] = "Library.Subjects.GetAll",
        ["/publishers"] = "Library.Publishers.GetAll",
        ["/gaps"] = "Library.Gaps.GetAll",
        ["/publication-types"] = "Library.PublicationTypes.GetAll",
        ["/users"] = "Library.Users.GetAll",
        ["/profile"] = "Library.Settings.GetUserSettings",
        ["/settings"] = "Library.Settings.GetById",
        ["/menu-settings"] = "Library.Menu.Update",
        ["/admin/roles"] = "Library.Settings.GetAllRoles",
        ["/admin/user-roles"] = "Library.Settings.GetUserRoles"
    };

    public async Task<Result<List<MenuItemResponse>>> Handle(GetAllMenuItemsQuery query, CancellationToken cancellationToken)
    {
        DbSet<MenuItem> menuItems = unitOfWork.GetDbSet<MenuItem>();

        List<MenuItem> allItems = await menuItems
            .Include(m => m.Children)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        cacheManager.Store(MenuItemsAllKey, allItems, 60);

        HashSet<string> allowed = await GetAllowedEndpointsAsync(query.RoleNames, cancellationToken);

        List<MenuItemResponse> rootItems = allItems
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => MapToResponse(m, allItems, allowed))
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        return Result.Success(rootItems);
    }

    private async Task<HashSet<string>> GetAllowedEndpointsAsync(IReadOnlyList<string>? roleNames, CancellationToken cancellationToken)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (roleNames is null || roleNames.Count == 0)
            return allowed;

        foreach (string roleName in roleNames)
        {
            List<string> endpoints = await rolePermissionService
                .GetEndpointsForRoleAsync(roleName, cancellationToken)
                .ConfigureAwait(false);

            allowed.UnionWith(endpoints);
        }

        return allowed;
    }

    private static MenuItemResponse? MapToResponse(MenuItem item, List<MenuItem> allItems, HashSet<string> allowed)
    {
        if (!item.IsActive)
            return null;

        var children = allItems
            .Where(m => m.ParentId == item.Id)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => MapToResponse(m, allItems, allowed))
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        bool hasChildren = children.Count > 0;

        
        bool isGroup = string.IsNullOrEmpty(item.Path);
        bool pathAllowed = isGroup || (PathPermission.TryGetValue(item.Path!, out string? required) && allowed.Contains(required));

        if (isGroup ? !hasChildren : !pathAllowed)
            return null;

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
            Children = hasChildren ? children : null
        };
    }
}

