namespace UniMatch.Models
{
    public class Mensaje
    {
        public Guid Id { get; set; }
        public AppUser Usuario { get; set; }
        public ChatSala Chat { get; set; }
        public string Texto { get; set; }
        public string Fecha { get; set; }

    }
}
