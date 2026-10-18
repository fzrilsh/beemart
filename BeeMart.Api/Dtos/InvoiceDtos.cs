namespace BeeMart.Api.Dtos;

public record InvoiceLockResponse(string InvoiceNo, DateTime LockedAt, DateTime ExpiresAt);
