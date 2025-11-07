using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetList;

public sealed record GetManuscriptListQuery : IQuery<List<ManuscriptListResponse>>;

