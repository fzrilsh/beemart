using BeeMart.Api.Data.Entities;
using BeeMart.Api.Dtos;
using BeeMart.Api.Repositories;
using BeeMart.Api.Security;
using Microsoft.AspNetCore.Identity;

namespace BeeMart.Api.Services;

public class AuthService
{
    private readonly UserRepository _users;
    private readonly JwtTokenService _tokens;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(UserRepository users, JwtTokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var username = (request.Username ?? string.Empty).Trim();
        var fullName = (request.FullName ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Semua field wajib diisi.");
        }

        if (request.Role != "Admin" && request.Role != "Cashier")
        {
            throw new ValidationException("Role tidak valid.");
        }

        if (await _users.UsernameExistsAsync(username, cancellationToken))
        {
            throw new ConflictException("USERNAME_EXISTS", "Username already exists");
        }

        string idUser;
        do
        {
            idUser = Random.Shared.Next(10000, 100000).ToString();
        } while (await _users.IdUserExistsAsync(idUser, cancellationToken));

        var user = new User
        {
            IdUser = idUser,
            Username = username,
            FullName = fullName,
            Role = request.Role,
            Stsrc = true,
            Datein = DateTime.Now,
            Userin = "SYSTEM"
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        await _users.AddAsync(user, cancellationToken);

        return new RegisterResponse(user.IdUser, user.Username);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var username = (request.Username ?? string.Empty).Trim();
        var user = await _users.FindByUsernameAsync(username, cancellationToken);

        if (user is null || user.Role != request.Role)
        {
            throw new UnauthorizedException("INVALID_LOGIN", "Invalid login");
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password ?? string.Empty);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("INVALID_LOGIN", "Invalid login");
        }

        var token = _tokens.CreateToken(user);
        return new LoginResponse(token, new LoginUser(user.IdUser, user.Username, user.FullName, user.Role));
    }
}
