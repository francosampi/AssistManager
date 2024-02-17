using AsistManager.Models;
using AsistManager.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsistManager.Controllers
{
    public class AcreditadoController : Controller
    {
        private readonly AsistManagerContext _context;

        public AcreditadoController(AsistManagerContext context)
        {
            _context = context;
        }

        //Traer acreditados del evento e ir a la vista del listado
        public async Task<IActionResult> Index(int id)
        {
            //Buscar Acreditado con su Evento asignado
            var evento = _context.Eventos.Find(id);
            var acreditados = _context.Acreditados
                .Where(a => a.IdEvento == id);

            ViewData["Evento"] = evento;

            return View(await acreditados.ToListAsync());
        }

        //Ir a vista para insertar un Acreditado
        public IActionResult Create(int id)
        {
            var evento = _context.Eventos.Find(id);

            if(evento == null)
            {
                return NotFound();
            }

            ViewData["Evento"] = evento;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Crear un acreditado nuevo
        public async Task<IActionResult> Create(AcreditadoViewModel model, int id)
        {
            //Verifico si el modelo pasado desde la vista es válido (validaciones de campos)
            if (ModelState.IsValid)
            {
                //Asigno el id del Evento correspondiente
                model.IdEvento = id;

                //Creo instancia de la clase Acreditado y cargo los datos del ViewModel
                var acreditado = new Acreditado()
                {
                    IdEvento = model.IdEvento,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Dni = model.Dni,
                    Celular = model.Celular,
                    Cuit = model.Cuit,
                    Grupo = model.Grupo,
                    Habilitado = true,
                };

                //Agrego y guardo los cambios en la base de datos
                _context.Add(acreditado);
                await _context.SaveChangesAsync();

                //Redirecciono al Index y muestro alerta
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = $"El registro '<b>{model.Dni}</b>' se agregó exitosamente.";

                return RedirectToAction(nameof(Index), new {id = id});
            }

            return View(model);
        }

        //Buscar un acreditado por DNI (desde el listado)
        public IActionResult Search(int id, string dni)
        {
            if (string.IsNullOrEmpty(dni))
            {
                //Mostrar mensaje de alerta si el acreditado no existe
                TempData["AlertaTipo"] = "warning";
                TempData["AlertaMensaje"] = $"No se encontró el DNI.";

                return RedirectToAction(nameof(Index), new { id = id });
            }

            //Encontrar registro que coincida con DNI y Evento
            var acreditado = _context.Acreditados.FirstOrDefault(a => a.Dni == dni && a.IdEvento == id);

            if (acreditado == null)
            {
                //Mostrar mensaje de alerta si el acreditado no existe
                TempData["AlertaTipo"] = "danger";
                TempData["AlertaMensaje"] = $"No se encontró ningún registro con el DNI:  <b>{dni}</b>.";

                return RedirectToAction(nameof(Index), new { id = id });
            }

            //Buscar ingreso y egreso para este acreditado (si existe)
            var ingreso = _context.Ingresos.FirstOrDefault(i => i.IdAcreditado == acreditado.Id);
            var egreso = _context.Egresos.FirstOrDefault(i => i.IdAcreditado == acreditado.Id);

            //Pasar el acreditado y el registro de ingreso a la vista Search
            var model = new AcreditadoSearchViewModel
            {
                Acreditado = acreditado,
                Ingreso = ingreso,
                Egreso = egreso
            };

            return View(model);
        }


        //Ir a vista para editar un Acreditado
        public IActionResult Update(int id)
        {
            var acreditado = _context.Acreditados
                .Include(a => a.IdEventoNavigation)
                .FirstOrDefault(a => a.Id == id);

            //Verificar si el acreditado existe y tiene un evento asignado
            if (acreditado?.IdEventoNavigation == null)
            {
                return NotFound();
            }

            AcreditadoViewModel acreditadoView = new AcreditadoViewModel()
            {
                Nombre = acreditado.Nombre,
                Apellido = acreditado.Apellido,
                Celular = acreditado.Celular,
                Cuit = acreditado.Cuit,
                Dni = acreditado.Dni,
                Grupo = acreditado.Grupo,
            };

            ViewData["Evento"] = acreditado.IdEventoNavigation;

            return View(acreditadoView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Editar el Acreditado
        public IActionResult Update(int id, AcreditadoViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Verificar si el acreditado existe
                    var acreditado = _context.Acreditados.Find(id);

                    if (acreditado != null)
                    {
                        //Actualizar propiedades del acreditado
                        acreditado.Nombre = model.Nombre;
                        acreditado.Apellido = model.Apellido;
                        acreditado.Dni = model.Dni;
                        acreditado.Cuit = model.Cuit;
                        acreditado.Celular = model.Celular;
                        acreditado.Grupo = model.Grupo;

                        //Guardar cambios en la base de datos
                        _context.SaveChanges();

                        //Mostrar mensaje de alerta
                        TempData["AlertaTipo"] = "success";
                        TempData["AlertaMensaje"] = $"El registro '<b>{model.Dni}</b>' se modificó exitosamente.";

                        //Redireccionar al Index
                        return RedirectToAction(nameof(Index), new {id = acreditado.IdEvento});
                    }
                    else
                    {
                        //Mostrar mensaje de alerta si el acreditado no existe
                        TempData["AlertaTipo"] = "danger";
                        TempData["AlertaMensaje"] = "El acreditado no fue encontrado.";

                        return NotFound();
                    }
                }

                //Si hay errores de validación, regresar a la vista de edición
                return View(model);
            }
            catch (Exception ex)
            {
                //Informar error
                TempData["AlertaTipo"] = "danger";
                TempData["AlertaMensaje"] = $"Error al editar el acreditado: {ex.Message}";

                return View(model);
            }
        }

        //Ir a vista para borrar un Acreditado
        public IActionResult Delete(int id)
        {
            var acreditado = _context.Acreditados
                .Include(a => a.IdEventoNavigation)
                .FirstOrDefault(a => a.Id == id);

            //Verificar si el acreditado existe y tiene un evento asignado
            if (acreditado?.IdEventoNavigation == null)
            {
                return NotFound();
            }

            ViewData["Evento"] = acreditado.IdEventoNavigation;

            return View(acreditado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Borrar el Acreditado
        public IActionResult Delete(int id, int a)
        {
            var acreditado = _context.Acreditados.Find(id);

            //Verificar si el acreditado existe
            if (acreditado != null)
            {
                //Borrar acreditado
                _context.Acreditados.Remove(acreditado);

                //Guardar cambios en la base de datos
                _context.SaveChanges();

                //Redirecciono al Index y muestro alerta
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = $"El acreditado '<b>{acreditado.Dni}</b>' se eliminó exitosamente.";

                return RedirectToAction(nameof(Index), new { id = acreditado.IdEvento });
            }

            //Lanzar error si no existe el acreditado
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entry(int id)
        {
            var acreditado = _context.Acreditados.Find(id);

            if (acreditado != null)
            {
                var fecha = DateTime.Now;

                //Crear ingreso
                var ingreso = new Ingreso()
                {
                    IdAcreditado = id,
                    FechaOperacion = fecha,
                };

                //Guardar cambios en la base de datos
                _context.Ingresos.Add(ingreso);
                await _context.SaveChangesAsync();

                //Redirecciono al Index y muestro alerta
                TempData["AlertaTipo"] = "success";
                TempData["AlertaMensaje"] = $"Se registró el ingreso del registro <b>{acreditado.Dni}</b> exitosamente ({fecha}).";

                return RedirectToAction(nameof(Index), new { id = acreditado.IdEvento });
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Exit(int id)
        {
            var acreditado = _context.Acreditados.Find(id);

            if (acreditado != null)
            {
                var fecha = DateTime.Now;
                var ingreso = _context.Ingresos.ElementAt(0);

                if (ingreso != null)
                {
                    //Crear egreso
                    var egreso = new Egreso()
                    {
                        IdAcreditado = id,
                        FechaOperacion = fecha,
                    };

                    //Guardar cambios en la base de datos
                    _context.Egresos.Add(egreso);
                    await _context.SaveChangesAsync();

                    //Redirecciono al Index y muestro alerta
                    TempData["AlertaTipo"] = "success";
                    TempData["AlertaMensaje"] = $"Se registró el egreso del registro <b>{acreditado.Dni}</b> exitosamente ({fecha}).";

                    return RedirectToAction(nameof(Index), new { id = acreditado.IdEvento });
                }
            }

            return NotFound();
        }
    }
}
