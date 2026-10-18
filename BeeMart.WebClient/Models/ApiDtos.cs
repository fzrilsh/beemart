namespace BeeMart.WebClient.Models;

public record LoginRequest(string Username, string Password, string Role);

public record RegisterRequest(string Username, string FullName, string Password, string Role);

public record LoginUser(string IdUser, string Username, string FullName, string Role);

public record LoginResponse(string Token, LoginUser User);

public record ProductDto(string IdProduct, string Name, decimal WeightGram, int Stock, decimal Price, decimal Discount);

public record PaymentMethodDto(int IdPaymentMethod, string MethodName);

public record InvoiceLockResponse(string InvoiceNo, DateTime LockedAt, DateTime ExpiresAt);

public record CartItemRequest(string IdProduct, int Qty);

public record CartLineDto(string IdProduct, string Name, decimal Price, decimal Discount, int Qty, decimal SubTotal);

public record CalculateRequest(List<CartItemRequest> Items);

public record CalculateResponse(List<CartLineDto> Items, decimal Total);

public record SubmitTransactionRequest(string InvoiceNo, int IdPaymentMethod, List<CartItemRequest> Items);

public record SubmitTransactionResponse(string InvoiceNo, DateTime TransactionDate, decimal Total);

public record HistoryItemDto(string InvoiceNo, DateTime TransactionDate, string CashierName, int ItemCount, string PaymentMethod, decimal Total);

public record HistoryResponse(List<HistoryItemDto> Items, int Page, int PageSize, int TotalItems, int TotalPages);
