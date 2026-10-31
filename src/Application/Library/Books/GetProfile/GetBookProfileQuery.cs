using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetProfile;

public sealed record GetBookProfileQuery(long Id) : IQuery<BookProfileResponse>;

