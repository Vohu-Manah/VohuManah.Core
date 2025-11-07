using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.GetNames;

public sealed record GetPublisherNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


