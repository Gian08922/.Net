using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using UniMatch.Data;
using UniMatch.Models;

namespace UniMatch.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventosController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(User.Identity == null)
            {
                return View("Error");
            }
            var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (userDB == null)
            {
                return View("Error");
            }
            var creador = await _context.Evento.Select(e=>e.CreadorEvento).ToListAsync();
            var eventos = await _context.Evento.Where(e=>e.CreadorEvento.Id != userDB.Id).Where(e=>e.CreadorEvento.NombreUniversidad == userDB.UniversidadBusqueda).
                Where(e=> e.CreadorEvento.Sexo == userDB.Interes).ToListAsync();
          
            return View(eventos);
        }

        [HttpGet]
        public IActionResult CrearEvento() 
        {
            EventoVM eventoVM = new EventoVM();
            return View(eventoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEvento(EventoVM eventoVM)
        {
            if(eventoVM != null)
            {
                if (User.Identity == null)
                {
                    return View("Error");
                }
                var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (userDB == null)
                {
                    return View("Error");
                }
                var newEvent = new Evento {
                    CreadorEvento = userDB,
                    Name = eventoVM.Name,
                    Descripcion = eventoVM.Descripcion,
                    NumAsistentes = 1,
                    NumMaxAsistentes=eventoVM.NumMaxAsistentes,
                    Direccion = eventoVM.Direccion,
                    FechaEvento = eventoVM.FechaEvento,
                };
                _context.Evento.Add(newEvent);

                var asistEvent = new AsistentesEventos
                {
                    Evento = newEvent,
                    Asistente = userDB,
                };
                _context.Asistentes.Add(asistEvent);
                await _context.SaveChangesAsync();
                TempData["Correcto"] = "Has Creado el evento correctamente";
                return RedirectToAction("Index","Eventos");
            }
            return View(eventoVM);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AsistirEvento(Guid eventId)
        {
            if (User.Identity == null)
            {

                return NotFound();
            }
            var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (userDB == null)
            {
                return NotFound();
            }
            var creador =  _context.Evento.Select(e => e.CreadorEvento).ToList();
            var evento =_context.Evento.Find(eventId);
            if(evento == null) 
            {
                return NotFound();
            }

            var asist =  _context.Asistentes.Select(e => e.Asistente).ToList();
            var ev = _context.Asistentes.Select(e => e.Evento).ToList();
            var yaAsisto = _context.Asistentes.ToList();
            foreach(var a in yaAsisto)
            {
                if(a.Evento.Id == evento.Id  && a.Asistente.Id == userDB.Id )
                {
                    TempData["Error"] = "Ya estás asistiendo al evento.";
                    return RedirectToAction("Index", "Eventos");
                }
            }
            if( evento.NumAsistentes == evento.NumMaxAsistentes )
            {
                TempData["Error"] = "No puedes asistir al evento. El evento está lleno.";
                return RedirectToAction("Index", "Eventos");
            }
            evento.NumAsistentes++;

            var asistentes = new AsistentesEventos { Evento = evento,Asistente =userDB };
            _context.Asistentes.Add(asistentes);
            _context.SaveChanges();
            TempData["Correcto"] = "Has marcado asistir al evento correctamente";
            return  RedirectToAction("Index", "Eventos");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DejarAsistirEvento(Guid eventId)
        {
            if (User.Identity == null)
            {

                return NotFound();
            }
            var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (userDB == null)
            {
                return NotFound();
            }
            var creador = _context.Evento.Select(e => e.CreadorEvento).ToList();
            var evento =  _context.Evento.Find(eventId);
            if (evento == null)
            {
                return NotFound();
            }


            var asist = _context.Asistentes.Select(e => e.Asistente).ToList();
            var ev = _context.Asistentes.Select(e => e.Evento).ToList();
            var dejoAsistir =  _context.Asistentes.ToList();
            if(dejoAsistir.Where(a=>a.Evento.Id == evento.Id).Where(a=>a.Asistente.Id == userDB.Id).FirstOrDefault() == null)
            {
                TempData["Error"] = "Ya habias dejado de asistir o no asistías al evento.";
                return RedirectToAction("Index", "Eventos");
            }
            evento.NumAsistentes--;

            var eliminarAsistente = _context.Asistentes.Where(a => a.Evento == evento).Where(a=>a.Asistente == userDB).FirstOrDefault();
            if(eliminarAsistente == null) { return View("Error"); }
            _context.Asistentes.Remove(eliminarAsistente);
            _context.SaveChanges();

            TempData["Correcto"] = "Has dejado de asistir al evento correctamente";
            return RedirectToAction("Index", "Eventos");
        }

        public async Task<IActionResult> MisEventos()
        {
            if (User.Identity == null)
            {

                return NotFound();
            }
            var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (userDB == null)
            {
                return NotFound();
            }
            var eventos = await _context.Evento.Where(e=>e.CreadorEvento.Id == userDB.Id).ToListAsync();
            return View(eventos);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrar(Guid eventId)
        {
            
            var eventoBorrar = _context.Evento.Find(eventId);
            if(eventoBorrar == null)
            {
                TempData["Error"] = "No se ha borrado el evento correctamente";
                return NotFound();
            }
            _context.Evento.Remove(eventoBorrar);
            var asistentes = _context.Asistentes.Where(a=>a.Evento.Id == eventoBorrar.Id).ToList();
            if(asistentes == null)
            {
                TempData["Error"] = "No se han borrado los asistentes correctamente";
                return NotFound();
            }
            foreach( var a in asistentes)
            {
                _context.Asistentes.Remove(a);
            }

            TempData["Correcto"] = "Se ha borrado el evento y sus asistentes correctamente";
            _context.SaveChanges();

            return RedirectToAction("MisEventos", "Eventos"); 
        }


        [HttpGet]
        public IActionResult EditarEvento(Guid eventId)
        {
            
            var evento = _context.Evento.Find(eventId);
            if(evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEvento(Evento evento)
        {
            
            if (evento != null)
            {
                var eventoEditar = await _context.Evento.FindAsync(evento.Id);
                if (eventoEditar == null)
                {
                    return View(evento);
                }
                eventoEditar.Name = evento.Name;
                eventoEditar.NumMaxAsistentes = evento.NumMaxAsistentes;
                eventoEditar.Direccion = evento.Direccion;
                eventoEditar.FechaEvento = evento.FechaEvento;
                eventoEditar.Descripcion = evento.Descripcion;
                await _context.SaveChangesAsync();
                TempData["Correcto"] = "Has editado el evento correctamente";
                return RedirectToAction("MisEventos", "Eventos"); ;
            }
            return View(evento);
        }
    }
}
