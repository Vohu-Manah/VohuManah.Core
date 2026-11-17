using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publishers.GetNames;

public sealed record GetPublisherNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


