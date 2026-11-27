using System.ComponentModel.DataAnnotations;

namespace BorumProjektstyring.Pages.Inputs;

public class ProfileInput
{
    [Required(ErrorMessage = "Indtast et navn.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Indtast en e-mailadresse.")]
    [EmailAddress(ErrorMessage = "Indtast en gyldig e-mailadresse.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Indtast et telefonnummer.")]
    public string? Phone { get; set; }
}
