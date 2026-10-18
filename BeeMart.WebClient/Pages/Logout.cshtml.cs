using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeeMart.WebClient.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        Response.Cookies.Delete(SessionCookie.Name);
        Response.Cookies.Delete(SessionCookie.RoleName);
        Response.Cookies.Delete(SessionCookie.UsernameName);
        return RedirectToPage("/Login");
    }
}
