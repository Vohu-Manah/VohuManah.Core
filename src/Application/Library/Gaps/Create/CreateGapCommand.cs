using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.Create;

public sealed record CreateGapCommand(
    string Title,
    string Note,
    bool State) : ICommand<int>;

