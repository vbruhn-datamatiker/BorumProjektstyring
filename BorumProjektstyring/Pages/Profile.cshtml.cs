using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Repo;
using ClassLib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BorumProjektstyring.Pages
{
    public class ProfileModel : PageModel
    {

        private readonly UserService _userService;
        private readonly IRepo<User> _userRepo;
        private readonly IRepo<UserAuthentication> _userAuthRepo;

        public ProfileModel(IUserAuthRepo repo)
        {
            _userService = new UserService(repo);
            _userRepo = new UserRepo();
            _userAuthRepo = new UserAuthRepo(); 
        }

        [BindProperty]
        public ProfileInput Profile { get; set; } = new();

        [BindProperty]
        public PasswordInput Password { get; set; } = new();

        public string PasswordError { get; private set; }

        public string PasswordSuccess { get; private set; }

        public string UserSuccess { get; private set; }

        public bool PasswordDialogOpen { get; private set; }

        public int UserId { get; private set; }




        public IActionResult OnGet()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return RedirectToPage("/Login");
            }

            var user = _userRepo.GetById(userId);
            var userAuth = _userAuthRepo.GetById(userId);

            if (user is null)
            {
                return NotFound();
            }

            // Need to update database so User and UserAuth are in sync, as emails are not the same on these tables.
            // This is a temporary solution until we have a better way to handle this.
            Profile.Username = user.Name;
            Profile.Email = userAuth.Username;
            Profile.Phone = user.Phone;
            UserId = userId;

            return Page();
        }

        public IActionResult OnPostPassword()
        {
            PasswordDialogOpen = true;

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return RedirectToPage("/Login");
            }

            var userAuth = _userAuthRepo.GetById(userId);
            var username = userAuth?.Username;

            if (string.IsNullOrWhiteSpace(username))
            {
                return RedirectToPage("/Login");
            }

            ModelState.Clear();
            if (!TryValidateModel(Password, nameof(Password)))
            {
                LoadProfileValues();
                return Page();
            }

            if (!_userService.AuthenticateUser(username, Password.CurrentPassword!))
            {
                PasswordError = "Den nuværende adgangskode er forkert.";
            }
            else if (!_userService.ChangePassword(
                username,
                Password.NewPassword!,
                out var passwordError))
            {
                PasswordError = passwordError;
            }
            else
            {
               
                PasswordSuccess = "Adgangskode er ændret.";
                PasswordDialogOpen = false;
                Password = new PasswordInput();
            }

            LoadProfileValues();
            return Page();
        }

        public IActionResult OnPostUser()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return RedirectToPage("/Login");
            }
            var user = _userRepo.GetById(userId);
            if (user is null)
            {
                return NotFound();
            }

            var userAuth = _userAuthRepo.GetById(userId);
            if (userAuth is null)
            {
                return NotFound();
            }

            ModelState.Clear();
            if (!TryValidateModel(Profile, nameof(Profile)))
            {
                return Page();
            }

            user.Name = Profile.Username!;
            user.Email = Profile.Email!;
            user.Phone = Profile.Phone!;
            userAuth.Username = Profile.Email!;

            _userRepo.Update(user, userId);
            _userAuthRepo.Update(userAuth, userId);
            UserSuccess = "Dine oplysninger er opdateret.";
            LoadProfileValues();
            return Page();
        }

        private void LoadProfileValues()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return;
            }

            var user = _userRepo.GetById(userId);
            var userAuth = _userAuthRepo.GetById(userId);

            if (user is null)
            {
                return;
            }

            // Need to update database so User and UserAuth are in sync, as emails are not the same on these tables.
            // This (using userAuth.Username) is a temporary solution until we have a better way to handle this.
            Profile.Username = user.Name;
            Profile.Email = userAuth?.Username;
            Profile.Phone = user.Phone;
        }






    }
}
