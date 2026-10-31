using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Library.Books.GetBarcodeData;






public sealed record GetBookBarcodeQuery : IQuery<List<BookBarcodeResponse>>
{
    
    
    
    
    
    public long AfterId { get; init; }

    
    public int Take { get; init; } = 500;

    
    public string? Year { get; init; }

    
    public string? Search { get; init; }
}
