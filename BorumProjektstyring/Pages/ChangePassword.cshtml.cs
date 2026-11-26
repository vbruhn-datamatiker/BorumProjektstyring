using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ClassLib.Interface;
using ClassLib.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace BorumProjektstyring.Pages;

public class ChangePasswordModel : PageModel
{
    private readonly IUserAuthRepo _userAuthRepo;

    public ChangePasswordModel(IUserAuthRepo userAuthRepo)
    {
        _userAuthRepo = userAuthRepo;
    }

    [BindProperty]
    [Required(ErrorMessage = "Indtast en ny adgangskode.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Bekræft den nye adgangskode.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Adgangskoderne er ikke ens.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        return User.Identity?.IsAuthenticated == true
            ? Page()
            : RedirectToPage("/Login");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return RedirectToPage("/Login");
        }

        var userAuth = _userAuthRepo.GetById(userId);

        if (userAuth is null)
        {
            return RedirectToPage("/Login");
        }

        using var sha256 = SHA256.Create();
        userAuth.Password = sha256.ComputeHash(Encoding.UTF8.GetBytes(NewPassword));

        if (_userAuthRepo.UpdatePassword(userAuth, userId) is null)
        {
            ModelState.AddModelError(string.Empty, "Adgangskoden kunne ikke ændres.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userAuth.UserId.ToString()),
            new(ClaimTypes.Name, userAuth.Username),
            new(ClaimTypes.Role, userAuth.Role),
            new("MustChangePassword", "false")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return userAuth.Role == "Admin"
            ? RedirectToPage("/Admin")
            : RedirectToPage("/Profile");
    }
}
