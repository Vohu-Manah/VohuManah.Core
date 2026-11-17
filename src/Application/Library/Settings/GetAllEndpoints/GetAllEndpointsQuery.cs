using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAllEndpoints;

public sealed record GetAllEndpointsQuery : IQuery<Dictionary<string, List<string>>>;


