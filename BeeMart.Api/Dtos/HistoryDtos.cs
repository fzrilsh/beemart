namespace BeeMart.Api.Dtos;

public record HistoryItemDto(string InvoiceNo, DateTime TransactionDate, string FullName, int ItemCount, string MethodName, decimal TotalPrice);

public record HistoryResponse(List<HistoryItemDto> Items, int Page, int PageSize, int TotalItems, int TotalPages);
