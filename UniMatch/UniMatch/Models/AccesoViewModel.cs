using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UniMatch.Models
{
    public class AccesoViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contaseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Display(Name = "Recordar")]
        public bool RememberMe { get; set; }
    }
}
