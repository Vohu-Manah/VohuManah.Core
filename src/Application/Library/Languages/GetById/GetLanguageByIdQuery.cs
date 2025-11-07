using Application.Abstractions.Messaging;

namespace Application.Library.Languages.GetById;

public sealed record GetLanguageByIdQuery(int Id) : IQuery<LanguageResponse>;

