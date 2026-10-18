namespace BeeMart.Api.Dtos;

public record RegisterRequest(string Username, string FullName, string Password, string Role);

public record RegisterResponse(string IdUser, string Username);

public record LoginRequest(string Username, string Password, string Role);

public record LoginResponse(string Token, LoginUser User);

public record LoginUser(string IdUser, string Username, string FullName, string Role);
