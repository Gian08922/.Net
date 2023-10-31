namespace UniMatch.Models
{
    public class ChatUsuario
    {
        public Guid Id { get; set; }   
        public AppUser Usuario1 { get; set; }
        public AppUser Usuario2 { get; set; }
        public ChatSala Chat { get; set; }
    }
}
