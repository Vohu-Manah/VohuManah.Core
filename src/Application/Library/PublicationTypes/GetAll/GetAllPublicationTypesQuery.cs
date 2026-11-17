using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.PublicationTypes.GetAll;

public sealed record GetAllPublicationTypesQuery() : IQuery<List<PublicationTypeResponse>>;

