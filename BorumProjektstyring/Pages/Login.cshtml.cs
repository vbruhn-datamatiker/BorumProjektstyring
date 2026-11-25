using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLib.Model;
using ClassLib.Repo;
using ClassLib.Service;
using ClassLib.Interface;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace BorumProjektstyring.Pages
{
    public class LoginModel : PageModel
    {

        private readonly UserService _userService;

        public LoginModel(IUserAuthRepo repo)
        {
            _userService = new UserService(repo);
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }



        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostSubmit()
        {
            var userRepo = new UserAuthRepo();
            var userAuth = userRepo.GetAuthByUserName(Username);

            if (!ModelState.IsValid || !_userService.AuthenticateUser(Username, Password))
            {
                ModelState.AddModelError(string.Empty, "Ugyldigt brugernavn eller adgangskode.");
                return Page();
            }

            var role = userAuth?.Role;
            if (role != "Admin" && role != "Koordinator" && role != "Bruger")
            {
                ModelState.AddModelError(string.Empty, "Brugeren har ingen gyldig rolle.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userAuth.UserId.ToString()),
                new(ClaimTypes.Name, Username),
                new(ClaimTypes.Role, role)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return role == "Admin"
                ? RedirectToPage("/Admin")
                : RedirectToPage("/Profile");
        }
    }
}
