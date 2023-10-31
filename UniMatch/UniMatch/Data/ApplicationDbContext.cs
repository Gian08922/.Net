using UniMatch.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace UniMatch.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
             
        }
        //Agregamos los modelos que vamos a crear en la base de datos.
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Imagen> Imagen { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<AsistentesEventos> Asistentes { get; set;}
        public DbSet<ChatSala> ChatSala { get; set; }
        public DbSet<ChatUsuario> ChatUser { get; set; }
        public DbSet<Mensaje> Mensaje { get; set; }

    }
}
