using System.ComponentModel.DataAnnotations;
using ClassLib.Security;

namespace BorumProjektstyring.Pages;

public class PasswordInput
{
    [Required(ErrorMessage = "Indtast din nuværende adgangskode.")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Indtast en ny adgangskode.")]
    [StringLength(
        PasswordPolicy.MaximumLength,
        MinimumLength = PasswordPolicy.MinimumLength,
        ErrorMessage = "Adgangskoden skal være mellem 10 og 128 tegn.")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Bekræft den nye adgangskode.")]
    [Compare(nameof(NewPassword), ErrorMessage = "De nye adgangskoder er ikke ens.")]
    public string? ConfirmNewPassword { get; set; }
}
