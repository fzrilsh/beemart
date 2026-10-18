using BeeMart.WebClient.Models;
using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeeMart.WebClient.Pages;

public class LoginModel : PageModel
{
    private readonly ApiClient _api;

    public LoginModel(ApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Role { get; set; } = "Cashier";

    public string? ErrorMessage { get; set; }

    public string? SuccessMessage { get; set; }

    public void OnGet(bool registered = false)
    {
        if (registered)
        {
            SuccessMessage = "Registrasi berhasil. Silakan login.";
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username dan Password wajib diisi.";
            return Page();
        }

        try
        {
            var result = await _api.LoginAsync(new LoginRequest(Username.Trim(), Password, Role), cancellationToken);

            Response.Cookies.Append(SessionCookie.Name, result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
            Response.Cookies.Append(SessionCookie.RoleName, result.User.Role, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
            Response.Cookies.Append(SessionCookie.UsernameName, result.User.Username, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return RedirectToPage("/Transaction");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
