using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.Delete;

public sealed record DeleteGapCommand(int Id) : ICommand;

