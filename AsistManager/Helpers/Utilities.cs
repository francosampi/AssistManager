using AsistManager.Models;
using ExcelDataReader;
using System.Globalization;
using System.Text;

namespace AsistManager.Helpers
{
    public static class Utilities
    {
        //Verificar si un campo es true o false o no
        public static bool EsCampoVerdadero(string valor)
        {
            valor = valor.ToLower();

            if (valor == "si" || valor == "sí")
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

        public static Acreditado? LeerFilaExcelAcreditado(IExcelDataReader reader)
        {
            //Leer los valores de las celdas
            string nombre = reader.GetValue(0)?.ToString() ?? "";
            string apellido = reader.GetValue(1)?.ToString() ?? "";
            string dni = reader.GetValue(2)?.ToString() ?? "";
            string cuit = reader.GetValue(3)?.ToString() ?? "";

            //Verificar si alguno de los campos requeridos está vacío
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(cuit))
            {
                return null;
            }

            //Crear una instancia si todos los campos requeridos tienen valor
            return new Acreditado
            {
                Nombre = nombre,
                Apellido = apellido,
                Dni = dni,
                Cuit = cuit,
                Celular = reader.GetValue(4)?.ToString() ?? "",
                Grupo = reader.GetValue(5)?.ToString() ?? "",
                Habilitado = Utilities.EsCampoVerdadero(reader.GetValue(6)?.ToString() ?? ""),
                Alta = Utilities.EsCampoVerdadero(reader.GetValue(7)?.ToString() ?? "")
            };
        }
    }
}
