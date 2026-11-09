using Application.Abstractions.Messaging;

namespace Application.Library.Menu.GetAll;

public sealed record GetAllMenuItemsQuery(IReadOnlyList<string> RoleNames) : IQuery<List<MenuItemResponse>>;

