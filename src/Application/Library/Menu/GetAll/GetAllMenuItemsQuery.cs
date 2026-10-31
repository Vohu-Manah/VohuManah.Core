using Application.Abstractions.Messaging;

namespace Application.Library.Menu.GetAll;

public sealed record GetAllMenuItemsQuery() : IQuery<List<MenuItemResponse>>;

