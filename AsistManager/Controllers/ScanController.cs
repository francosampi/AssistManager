using AsistManager.Models;
using AsistManager.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistManager.Controllers
{
    [Authorize]
    public class ScanController : Controller
    {
        private readonly AsistManagerContext _context;

        public ScanController(AsistManagerContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            var evento = _context.Eventos.Find(id);

            return View(evento);
        }

        //Buscar un acreditado por DNI (desde el listado)
        public IActionResult Scan(int id, string codigo)
        {
            var evento = _context.Eventos.Find(id);

            try
            {
                //Descomponer el codigo escaneado
                string[] checkDNI = codigo.Split('\"');

                //Rescatar los datos
                if (checkDNI.Length >= 8)
                {
                    string nroTramite = checkDNI[0];
                    string nombre = checkDNI[1];
                    string apellido = checkDNI[2];
                    string genero = checkDNI[3];
                    string dni = checkDNI[4];
                    string otroDato = checkDNI[5];
                    string fechaNacimiento = checkDNI[6];
                    string fechaEmision = checkDNI[7];

                    //Encontrar registro que coincida con el Evento y el DNI rescatado
                    var acreditado = _context.Acreditados.FirstOrDefault(a => a.Dni == dni && a.IdEvento == id);

                    if (acreditado == null)
                    {
                        //Crear un diccionario temporal, por si se decide cargar al acreditado
                        Dictionary<string, string> datosDNI = new Dictionary<string, string>
                        {
                            { "NroTramite", nroTramite },
                            { "Nombre", nombre },
                            { "Apellido", apellido },
                            { "Genero", genero },
                            { "Documento", dni },
                            { "OtroDato", otroDato },
                            { "FechaNacimiento", fechaNacimiento },
                            { "FechaEmision", fechaEmision },
                            { "IdEvento", id.ToString() }
                        };

                        //Habilitar nueva sección para que el usuario pueda cargarlo
                        TempData["NuevoAcreditadoACargar"] = datosDNI;

                        return RedirectToAction(nameof(Index), new { id = id });
                    }

                    //Buscar ingreso y egreso para este acreditado (si existe)
                    var ingreso = _context.Ingresos.FirstOrDefault(i => i.IdAcreditado == acreditado.Id);
                    var egreso = _context.Egresos.FirstOrDefault(i => i.IdAcreditado == acreditado.Id);

                    //Pasar el acreditado y el registro de ingreso a la vista Details
                    var model = new AcreditadoSearchViewModel
                    {
                        Acreditado = acreditado,
                        Ingreso = ingreso,
                        Egreso = egreso
                    };

                    TempData["HuboEscaneo"] = true;

                    return RedirectToAction(nameof(AcreditadoController.Details), "Acreditado", new { id = id, dni = dni });
                }
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de alerta si el acreditado no existe
                TempData["AlertaTipo"] = "danger";
                TempData["AlertaMensaje"] = $"Error: DNI inválido. {ex.Message}";
            }

            return View(nameof(Index), evento);
        }
    }
}