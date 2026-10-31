namespace Application.Library.Books.GetBarcodeData;


public sealed record BookBarcodeResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string No { get; init; } = string.Empty;
}
