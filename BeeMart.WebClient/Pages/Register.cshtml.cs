using BeeMart.WebClient.Models;
using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeeMart.WebClient.Pages;

public class RegisterModel : PageModel
{
    private readonly ApiClient _api;

    public RegisterModel(ApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string RePassword { get; set; } = string.Empty;

    [BindProperty]
    public string Role { get; set; } = "Cashier";

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Semua field wajib diisi.";
            return Page();
        }

        if (Password != RePassword)
        {
            ErrorMessage = "Password dan Re-Password tidak sama.";
            return Page();
        }

        try
        {
            await _api.RegisterAsync(new RegisterRequest(Username.Trim(), FullName.Trim(), Password, Role), cancellationToken);
            return RedirectToPage("/Login", new { registered = 1 });
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
