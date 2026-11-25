using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Repo;
using ClassLib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Indtast din nuværende adgangskode.")]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Indtast en ny adgangskode.")]
        public string? NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Bekræft den nye adgangskode.")]
        [Compare(nameof(NewPassword), ErrorMessage = "De nye adgangskoder er ikke ens.")]
        public string? ConfirmNewPassword { get; set; }

        public string PasswordError { get; private set; }

        public string PasswordSuccess { get; private set; }

        public bool PasswordDialogOpen { get; private set; }


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
            Username = user.Name;
            Email = userAuth.Username;
            Phone = user.Phone;

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

            if (!ModelState.IsValid)
            {
                LoadProfileValues();
                return Page();
            }

            if (!_userService.AuthenticateUser(username, CurrentPassword!))
            {
                PasswordError = "Den nuværende adgangskode er forkert.";
            }
            else if (_userService.ChangePasswordHash(username, NewPassword!) == null)
            {
                PasswordError = "Adgangskoden kunne ikke ændres.";
            }
            else
            {
               
                PasswordSuccess = "Adgangskode er ændret.";
                PasswordDialogOpen = false;
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }

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
            // This is a temporary solution until we have a better way to handle this.
            Username = user.Name;
            Email = userAuth?.Username;
            Phone = user.Phone;
        }






    }
}
