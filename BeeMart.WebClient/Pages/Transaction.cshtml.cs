using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeeMart.WebClient.Pages;

public class TransactionModel : PageModel
{
    public string SessionText { get; private set; } = string.Empty;

    public IActionResult OnGet()
    {
        var token = Request.Cookies[SessionCookie.Name];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        var role = Request.Cookies[SessionCookie.RoleName] ?? "Cashier";
        var roleLabel = role == "Admin" ? "Admin" : "Kasir";
        var username = Request.Cookies[SessionCookie.UsernameName] ?? string.Empty;
        SessionText = DateTime.Now.ToString("d/M/yyyy, HH.mm.ss") + " | " + roleLabel + ": " + username;
        return Page();
    }
}
