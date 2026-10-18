namespace BeeMart.WebClient.Models;

public record CalculateProxyRequest(List<CartItemRequest> Items);

public record SubmitProxyRequest(string InvoiceNo, int IdPaymentMethod, List<CartItemRequest> Items);
