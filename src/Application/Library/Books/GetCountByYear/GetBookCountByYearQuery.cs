using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Books.GetCountByYear;

public sealed record GetBookCountByYearQuery : IQuery<List<ListItemResponse>>;

