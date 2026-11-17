using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Manuscripts.GetAll;

public sealed record GetAllManuscriptsQuery() : IQuery<List<ManuscriptResponse>>;

