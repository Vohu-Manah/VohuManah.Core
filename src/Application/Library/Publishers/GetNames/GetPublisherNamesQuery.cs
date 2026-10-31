using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publishers.GetNames;

public sealed record GetPublisherNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


