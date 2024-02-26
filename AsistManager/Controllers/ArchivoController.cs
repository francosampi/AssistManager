using AsistManager.Models;
using AsistManager.Helpers;
using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;

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

        //Ir a vista para importar/exportar datos del Evento
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
        //Leer registros del excel subido
        public IActionResult Read([FromForm] IFormFile file)
        {
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
                    file.CopyTo(stream);
                }

                //Abrir archivo y leerlo linea por linea
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            int contadorRegistros = 0;
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
                                    Acreditado acreditado = Utilities.LeerFilaExcelAcreditado(reader);

                                    //Verificar si alguno de los campos requeridos está vacío
                                    if (string.IsNullOrWhiteSpace(acreditado.Nombre) ||
                                        string.IsNullOrWhiteSpace(acreditado.Apellido) ||
                                        string.IsNullOrWhiteSpace(acreditado.Dni) ||
                                        string.IsNullOrWhiteSpace(acreditado.Cuit))
                                    {
                                        contadorAlertas++;
                                    }

                                    registros.Add(acreditado);
                                }
                            } while (reader.NextResult());

                            contadorRegistros = registros.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return StatusCode(StatusCodes.Status200OK, registros);
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
                            int contadorAlertas = 0;
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
                                    Acreditado? acreditado = Utilities.LeerFilaExcelAcreditado(reader);

                                    //Verificar si alguno de los campos requeridos está vacío
                                    if (string.IsNullOrWhiteSpace(acreditado.Nombre) ||
                                        string.IsNullOrWhiteSpace(acreditado.Apellido) ||
                                        string.IsNullOrWhiteSpace(acreditado.Dni) ||
                                        string.IsNullOrWhiteSpace(acreditado.Cuit))
                                    {
                                        contadorAlertas++;
                                    }
                                    else
                                    {
                                        //Asigno el ID del Evento correspondiente
                                        acreditado.IdEvento = id;

                                        //Insertar registro en la base
                                        _context.Acreditados.Add(acreditado);
                                        contadorRegistros++;
                                    }

                                    registros.Add(acreditado);
                                    
                                }
                            } while (reader.NextResult());

                            //Guardar cambios en la base
                            await _context.SaveChangesAsync();

                            //Informar lo acontecido (registros insertado y alertas)
                            string mensajeRegistros = "Se han insertado los <b>" + contadorRegistros + " registro(s)</b> de los " + registros.Count + " correctamente.";
                            string mensajeAlerta = contadorAlertas > 0 ? "<br><hr/> Hay <b>" + contadorAlertas + " registro(s)</b> con campos vacíos sin insertar." : "";

                            if (contadorRegistros == 0)
                            {
                                mensajeRegistros = "Este archivo se encuentra vacío.";
                                TempData["AlertaTipo"] = "warning";
                            }

                            ViewData["Registros"] = registros;
                            TempData["AlertaMensaje"] = mensajeRegistros + mensajeAlerta;
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

        public IActionResult ExportSheet()
        {
            //Obtener excel precargado, leerlo y descargarlo
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "xlsx", "registros.xlsx");

            //Verificar si el archivo existe
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "registros.xlsx");
        }

        public FileResult ExportAccredited(int id)
        {
            var evento = _context.Eventos.Find(id);

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

                var acreditados = _context.Acreditados
                    .Where(a => a.IdEvento == id);

                //Agregar los datos de los acreditados a la tabla de datos
                foreach (var acreditado in acreditados)
                {
                    dataTable.Rows.Add(
                        acreditado.Nombre,
                        acreditado.Apellido,
                        acreditado.Dni,
                        acreditado.Cuit,
                        acreditado.Celular,
                        acreditado.Grupo,
                        acreditado.Habilitado ? "Sí" : "No",
                        acreditado.Alta ? "Sí" : "No"
                    );
                }

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
