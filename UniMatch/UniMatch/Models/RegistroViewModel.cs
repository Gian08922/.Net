using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;



namespace UniMatch.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "subir el archivo es obligatorio")]
        [Display(Name = "Subir archivo")]
        public List<IFormFile> Archivo { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El {0} tiene que ser mayor de {2} caracteres", MinimumLength = 2)]
        public string Nombre { get; set; }

        
        [StringLength(300, ErrorMessage = "El {0} tiene que ser mayor de 2 caracteres", MinimumLength = 2)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El nombre de la universidad es obligatorio")]
        [Display(Name = "Nombre Universidad")]
        public NombreUni NombreUniversidad { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        [RegularExpression(@"[a-zA-Z0-9_.+-]+(@alumnos.ufv.es|@estudiantes.uam.es|@alumnos.ucm.es|@alumnos.upm.es|@alumnos.uax.es|@alumnos.ceu.es|@alumnos.uned.es|@alumnos.ue.es|@alumnos.ie.es)+$", ErrorMessage = "Formato incorrecto")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contaseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Debes introducir una contraseña de al menos 8 caracteres, que contienen Mayusculas, Minúsculas, Dígitos y caracteres especiales(*+!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contaseña es obligatoria")]
        [Compare("Password", ErrorMessage = "La contraseña y confirmación de contraseña no coincide")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(18,120)]
        public int Edad { get; set; }

        [Display(Name = "¿Que perfiles quieres conocer?")]
        public Sexo? Interes { get; set; }

        [Display(Name = "¿Con que te identificas?")]
        public Sexo? Sexo { get; set; }
        //Para seleccion de roles
        [Display(Name = "Seleccionar Rol")]
        public IEnumerable<SelectListItem> ListaRoles { get; set; }
        [Display(Name = "Seleccionar Rol")]
        public string RolSeleccionado { get; set; }
    }
}
