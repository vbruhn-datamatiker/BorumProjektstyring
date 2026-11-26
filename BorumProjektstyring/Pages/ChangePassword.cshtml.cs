using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Security;
using ClassLib.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorumProjektstyring.Pages;

public class ChangePasswordModel : PageModel
{
    private readonly IUserAuthRepo _userAuthRepo;
    private readonly UserService _userService;

    public ChangePasswordModel(
        IUserAuthRepo userAuthRepo,
        UserService userService)
    {
        _userAuthRepo = userAuthRepo;
        _userService = userService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Indtast en ny adgangskode.")]
    [StringLength(
        PasswordPolicy.MaximumLength,
        MinimumLength = PasswordPolicy.MinimumLength,
        ErrorMessage = "Adgangskoden skal være mellem 15 og 128 tegn.")]
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

        if (!_userService.ChangePassword(
                userAuth.Username,
                NewPassword,
                out var passwordError))
        {
            ModelState.AddModelError(string.Empty, passwordError);
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
