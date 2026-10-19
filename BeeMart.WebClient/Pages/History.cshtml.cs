using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeeMart.WebClient.Pages;

public class HistoryModel : PageModel
{
    public string RoleLabel { get; private set; } = string.Empty;

    public string Username { get; private set; } = string.Empty;

    public IActionResult OnGet()
    {
        var token = Request.Cookies[SessionCookie.Name];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        var role = Request.Cookies[SessionCookie.RoleName] ?? "Cashier";
        RoleLabel = role == "Admin" ? "Admin" : "Kasir";
        Username = Request.Cookies[SessionCookie.UsernameName] ?? string.Empty;
        return Page();
    }
}
