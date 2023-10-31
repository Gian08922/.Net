namespace UniMatch.Models
{
    public class Imagen
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public byte[] Archivo { get; set; }
        public AppUser Usuario { get; set; }
    }
}
