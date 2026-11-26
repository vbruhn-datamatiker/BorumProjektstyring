using System.ComponentModel.DataAnnotations;

namespace BorumProjektstyring.Pages;

public class PasswordInput
{
    [Required(ErrorMessage = "Indtast din nuværende adgangskode.")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Indtast en ny adgangskode.")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Bekræft den nye adgangskode.")]
    [Compare(nameof(NewPassword), ErrorMessage = "De nye adgangskoder er ikke ens.")]
    public string? ConfirmNewPassword { get; set; }
}
