using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace UniMatch.Models
{
    public class CambiarPasswordVM
    {
       
        [Required(ErrorMessage = "La contaseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contaseña es obligatoria")]
        [Compare("Password", ErrorMessage = "La contraseña y confirmación de contraseña no coincide")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }


        
    }
}
