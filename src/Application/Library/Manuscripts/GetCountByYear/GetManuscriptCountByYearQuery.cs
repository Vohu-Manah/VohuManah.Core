using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Manuscripts.GetCountByYear;

public sealed record GetManuscriptCountByYearQuery : IQuery<List<ListItemResponse>>;

