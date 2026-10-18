namespace BeeMart.Api.Dtos;

public record HistoryItemDto(string InvoiceNo, DateTime TransactionDate, string CashierName, int ItemCount, string PaymentMethod, decimal Total);

public record HistoryResponse(List<HistoryItemDto> Items, int Page, int PageSize, int TotalItems, int TotalPages);
