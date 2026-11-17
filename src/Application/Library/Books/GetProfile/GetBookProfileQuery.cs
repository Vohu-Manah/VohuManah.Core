using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Books.GetProfile;

public sealed record GetBookProfileQuery(long Id) : IQuery<BookProfileResponse>;

