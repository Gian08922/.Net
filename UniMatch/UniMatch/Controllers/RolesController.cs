using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using UniMatch.Data;

namespace UniMatch.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        //atributos de la clase RolesController
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        /**
         * Constructor de la clase Rolescontroller donde se inicializan los atributos de la clase.
         * 
         * @param ApplicationDbContext para poder acceder a la tabla de la base de datos.
         * @param UserManager para poder gestionar el usuario logueado
         * @param RoleManager para poder gestionar los roles.
         * 
         * @author David Pinazo García.
         * @version 1.0
         **/
        public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> rolemanager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _rolemanager = rolemanager;
        }

        /**
        * Metodo Index que devuelve la vista Index para este controlador. Al cual le pasamos la lista de los roles
        * creados.
        * 
        * @return vista Index creada por defecto con la lista de productos como argumento.
        * 
        * @author David Pinazo García
        * @version 1.0
        **/
        [HttpGet]
        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }


        /**
         * Metodo Get para crear un Rol
         * 
         * @return vista.
         * 
         * @author David Pinazo García
         * @version 1.0
         **/
        [HttpGet]
        public IActionResult Crear()
        {

            return View();
        }

        /**
          * Metodo Post para crear un rol. En este método comprobamos si el rol que estamos creando ya existe
          * Tras ello, creamos el rol almacenandolo en la base de datos.
          * 
          * @param IdentityRole con el rol creado en el formulario
          * @return redireccción al index.
          * 
          * 
          * @author David Pinazo García
          * @version 1.0
          **/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(IdentityRole rol)
        {
            if (await _rolemanager.RoleExistsAsync(rol.Name))
            {
                TempData["Error"] = "El rol ya existe";
                return RedirectToAction(nameof(Index));
            }
            //se crea el rol
            await _rolemanager.CreateAsync(new IdentityRole()
            {
                Name = rol.Name
            });
            TempData["Correcto"] = "El rol se ha creado correctamente";
            return RedirectToAction(nameof(Index));
        }


        /**
         * Metodo Get para Editar un Rol. En el cual obtenemos el producto a través del id y devolvemos la 
         * vista con el producto buscado.
         * 
         * @param string con el id del rol.
         * @return vista con el rol obtenido de la base de datos.
         * 
         * 
         * @author David Pinazo García
         * @version 1.0
         **/

        [HttpGet]
        public IActionResult Editar(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                //Actualizar el rol
                var rolBd = _context.Roles.FirstOrDefault(x => x.Id == id);
                return View(rolBd);
            }
        }
        /**
         * Metodo Post para editar un rol. En este método obtenemos el rol seleccionado para editar, y
         * comprobamos si ya existe el rol y si no es así cambiamos el nombre. Actualizamos y
         * guardamos los cambios.
         * 
         * @param IdentityRole rol obtenido del formulario
         * @return redireccción al index.
         * 
         * 
         * @author David Pinazo García
         * @version 1.0
         **/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(IdentityRole rol)
        {
            if (await _rolemanager.RoleExistsAsync(rol.Name))
            {
                TempData["Error"] = "El rol ya existe";
                return RedirectToAction(nameof(Index));
            }
            //se crea el rol
            var rolBd = _context.Roles.FirstOrDefault(x => x.Id == rol.Id);
            if (rolBd == null)
            {
                return RedirectToAction(nameof(Index));
            }
            rolBd.Name = rol.Name;
            rolBd.NormalizedName = rol.Name.ToUpper();

            await _rolemanager.UpdateAsync(rolBd);
            TempData["Correcto"] = "El rol ha sido editado correctamente";
            return RedirectToAction(nameof(Index));
        }

        /**
         * Metodo Post para Borrar un rol. En este método si existe o no el rol, si existe lo borramos
         * de la tabla de roles y de usuarios y guardamos cambios.
         * 
         * @param String con el id 
         * @return redireccción al index.
         * 
         * 
         * @author David Pinazo García
         * @version 1.0
         **/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrar(string id)
        {

            var rolBd = _context.Roles.FirstOrDefault(x => x.Id == id);
            if (rolBd == null)
            {
                TempData["Error"] = "No existe el rol";
                return RedirectToAction(nameof(Index));
            }

            var usuariosRol = _context.UserRoles.Where(u => u.RoleId == id).Count();
            if (usuariosRol > 0)
            {
                TempData["Error"] = "El rol tiene usuarios asignados, NO se puede borrar.";
                return RedirectToAction(nameof(Index));
            }

            await _rolemanager.DeleteAsync(rolBd);
            TempData["Correcto"] = "Rol borrado correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
