using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.PublicationTypes.GetNames;

public sealed record GetPublicationTypeNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


