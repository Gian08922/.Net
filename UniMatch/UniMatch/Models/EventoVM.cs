using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace UniMatch.Models
{
    public class EventoVM
    {
        [Required(ErrorMessage = "El nombre del evento es obligatorio")]
        [Display(Name = "Nombre del Evento:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción del evento es obligatoria")]
        [Display(Name = "Descripción del Evento:")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El número máximo de asistentes es obligatorio")]
        [Display(Name = "Número máximo de asistentes:")]
        public int NumMaxAsistentes { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name = "Dirección Evento:")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha Evento:")]
        public string FechaEvento { get; set; }

        

    }
}
