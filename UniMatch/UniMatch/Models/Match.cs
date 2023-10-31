namespace UniMatch.Models
{
    public class Match
    {
        public Guid Id { get; set; }
        public AppUser User1 { get; set; }
        public AppUser User2 { get; set; }
        public bool Like { get; set; }
        public bool IsMatch { get; set; }


    }
}
