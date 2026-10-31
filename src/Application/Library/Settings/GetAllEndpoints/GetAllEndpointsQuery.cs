using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetAllEndpoints;

public sealed record GetAllEndpointsQuery : IQuery<Dictionary<string, List<string>>>;


