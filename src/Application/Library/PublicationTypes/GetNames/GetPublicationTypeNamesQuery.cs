using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.GetNames;

public sealed record GetPublicationTypeNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


