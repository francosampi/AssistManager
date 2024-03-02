using AsistManager.Models;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Text;

namespace AsistManager.Helpers
{
    public static class Utilities
    {
        //Verificar si un campo es true o false
        public static bool EsCampoVerdadero(string valor)
        {
            valor = valor.ToLower();

            if (valor == "si" || valor == "sí" || valor=="s")
            {
                return true;
            }

            if (bool.TryParse(valor, out bool result))
            {
                return result;
            }

            return false;
        }

        //Para remover tildes
        public static string RemoveAccents(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        //Preparar string para un filtro
        public static string PrepareFilter(string filter)
        {
            filter = filter.Trim().ToLower();
            filter = RemoveAccents(filter);
            return filter;
        }

        public static Acreditado LeerFilaExcelAcreditado(IExcelDataReader reader)
        {
            //Leer los valores de las celdas
            string nombre = reader.GetValue(0)?.ToString() ?? "";
            string apellido = reader.GetValue(1)?.ToString() ?? "";
            string dni = reader.GetValue(2)?.ToString() ?? "";
            string cuit = reader.GetValue(3)?.ToString() ?? "";
            string celular = reader.GetValue(4)?.ToString() ?? "";
            string grupo = reader.GetValue(5)?.ToString() ?? "";
            bool habilitado = Utilities.EsCampoVerdadero(reader.GetValue(6)?.ToString() ?? "");
            bool alta = Utilities.EsCampoVerdadero(reader.GetValue(7)?.ToString() ?? "");

            //Crear una instancia si todos los campos requeridos tienen valor
            return new Acreditado
            {
                Nombre = nombre,
                Apellido = apellido,
                Dni = dni,
                Cuit = cuit,
                Celular = celular,
                Grupo = grupo,
                Habilitado = habilitado,
                Alta = alta
            };
        }

        public static void AddRowAcreditado(Acreditado acreditado, DataTable dataTable)
        {
            string fechaIngreso = "-";
            string horaIngreso = "-";

            string fechaEgreso = "-";
            string horaEgreso = "-";

            //Obtener la fecha y hora de ingreso
            if (acreditado.Ingresos.Any())
            {
                var primerIngreso = acreditado.Ingresos.First();
                fechaIngreso = primerIngreso.FechaOperacion.ToString("dd/MM/yyyy");
                horaIngreso = primerIngreso.FechaOperacion.ToString("HH:mm:ss");
            }

            //Obtener la fecha y hora de egreso
            if (acreditado.Egresos.Any())
            {
                var primerEgreso = acreditado.Egresos.First();
                fechaEgreso = primerEgreso.FechaOperacion.ToString("dd/MM/yyyy");
                horaEgreso = primerEgreso.FechaOperacion.ToString("HH:mm:ss");
            }

            //Por cada registro, escribo en el excel
            dataTable.Rows.Add(
                acreditado.Nombre,
                acreditado.Apellido,
                acreditado.Dni,
                acreditado.Cuit,
                acreditado.Celular,
                acreditado.Grupo,
                acreditado.Habilitado ? "Sí" : "No",
                acreditado.Alta ? "Sí" : "No",
                fechaIngreso,
                horaIngreso,
                fechaEgreso,
                horaEgreso
            );
        }
    }
}
