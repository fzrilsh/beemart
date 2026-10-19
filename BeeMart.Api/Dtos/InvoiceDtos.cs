namespace BeeMart.Api.Dtos;

public record InvoiceLockResponse(string InvoiceNo, DateTime LockDate, DateTime ExpiresAt);
