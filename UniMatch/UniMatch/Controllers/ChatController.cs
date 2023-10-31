using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UniMatch.Data;
using UniMatch.Models;

namespace UniMatch.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> SalaChat(Guid Id)
        {

            var chatUser = await _context.ChatUser.Select(cu => cu.Chat).ToListAsync();
            ViewBag.chatUser = _context.ChatUser.FirstOrDefault(c=>c.Chat.Id ==Id);
            var msn = await _context.Mensaje.Select(m => m.Chat).ToListAsync();
            ViewBag.Mensajes = _context.Mensaje.Where(m => m.Chat.Id == Id).ToList();
            var usr = await _context.AppUser.Select(u=>u.Id).ToListAsync();
            var msnUser = await _context.Mensaje.Select(m => m.Usuario).ToListAsync();
            ViewBag.UserRegistrado = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            MensajeVM mensajeVM = new MensajeVM();
            return View(mensajeVM);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(MensajeVM msnVM, Guid idChat)
        {
            if(msnVM == null)
            {
                return NotFound();
            }
            var chat = await _context.ChatSala.FirstOrDefaultAsync(c => c.Id == idChat);
            if(chat == null)
            {
                return NotFound();
            }
            if (User.Identity == null)
            {
                return View("Error");
            }
            var userDB = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (userDB == null)
            {
                return View("Error");
            }

            var mensaje = new Mensaje
            {
                Chat = chat,
                Usuario = userDB,
                Texto =msnVM.Mensaje,
                Fecha = DateTime.Now.ToString(),
            };
            _context.Mensaje.Add(mensaje);
            await _context.SaveChangesAsync();
           

            return RedirectToAction("SalaChat","Chat", new {Id = chat.Id});
        }
    }
}
