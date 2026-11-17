using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.PublicationTypes.GetNames;

public sealed record GetPublicationTypeNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


