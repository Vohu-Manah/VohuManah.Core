using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;

namespace VohuManah.Application.Library.Users.GetAllEntities;

public sealed record GetAllUserEntitiesQuery : IQuery<List<User>>;

