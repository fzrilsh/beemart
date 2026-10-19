namespace BeeMart.Api.Dtos;

public record CartItemRequest(string IdProduct, int Qty);

public record CalculateRequest(List<CartItemRequest> Items);

public record CartLineDto(string IdProduct, string Name, decimal Price, decimal Discount, int Qty, decimal SubTotal);

public record CalculateResponse(List<CartLineDto> Items, decimal TotalPrice);

public record SubmitTransactionRequest(string InvoiceNo, int IdPaymentMethod, List<CartItemRequest> Items);

public record SubmitTransactionResponse(string InvoiceNo, DateTime TransactionDate, decimal TotalPrice);
