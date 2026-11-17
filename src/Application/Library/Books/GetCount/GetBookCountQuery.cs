using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Books.GetCount;

public sealed record GetBookCountQuery : IQuery<int>;

