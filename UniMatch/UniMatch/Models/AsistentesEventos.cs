namespace UniMatch.Models
{
    public class AsistentesEventos
    {
        public Guid Id { get; set; }
        public AppUser Asistente { get; set; }
        public Evento Evento { get; set; }
    }
}
