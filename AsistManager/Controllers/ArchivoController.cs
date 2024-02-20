using AsistManager.Models;
using AsistManager.Helpers;
using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AsistManager.Controllers
{
    [Authorize]
    public class ArchivoController : Controller
    {
        private readonly AsistManagerContext _context;

        public ArchivoController(AsistManagerContext context)
        {
            _context = context;
        }

        //Ir a vista para importar datos del Evento
        public IActionResult Index(int id)
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
        //Subir el excel de la base y leer registros
        public async Task<IActionResult> Read(int id, IFormFile file)
        {
            var evento = _context.Eventos.Find(id);

            //Manejo de la codificación de caracteres específicos durante la lectura del archivo
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Lista temporal para mostrar los registros del archivo
            List<Acreditado> registros = new List<Acreditado>();

            //Verificar que el archivo no sea nulo
            if (file != null && file.Length > 0)
            {
                //Crear directorio para guardarlo, y una lista temporal
                var carpetaUploads = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                var filePath = Path.Combine(carpetaUploads, file.FileName);

                if (!Directory.Exists(carpetaUploads))
                {
                    Directory.CreateDirectory(carpetaUploads);
                }

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //Abrir archivo y leerlo linea por linea
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            int contadorRegistros=0;
                            int contadorAlertas = 0;
                            bool flagHeader = false;

                            do
                            {
                                //Leer cada registro, excepto el encabezado
                                while (reader.Read())
                                {
                                    if (!flagHeader)
                                    {
                                        flagHeader = true;
                                        continue;
                                    }

                                    //Por cada registro, generar un objeto
                                    Acreditado acreditado = new Acreditado();

                                    acreditado.Nombre = reader.GetValue(0)?.ToString();
                                    acreditado.Apellido = reader.GetValue(1)?.ToString();
                                    acreditado.Dni = reader.GetValue(2)?.ToString();
                                    acreditado.Cuit = reader.GetValue(3)?.ToString();
                                    acreditado.Celular = reader.GetValue(4)?.ToString();
                                    acreditado.Grupo = reader.GetValue(5)?.ToString();

                                    //Detectar valor booleano o cadena
                                    string valorHabilitado = reader.GetValue(6)?.ToString() ?? string.Empty;
                                    acreditado.Habilitado = Utilities.EsCampoVerdadero(valorHabilitado);

                                    //Detectar valor booleano o cadena
                                    string valorAlta = reader.GetValue(7)?.ToString() ?? string.Empty;
                                    acreditado.Alta = Utilities.EsCampoVerdadero(valorAlta);

                                    if (acreditado.Dni==null)
                                    {
                                        contadorAlertas++;
                                    }

                                    registros.Add(acreditado);
                                    contadorRegistros++;
                                }
                            } while (reader.NextResult());

                            //Informar lo acontecido (registros leídos y alertas)
                            string mensajeRegistros = "Se han leído los <b>" + contadorRegistros + " registro(s)</b> correctamente. ";
                            string mensajeAlerta = contadorAlertas>0 ? "Hay <b>" +contadorAlertas + " alerta(s)</b>." : "";

                            if (contadorRegistros == 0)
                            {
                                mensajeRegistros = "Este archivo se encuentra vacío.";
                                TempData["AlertaTipo"] = "warning";
                            }

                            ViewData["Registros"] = registros;
                            TempData["AlertaMensaje"] = mensajeRegistros+mensajeAlerta;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Informar error
                        TempData["AlertaTipo"] = "danger";
                        TempData["AlertaMensaje"] = "Error al leer el archivo. ("+ex.Message+")";
                    }
                }
            }

            return View(nameof(Index), evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Subir el excel de la base e insertar registros
        public async Task<IActionResult> Insert(int id, IFormFile file)
        {
            var evento = _context.Eventos.Find(id);

            //Manejo de la codificación de caracteres específicos durante la lectura del archivo
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Lista temporal para mostrar los registros del archivo
            List<Acreditado> registros = new List<Acreditado>();

            //Verificar que el archivo no sea nulo
            if (file != null && file.Length > 0)
            {
                //Crear directorio para guardarlo, y una lista temporal
                var carpetaUploads = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                var filePath = Path.Combine(carpetaUploads, file.FileName);

                if (!Directory.Exists(carpetaUploads))
                {
                    Directory.CreateDirectory(carpetaUploads);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //Abrir archivo y leerlo linea por linea
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            int contadorRegistros = 0;
                            bool flagHeader = false;

                            do
                            {
                                //Leer cada registro en la base, excepto el encabezado
                                while (reader.Read())
                                {
                                    if (!flagHeader)
                                    {
                                        flagHeader = true;
                                        continue;
                                    }

                                    //Por cada registro, generar un objeto
                                    Acreditado acreditado = new Acreditado();

                                    acreditado.Nombre = reader.GetValue(0)?.ToString();
                                    acreditado.Apellido = reader.GetValue(1)?.ToString();
                                    acreditado.Dni = reader.GetValue(2)?.ToString();
                                    acreditado.Cuit = reader.GetValue(3)?.ToString();
                                    acreditado.Celular = reader.GetValue(4)?.ToString();
                                    acreditado.Grupo = reader.GetValue(5)?.ToString();

                                    //Detectar valor booleano o cadena
                                    string valorHabilitado = reader.GetValue(6)?.ToString() ?? string.Empty;
                                    acreditado.Habilitado = Utilities.EsCampoVerdadero(valorHabilitado);

                                    //Detectar valor booleano o cadena
                                    string valorAlta = reader.GetValue(7)?.ToString() ?? string.Empty;
                                    acreditado.Alta = Utilities.EsCampoVerdadero(valorAlta);

                                    //Asigno el ID del Evento correspondiente
                                    acreditado.IdEvento = id;

                                    //Insertar registro en la base
                                    _context.Acreditados.Add(acreditado);

                                    registros.Add(acreditado);
                                    contadorRegistros++;
                                }
                            } while (reader.NextResult());

                            //Guardar cambios en la base
                            await _context.SaveChangesAsync();

                            //Informar lo acontecido (registros insertado y alertas)
                            string mensajeRegistros = "Se han insertado los <b>" + contadorRegistros + " registro(s)</b> correctamente. ";

                            if (contadorRegistros == 0)
                            {
                                mensajeRegistros = "Este archivo se encuentra vacío.";
                                TempData["AlertaTipo"] = "warning";
                            }

                            ViewData["Registros"] = registros;
                            TempData["AlertaMensaje"] = mensajeRegistros;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Informar error
                        TempData["AlertaTipo"] = "danger";
                        TempData["AlertaMensaje"] = "Error al leer el archivo. (" + ex.Message + ")";
                    }
                }
            }

            return View(nameof(Index), evento);
        }

        public FileResult Export(int id)
        {
            try
            {
                DataTable dataTable = new DataTable("Registros");

                dataTable.Columns.AddRange(
                [
                    new DataColumn("Nombre"),
                    new DataColumn("Apellido"),
                    new DataColumn("DNI"),
                    new DataColumn("CUIT"),
                    new DataColumn("Celular"),
                    new DataColumn("Grupo"),
                    new DataColumn("Habilitado"),
                    new DataColumn("Alta")
                ]);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "registros.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                //Informar error
                TempData["AlertaTipo"] = "danger";
                TempData["AlertaMensaje"] = "Error al leer el archivo. (" + ex.Message + ")";
            }

            return File(new byte[0], "application/octet-stream");
        }
    }
}
