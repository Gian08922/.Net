using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace UniMatch.Models
{
    public class OlvidoPasswordVM
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
