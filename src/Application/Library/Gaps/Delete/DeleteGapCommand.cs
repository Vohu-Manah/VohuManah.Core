using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Gaps.Delete;

public sealed record DeleteGapCommand([Display(Name = "شناسه")] int Id) : ICommand;

