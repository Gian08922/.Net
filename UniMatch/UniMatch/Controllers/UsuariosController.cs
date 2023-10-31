using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMatch.Data;
using UniMatch.Models;

namespace UniMatch.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public UsuariosController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var usuarios = await _context.AppUser.ToListAsync();
            var rolesUsuario = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();

            foreach (var user in usuarios)
            {
                var rol = rolesUsuario.FirstOrDefault(u=>u.UserId == user.Id);
                if(rol == null)
                {
                    user.Rol = "Ninguno";
                }else
                {
                    user.Rol = roles.FirstOrDefault(u => u.Id == rol.RoleId).Name;
                }
            }
            return View(usuarios);
        }

        //Editar USUARIO (ASIGNACION DE ROL)

        [HttpGet]
        public  IActionResult Editar(string id)
        {
            var userBD = _context.AppUser.FirstOrDefault(u=>u.Id == id);
            if(userBD == null)
            {
                return NotFound();
            }
            var roles = _context.Roles.ToList();
            var rolUsuario = _context.UserRoles.ToList();
            var rol = rolUsuario.FirstOrDefault(u => u.UserId == userBD.Id);
            if (rol != null)
            {
                userBD.IdRol = roles.FirstOrDefault(u => u.Id == rol.RoleId).Id;
            }
            userBD.ListaRoles = _context.Roles.Select(u=> new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id,
            });
            return View(userBD);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(AppUser usuario)
        {

            if (ModelState.IsValid)
            {
                var userBD = _context.AppUser.FirstOrDefault(u => u.Id == usuario.Id);
                if (userBD == null)
                {
                    return NotFound();
                }

                var rolUsuario = _context.UserRoles.FirstOrDefault(u=> u.UserId == userBD.Id);
                if (rolUsuario != null)
                {
                    //obtener rol actual
                    var rolActual = _context.Roles.Where(u => u.Id == rolUsuario.RoleId).Select(e => e.Name).FirstOrDefault();
                    //Eliminar Rol actual
                    await _userManager.RemoveFromRoleAsync(userBD, rolActual);
                }
                //Agregar usuario a nuevo rol seleccionado
                await _userManager.AddToRoleAsync(userBD,_context.Roles.FirstOrDefault(u=> u.Id == usuario.IdRol).Name);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            usuario.ListaRoles = _context.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id,
            });
            return View(usuario);
        }

        //Metodo Bloquear/ desbloquear Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BloquearDesbloquear(string idUser)
        {
            var userBd = _context.AppUser.FirstOrDefault(u=>u.Id == idUser);
            if(userBd == null)
            {
                TempData["Error"] = "No se ha podido bloquear al usuarrio correctamente";
                return NotFound();
            }
            if(userBd.LockoutEnd!= null && userBd.LockoutEnd > DateTime.Now)
            {
                //El usuario se encuentra bloqueado y lo podemos desbloquear
                userBd.LockoutEnd = DateTime.Now;
                TempData["Correcto"] = "El usuario se ha desbloqueado correctamente";

            }
            else
            {
                // el usuario no esta bloqueado y lo podemos bloquear
                userBd.LockoutEnd = DateTime.Now.AddDays(10);
                TempData["Correcto"] = "El usuario se ha bloqueado correctamente";
            }

            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        //Metodo Borrar Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrar(string idUser)
        {
            var userBd = _context.AppUser.FirstOrDefault(u => u.Id == idUser);
            if (userBd == null)
            {
                TempData["Error"] = "No se ha podido bloquear al usuarrio correctamente";
                return NotFound();
            }
            _context.AppUser.Remove(userBd);
            TempData["Correcto"] = "Se ha borrado el usario correctamente";
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //Editar Perfil
        [HttpGet]
        public IActionResult EditarPerfil(string Id)
        {
            if(Id == null)
            {
                return NotFound();
            }
            var usuario = _context.AppUser.Find(Id);
            if(usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(AppUser appUser)
        {
            List <string> nombres = new List<string>(); 
            List<MemoryStream> streams = new List<MemoryStream>();
            foreach (var archivo in appUser.Archivo)
            {
                nombres.Add(archivo.FileName);
                Console.WriteLine(nombres);
                using var stream = new MemoryStream();
                await archivo.CopyToAsync(stream);
                streams.Add(stream);
                

            }
            if (appUser != null)
            {
                var usuario = await _context.AppUser.FindAsync(appUser.Id);

                if(usuario == null)
                {
                    return View(appUser);
                }
                

                usuario.Nombre = appUser.Nombre;
                usuario.Descripcion =appUser.Descripcion;
                usuario.Edad = appUser.Edad;
                usuario.EdadMin = appUser.EdadMin;
                usuario.EdadMax=appUser.EdadMax;
                usuario.UniversidadBusqueda = appUser.UniversidadBusqueda;
                usuario.Interes = appUser.Interes;
                usuario.Sexo = appUser.Sexo;
                await _userManager.UpdateAsync(usuario);
                //usuario.Imagen = stream.ToArray();
                var imagenes =   _context.Imagen.Include(u=>u.Usuario).Where(u => u.Usuario.Id == usuario.Id);
                await imagenes.ToListAsync();
                List<string> nomImg = new List<string>();
                foreach(var imagen in imagenes)
                {
                    nomImg.Add(imagen.Nombre);
                }
                int i = 0;
                foreach (var st in streams)
                {
                    if(nomImg.Contains(nombres[i]) == false)
                    {
                        var nuevaImagen = new Imagen
                        {
                            Archivo = st.ToArray(),
                            Usuario = usuario,
                            Nombre = nombres[i]
                        };
                        _context.Imagen.Add(nuevaImagen);
                    }
                    
                    i++;
                }

                await _context.SaveChangesAsync();
                TempData["Correcto"] = "Has editado tu perfil correctamente";
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(appUser);
        }


        //cambiar Password
        [HttpGet]
        public IActionResult CambiarPassword()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordVM cpVM,string email)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(email);   
                if(usuario == null)
                {
                    return RedirectToAction("Error");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var resultado = await _userManager.ResetPasswordAsync(usuario,token,cpVM.Password);
                if (resultado.Succeeded)
                {
                    TempData["Correcto"] = "La contraseña se ha cambiado correctamente";
                    return RedirectToAction(nameof(Index), "Prestamos"); ;
                }
                TempData["Error"] = "No se ha podido cambiar la contraseña";
                return View(cpVM);
            }
            TempData["Error"] = "No se ha podido cambiar la contraseña";
            return View(cpVM);

        }

    }
}
