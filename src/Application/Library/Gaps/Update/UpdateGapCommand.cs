using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.Update;

public sealed record UpdateGapCommand(
    int Id,
    string Title,
    string Note,
    bool State) : ICommand;

