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

        public LoginModel(IUserRepo repo)
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

            var userRepo = new UserRepo();
            var userAuth = userRepo.GetAuthByUserName(Username);
            if (ModelState.IsValid)
            {
                if (_userService.AuthenticateUser(Username, Password))
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
                    if (userAuth?.Role == "Admin")
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                        return RedirectToPage("/Admin");
                    }
                    else if (userAuth?.Role == "Koordinator")
                    {
                        return RedirectToPage("/Profile");
                    }
                    else
                    {
                        // Handle other roles or redirect to a default page
                       ModelState.AddModelError(string.Empty, "Ugyldigt brugernavn eller adgangskode.");
                    }
                }
           
                // Authentication successful
                // You can set session or cookie here if needed
                return RedirectToPage("/Profile");
            }
            else
            {
                // Authentication failed
                ModelState.AddModelError(string.Empty, "Ugyldigt brugernavn eller adgangskode.");
                return Page();
            }
        }
    }
}
