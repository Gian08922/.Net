namespace UniMatch.Models
{
    public class Evento
    {
        public Guid Id { get; set; }
        public AppUser CreadorEvento { get; set; }
        public string Name { get; set; }
        public string Descripcion { get; set; }
        public int NumMaxAsistentes { get; set; }
        public int NumAsistentes { get; set; }
        public string FechaEvento { get; set; }
        public string Direccion { get; set; }

    }
}
