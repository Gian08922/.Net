using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
namespace UniMatch.Models
{
    public class AppUser: IdentityUser
    {
        
        public string ? Nombre { get; set; }
        public NombreUni ?NombreUniversidad { get; set; }
        public string ? Descripcion { get; set; }
        public int ? Edad { get; set; }
        
        //public byte[] ? Imagen { get; set; }
        [Display(Name ="Edad Mínima")]
        public int ? EdadMin { get; set; }
        [Display(Name = "Edad Máxima")]
        public int ? EdadMax { get; set; }
        [Display(Name = "Universidad Búsqueda")]
        public NombreUni ? UniversidadBusqueda { get; set; }

        [Display(Name = "¿Que perfiles quieres conocer?")]
        public Sexo? Interes { get; set; }
        [Display(Name = "¿Con que te identificas?")]
        public Sexo? Sexo { get; set; }



        //Nuevos prpiedades para usar roles y asignación de un rol a un usuario (Estos no se guardan en la tabla de usuarios)
        [NotMapped]
        [Display(Name = "Rol para el usuario")]
        public string IdRol { get; set; }
        [NotMapped]
        public string Rol { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> ListaRoles { get; set; }
        [NotMapped, Display(Name = "Subir archivo")]
        public List<IFormFile> Archivo { get; set; }
    }

    public enum NombreUni
    {
        UAM,
        UFV,
        UCM,
        UAX,
        UPM,
        CEU,
        UNED,
        UE,
        IE,

    }
    public enum Sexo
    {
        Hombres,
        Mujeres,
        Otro,
    }
    
}
