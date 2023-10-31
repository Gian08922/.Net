using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UniMatch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniMatch.Data;

namespace UniMatch.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userDB = _context.AppUser.FirstOrDefault(u=>u.UserName == User.Identity.Name);
            
            if(userDB == null)
            {
                return View("Error");
            }
            var chat = await _context.ChatUser.Select(c=>c.Chat).ToListAsync();
            var ChatUser1 = await _context.ChatUser.Where(c => c.Usuario1.Id == userDB.Id).ToListAsync();
            var ChatUser2 = await _context.ChatUser.Where(c => c.Usuario2.Id == userDB.Id).ToListAsync();
            ViewBag.ChatUser = ChatUser1.Union(ChatUser2).ToList();
            Random random = new Random();
            int toSkip = random.Next(1, _context.Imagen.Count());
            var usersImg = _context.Imagen.Include(u => u.Usuario).Where(u => u.Usuario.Id != userDB.Id).OrderBy(u => Guid.NewGuid()).Skip(toSkip).Take(3000);

            
            
            foreach (var user in usersImg)
            {
                if (userDB.UniversidadBusqueda == user.Usuario.NombreUniversidad && (userDB.EdadMin <= user.Usuario.Edad && userDB.EdadMax >= user.Usuario.Edad) 
                    &&(userDB.Interes == user.Usuario.Sexo))
                {
                    var userMatch = _context.Match.Where(u => u.User1.Id == userDB.Id).Where(u => u.User2.Id == user.Usuario.Id).FirstOrDefault();
                    var userMatch2 = _context.Match.Where(u => u.User2.Id == userDB.Id).Where(u => u.User1.Id == user.Usuario.Id).FirstOrDefault();
                    if (userMatch == null && userMatch2 == null)
                    {
                        var rolUser = _context.UserRoles.Where(u => u.UserId == user.Usuario.Id).FirstOrDefault();

                        if (rolUser == null)
                        {
                            return View("Error");
                        }
                        var rol = _context.Roles.Where(r => r.Id == rolUser.RoleId).FirstOrDefault();
                        if (rol == null)
                        {
                            return View("Error");
                        }
                        if (rol.Name.Equals("Administrador") == false)
                        {
                            var imagenes = _context.Imagen.Include(u => u.Usuario).Where(u => u.Usuario.Id == user.Usuario.Id);
                            return View(await imagenes.ToListAsync());
                        }
                        
                    }
                    else
                    {
                        if(userMatch2 == null)
                        {
                            if (userMatch.IsMatch == false && userMatch.Like ==false )
                            {
                                var rolUser = _context.UserRoles.Where(u => u.UserId == user.Usuario.Id).FirstOrDefault();

                                if (rolUser == null)
                                {
                                    return View("Error");
                                }
                                var rol = _context.Roles.Where(r => r.Id == rolUser.RoleId).FirstOrDefault();
                                if (rol == null)
                                {
                                    return View("Error");
                                }
                                if (rol.Name.Equals("Administrador") == false)
                                {
                                    var imagenes = _context.Imagen.Include(u => u.Usuario).Where(u => u.Usuario.Id == user.Usuario.Id);
                                    return View(await imagenes.ToListAsync());
                                }
                            }
                        }
                        else
                        {
                            if (userMatch2.IsMatch == false && userMatch2.Like == false)
                            {
                                var rolUser = _context.UserRoles.Where(u => u.UserId == user.Usuario.Id).FirstOrDefault();

                                if (rolUser == null)
                                {
                                    return View("Error");
                                }
                                var rol = _context.Roles.Where(r => r.Id == rolUser.RoleId).FirstOrDefault();
                                if (rol == null)
                                {
                                    return View("Error");
                                }
                                if (rol.Name.Equals("Administrador") == false)
                                {
                                    var imagenes = _context.Imagen.Include(u => u.Usuario).Where(u => u.Usuario.Id == user.Usuario.Id);
                                    return View(await imagenes.ToListAsync());
                                }
                            }
                        }
                    }                    
                }
            }
            var imgNull = _context.Imagen.Where(u => u.Usuario.Id == "");
            return View(await imgNull.ToListAsync());
        }
        public ActionResult ConvertirImagen(Guid codigo)
        {
           var img = _context.Imagen.FirstOrDefault(im=>im.Id == codigo);
            if(img == null)
            {
                return View("Error");
            }
            return (img.Nombre.Contains("jpg")) ? File(img.Archivo, "Imagenes/jpg"): (img.Nombre.Contains("png")) ? File(img.Archivo, "Imagenes/png"):
                (img.Nombre.Contains("webp")) ? File(img.Archivo, "Imagenes/webp") : File(img.Archivo, "Imagenes/jfif");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Like(string Id)
        {
            var user1 = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var user2 = _context.AppUser.FirstOrDefault(u => u.Id == Id);
            if (user1 == null || user2 == null) {
                return View("Error");
            }
            var match =_context.Match.Where(u=>u.User1.Id == user1.Id).Where(u=>u.User2.Id == user2.Id).FirstOrDefault();
            var match2 = _context.Match.Where(u => u.User1.Id == user2.Id).Where(u => u.User2.Id == user1.Id).FirstOrDefault();
            if (match == null && match2 == null)
            {
                var nMatch = new Match
                {
                    User1 = user1,
                    User2 = user2,
                    Like= true,
                    IsMatch=false
                };
                _context.Match.Add(nMatch);               
            }
            else
            {
                if(match == null)
                {
                    if(match2.Like == true)
                    {
                        match2.IsMatch = true;
                    }
                    else
                    {
                        match2.Like = true;
                    }
                    if ( match2.IsMatch == true)
                    {
                        var fecha = DateTime.Now;
                        var nChat = new ChatSala
                        {
                            Fecha = fecha.ToString(),
                            Nombre = user1.Nombre + " " + user2.Nombre,
                        };

                        if (nChat == null)
                        {
                            TempData["Error"] = "Error al crear el chat";
                            return NotFound();
                        }
                        else
                        {
                            _context.ChatSala.Add(nChat);
                            var chatUser = new ChatUsuario
                            {
                                Usuario1 = user1,
                                Usuario2 = user2,
                                Chat = nChat,
                            };
                            if (chatUser == null)
                            {
                                TempData["Error"] = "Error al crear el chatUsuario";
                                return NotFound();
                            }
                            _context.ChatUser.Add(chatUser);
                        }
                    }
                }
                else
                {
                    if (match.Like == true)
                    {
                        match.IsMatch = true;
                    }
                    else
                    {
                        match.Like = true;
                    }
                    if (match.IsMatch == true)
                    {
                        var fecha = DateTime.Now;
                        var nChat = new ChatSala
                        {
                            Fecha = fecha.ToString(),
                            Nombre = user1.Nombre + " " + user2.Nombre,
                        };

                        if (nChat == null)
                        {
                            TempData["Error"] = "Error al crear el chat";
                            return NotFound();
                        }
                        else
                        {
                            _context.ChatSala.Add(nChat);
                            var chatUser = new ChatUsuario
                            {
                                Usuario1 = user1,
                                Usuario2 = user2,
                                Chat = nChat,
                            };
                            if (chatUser == null)
                            {
                                TempData["Error"] = "Error al crear el chatUsuario";
                                return NotFound();
                            }
                            _context.ChatUser.Add(chatUser);
                        }
                    }
                }
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Dislike(string Id)
        {
            var user1 = _context.AppUser.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var user2 = _context.AppUser.FirstOrDefault(u => u.Id == Id);
            if (user1 == null || user2 == null)
            {
                return View("Error");
            }
            var match = _context.Match.Where(u => u.User1.Id == user1.Id).Where(u => u.User2.Id == user2.Id).FirstOrDefault();
            var match2 = _context.Match.Where(u => u.User1.Id == user2.Id).Where(u => u.User2.Id == user1.Id).FirstOrDefault();
            if (match == null && match2 == null)
            {
                var nMatch = new Match
                {
                    User1 = user1,
                    User2 = user2,
                    Like = false,
                    IsMatch = false
                };
                _context.Match.Add(nMatch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}