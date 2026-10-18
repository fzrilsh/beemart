namespace BeeMart.Api.Dtos;

public record ErrorResponse(string Code, string Message, object? Details);
