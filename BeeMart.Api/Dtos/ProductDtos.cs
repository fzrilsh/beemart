namespace BeeMart.Api.Dtos;

public record ProductDto(string IdProduct, string Name, decimal WeightGram, int Stock, decimal Price, decimal Discount);

public record PaymentMethodDto(int IdPaymentMethod, string MethodName);
