using Application.Abstractions.Messaging;
using Domain.Library;

namespace Application.Library.Users.GetAllEntities;

public sealed record GetAllUserEntitiesQuery : IQuery<List<User>>;

