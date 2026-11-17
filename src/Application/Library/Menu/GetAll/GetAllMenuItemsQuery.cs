using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Menu.GetAll;

public sealed record GetAllMenuItemsQuery(IReadOnlyList<string> RoleNames) : IQuery<List<MenuItemResponse>>;

