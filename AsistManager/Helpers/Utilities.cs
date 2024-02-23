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

        public static Acreditado LeerFilaExcelAcreditado(IExcelDataReader reader)
        {
            Acreditado acreditado = new Acreditado();

            acreditado.Nombre = reader.GetValue(0)?.ToString() ?? "";
            acreditado.Apellido = reader.GetValue(1)?.ToString() ?? "";
            acreditado.Dni = reader.GetValue(2)?.ToString() ?? "";
            acreditado.Cuit = reader.GetValue(3)?.ToString() ?? "";
            acreditado.Celular = reader.GetValue(4)?.ToString() ?? "";
            acreditado.Grupo = reader.GetValue(5)?.ToString() ?? "";

            //Detectar valor booleano o cadena
            string valorHabilitado = reader.GetValue(6)?.ToString() ?? string.Empty;
            acreditado.Habilitado = Utilities.EsCampoVerdadero(valorHabilitado);

            //Detectar valor booleano o cadena
            string valorAlta = reader.GetValue(7)?.ToString() ?? string.Empty;
            acreditado.Alta = Utilities.EsCampoVerdadero(valorAlta);

            return acreditado;
        }
    }
}
