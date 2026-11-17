using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Languages.GetById;

public sealed record GetLanguageByIdQuery(int Id) : IQuery<LanguageResponse>;

