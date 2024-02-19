using AsistManager.Models;
using AsistManager.Models.ViewModels;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace AsistManager.Controllers
{
    public class EventoController : Controller
    {
        private readonly AsistManagerContext _context;

        public EventoController(AsistManagerContext context)
        {
            _context = context;
        }

        //Traer eventos asíncronamente e ir a la vista del listado
        public async Task<IActionResult> Index()
        {
            var eventos = _context.Eventos;

            return View(await eventos.ToListAsync());
        }

        //Ir a la vista para las operaciones del Evento
        public IActionResult Menu(int id)
        {
            var evento = _context.Eventos.Find(id);

            //Verificar si el evento existe
            if (evento == null)
            {
                return NotFound();
            }

            //Traer informacion de la base para generar el Chart
            var acreditados = _context.Acreditados;

            var chartRegistros = acreditados.Where(i => i.IdEvento == id).Count();
            var chartHabilitados = acreditados.Where(i => i.IdEvento == id && i.Habilitado == true).Count();
            var chartNoHabilitados = acreditados.Where(i => i.IdEvento == id && i.Habilitado == false).Count();
            var chartIngreso = acreditados.Where(i => i.IdEvento == id && _context.Ingresos.Any(ingreso => ingreso.IdAcreditado == i.Id)).Count();
            var chartEgreso = acreditados.Where(i => i.IdEvento == id && _context.Egresos.Any(egreso => egreso.IdAcreditado == i.Id)).Count();

            ViewData["ChartRegistros"] = chartRegistros;
            ViewData["ChartHabilitados"] = chartHabilitados;
            ViewData["ChartNoHabilitados"] = chartNoHabilitados;
            ViewData["ChartIngreso"] = chartIngreso;
            ViewData["ChartEgreso"] = chartEgreso;

            return View(evento);
        }

        //Ir a vista para insertar un Evento
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Crear un evento nuevo
        public async Task<IActionResult> Create(EventoViewModel model)
        {
            //Verifico si el modelo pasado desde la vista es válido (validaciones de campos)
            if (ModelState.IsValid)
            {
                //Creo instancia de la clase Evento y cargo los datos del ViewModel
                var evento = new Evento()
                {
                    Nombre = model.Nombre,
                    FechaInicio = model.FechaInicio
                };

                //Agrego y guardo los cambios en la base de datos
                _context.Add(evento);

                await _context.SaveChangesAsync();

                //Redirecciono al Index y muestro alerta
                TempData["AlertaMensaje"] = "El evento '<b>" + model.Nombre + "</b>' se agregó exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        //Ir a vista para editar Evento
        public IActionResult Update(int id)
        {
            var evento = _context.Eventos.Find(id);

            //Verificar si el evento existe
            if (evento == null)
            {
                return NotFound();
            }

            ViewData["NombreEvento"] = evento.Nombre;
            ViewData["FechaEvento"] = evento.FechaInicio;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Editar el Evento
        public IActionResult Update(int id, EventoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var evento = _context.Eventos.Find(id);

                //Verificar si el evento existe
                if (evento != null)
                {
                    //Actualizar propiedades del evento
                    evento.Nombre = model.Nombre;
                    evento.FechaInicio = model.FechaInicio;

                    //Guardo los cambios en la base de datos
                    _context.SaveChanges();

                    //Redirecciono al Index del controlador y muestro alerta
                    TempData["AlertaMensaje"] = "El evento '<b>" + model.Nombre + "</b>' se modificó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Lanzar error si no existe el evento
                    return NotFound();
                }
            }

            //Si hay errores de validación, regresar a la vista de edición
            return View(model);
        }

        //Ir a vista para borrar Evento
        public IActionResult Delete(int id)
        {
            var evento = _context.Eventos.Find(id);

            //Verificar si el evento existe
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Borrar el Evento
        public IActionResult Delete(int id, int a)
        {
            //Hago Select del Evento e incluyo en la consulta tanto sus acreditados como sus ingresos y egresos
            var evento = _context.Eventos
                .Include(e => e.Acreditados)
                    .ThenInclude(a => a.Ingresos)
                .Include(e => e.Acreditados)
                    .ThenInclude(a=> a.Egresos)
                .FirstOrDefault(e => e.Id == id);

            //Verificar que el evento existe
            if (evento != null)
            {
                //Obtener los acreditados del evento y la cantidad a borrar
                var acreditados = evento.Acreditados;
                int cantidadAcreditados = acreditados.ToList().Count();

                //Eliminar los ingresos y egresos del acreditado
                foreach (var acreditado in acreditados)
                {
                    _context.Ingresos.RemoveRange(acreditado.Ingresos);
                    _context.Egresos.RemoveRange(acreditado.Egresos);
                }

                _context.Acreditados.RemoveRange(acreditados);

                //Una vez eliminados los acreditados, ingresos y egresos, elimino el evento
                _context.Eventos.Remove(evento);
                _context.SaveChanges();

                //Redireccionar al Index y mostrar alerta
                TempData["AlertaMensaje"] = $"El evento '<b>{evento.Nombre}</b>' y sus <b>{cantidadAcreditados}</b> acreditados se eliminaron exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            //Lanzar error si no existe el evento
            return NotFound();
        }
    }
}
