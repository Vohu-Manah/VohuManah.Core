using Application.Abstractions.Messaging;

namespace Application.Library.Settings.Update;

public sealed record UpdateSettingsCommand(
    string Name,
    string Value) : ICommand;

