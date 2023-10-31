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
    public class CuentasController: Controller
    {
        //atributos de la clase CuentasController
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly ApplicationDbContext _context;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, RoleManager<IdentityRole> rolemanager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _rolemanager = rolemanager;
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(string returnurl = null)
        {
            //Creación de roles
            if (!await _rolemanager.RoleExistsAsync("Administrador"))
            {
                //creación de  rol de administrador
                await _rolemanager.CreateAsync(new IdentityRole("Administrador"));
            }
            if (!await _rolemanager.RoleExistsAsync("Registrado"))
            {
                //creación de  rol de administrador
                await _rolemanager.CreateAsync(new IdentityRole("Registrado"));
            }

            ViewData["ReturnUrl"] = returnurl;
            RegistroViewModel registroVM = new RegistroViewModel();
            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel regVM ,string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (regVM != null)
            {
                List<string> nombres = new List<string>();
                List<MemoryStream> streams = new List<MemoryStream>();
                foreach (var archivo in regVM.Archivo)
                {
                    nombres.Add(archivo.FileName);
                    using var stream = new MemoryStream();
                    await archivo.CopyToAsync(stream);
                    streams.Add(stream);

                }
                

                if ((regVM.NombreUniversidad == NombreUni.UAM && regVM.Email.Contains("estudiantes.uam.es")) ||
                    (regVM.NombreUniversidad == NombreUni.UFV && regVM.Email.Contains("alumnos.ufv.es"))||
                    (regVM.NombreUniversidad == NombreUni.UCM && regVM.Email.Contains("alumnos.ucm.es")) ||
                    (regVM.NombreUniversidad == NombreUni.UAX && regVM.Email.Contains("alumnos.uax.es")) ||
                    (regVM.NombreUniversidad == NombreUni.UPM && regVM.Email.Contains("alumnos.upm.es")) ||
                    (regVM.NombreUniversidad == NombreUni.CEU && regVM.Email.Contains("alumnos.ceu.es")) ||
                    (regVM.NombreUniversidad == NombreUni.UNED && regVM.Email.Contains("alumnos.uned.es")) ||
                    (regVM.NombreUniversidad == NombreUni.UE && regVM.Email.Contains("alumnos.ue.es")) ||
                    (regVM.NombreUniversidad == NombreUni.IE && regVM.Email.Contains("alumnos.ie.es")))
                {
                    var usuario = new AppUser
                    {
                        UserName = regVM.Email,
                        Email = regVM.Email,
                        Nombre = regVM.Nombre,
                        Descripcion = regVM.Descripcion,
                        NombreUniversidad = regVM.NombreUniversidad,
                        NormalizedEmail = regVM.Email,
                        Edad = regVM.Edad,
                        //Imagen = stream.ToArray(),
                        EdadMin = 18,
                        EdadMax = 35,
                        UniversidadBusqueda = regVM.NombreUniversidad,
                        Interes = regVM.Interes,
                        Sexo = regVM.Sexo,

                    };
                    
                    var rol = "";
                    if (_context.AppUser.Count() >= 1)
                    {
                        rol = "Registrado";
                    }
                    else
                    {
                        rol = "Administrador";
                    }

                    var resultado = await _userManager.CreateAsync(usuario, regVM.Password);
                    //Asignacion del usuario que se registra al rol 
                    await _userManager.AddToRoleAsync(usuario, rol);
                    
                    if (resultado.Succeeded)
                    {
                        
                        int i = 0;
                        foreach (var st in streams) {
                            
                            var nuevaImagen = new Imagen
                            {
                                Archivo = st.ToArray(),
                                Usuario = usuario,
                                Nombre = nombres[i]
                            };
                            _context.Imagen.Add(nuevaImagen);
                            i++;
                        }

                        await _context.SaveChangesAsync();


                        //implementación de confirmación de email en el registro
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                        var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new
                        {
                            userID = usuario.Id,
                            code = code
                        }, protocol: HttpContext.Request.Scheme);
                        //Enviamos el email de confirmación
                        await _emailSender.SendEmailAsync(regVM.Email, "Confirmar su cuenta - UniMatch",
                        "Por favor confirma su cuenta dando click aqui: <a href=\"" + urlRetorno + "\">enlace</a>");

                        TempData["Correcto"] = "Se ha registrado correctamente";
                        return RedirectToAction("Login", "Cuentas");

                    }
                    ValidarErrores(resultado);
                }
                else
                {
                    TempData["Error"] = "El correo no pertenece a la universidad seleccionada.";
                    return View(regVM);
                }
                


            }
            return View(regVM);

        }
        [AllowAnonymous]
        //manejador de errores
        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, erro.Description);
            }
        }

        //Metodo formulario de acceso
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccesoViewModel accesVM)
        {
            

            if (ModelState.IsValid && accesVM != null)
            {

                var resultado = await _signInManager.PasswordSignInAsync(accesVM.Email, accesVM.Password, accesVM.RememberMe, lockoutOnFailure: true);

                if (resultado.Succeeded)
                {

                    return RedirectToAction("Index", "Home");

                }
                if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
                }
                else
                {
                    //ModelState.AddModelError(String.Empty, "El Inicio de sesion es invalido");
                    TempData["Error"] = "El correo o la contraseña no son correctas";
                    return View(accesVM);
                }

            }
            TempData["Error"] = "El correo o la contraseña no son correctas";
            return View(accesVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()
        {
            //salir de la applicacion
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Cuentas");
        }

        //Metodo para olvido de contraseñas.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> OlvidoPassword(OlvidoPasswordVM opVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(opVM.Email);

                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionOlvidoPassword");
                }
                var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var urlRetorno = Url.Action("ResetPassword", "Cuentas", new { userID = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(opVM.Email, "Recuperar Contraseña - UniMatch",
                    "Por favor recupere su contraseña dando click aqui: <a href=\"" + urlRetorno + "\">enlace</a>");
                return RedirectToAction("ConfirmacionOlvidoPassword");
            }
            return View(opVM);
        }

        //Metodo para olvido de contraseñas.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionOlvidoPassword()
        {
            return View();
        }


        //Funcionalidad para recuperar contraseñas
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(String code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(RecuperaPasswordVM rpVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(rpVM.Email);

                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }
                var resultado = await _userManager.ResetPasswordAsync(usuario, rpVM.Code, rpVM.Password);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }
                ValidarErrores(resultado);
            }
            return View(rpVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionRecuperaPassword()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return View("Error");
            }
            var resultado = await _userManager.ConfirmEmailAsync(usuario, code);
            return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");
        }





        [HttpGet]

        public async Task<IActionResult> RegistroAdministrador()
        {
            List<SelectListItem> listaRoles = new List<SelectListItem>();

            foreach (var rol in _rolemanager.Roles)
            {
                if (!await _rolemanager.RoleExistsAsync(rol.Name))
                {
                    await _rolemanager.CreateAsync(new IdentityRole(rol.Name));

                }

                listaRoles.Add(new SelectListItem()
                {
                    Value = rol.Name,
                    Text = rol.Name
                });
            }
            
            RegistroViewModel registroVM = new RegistroViewModel()
            {
                ListaRoles = listaRoles
            };
            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegistroAdministrador(RegistroViewModel regVM)
        {
            
            if ( regVM != null)
            {
                 List <string> nombres = new List<string>(); 
                 List<MemoryStream> streams = new List<MemoryStream>();
                 foreach(var archivo in regVM.Archivo)
                 {
                    nombres.Add(archivo.FileName);
                    using var stream = new MemoryStream();
                    await archivo.CopyToAsync(stream);
                    streams.Add(stream);

                 }
                    
                    
                
                    var usuario = new AppUser
                    {
                        UserName = regVM.Email,
                        Email = regVM.Email,
                        Nombre = regVM.Nombre,
                        Descripcion = regVM.Descripcion,
                        NombreUniversidad = regVM.NombreUniversidad,
                        NormalizedEmail = regVM.Email,
                        Edad = regVM.Edad,
                        //Imagen = stream.ToArray(),
                        EdadMin = 18,
                        EdadMax = 35,
                        UniversidadBusqueda = regVM.NombreUniversidad,
                        Interes = regVM.Interes,
                        Sexo = regVM.Sexo,
                    };
                    var resultado = await _userManager.CreateAsync(usuario, regVM.Password);

                    if (resultado.Succeeded)
                    //Para seleccion de rol en el registro
                    {
                        List<IdentityRole> roles = new List<IdentityRole>();

                        foreach (var rol in _rolemanager.Roles)
                        {
                            roles.Add(rol);
                        }
                        foreach (var rol in roles)
                        {
                            if (regVM.RolSeleccionado != null && regVM.RolSeleccionado.Length > 0 && regVM.RolSeleccionado == rol.Name)
                            {
                                await _userManager.AddToRoleAsync(usuario, regVM.RolSeleccionado);


                            }
                        }
                    
                        int i = 0;
                        foreach (var st in streams) {
                            
                            var nuevaImagen = new Imagen
                            {
                                Archivo = st.ToArray(),
                                Usuario = usuario,
                                Nombre = nombres[i]
                            };
                            _context.Imagen.Add(nuevaImagen);
                            i++;
                        }

                        await _context.SaveChangesAsync();
                        //implementación de confirmación de email en el registro
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                        var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userID = usuario.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        await _emailSender.SendEmailAsync(regVM.Email, "Confirmar su cuenta - UniMatch",
                        "Por favor confirma su cuenta dando click aqui: <a href=\"" + urlRetorno + "\">enlace</a>");


                        return RedirectToAction("Index", "Usuarios");

                    }
                    ValidarErrores(resultado);
            }

            //Para seleccion de rol
            List<SelectListItem> listaRoles = new List<SelectListItem>();
            listaRoles.Add(new SelectListItem()
            {
                Value = "Registrado",
                Text = "Registrado"
            });
            listaRoles.Add(new SelectListItem()
            {
                Value = "Administrador",
                Text = "Administrador"
            });
            regVM.ListaRoles = listaRoles;
            return View(regVM);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Denegado(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            return View();
        }
    }
}
