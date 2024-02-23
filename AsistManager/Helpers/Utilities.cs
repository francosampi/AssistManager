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
        public static string PrepareFilter(string filtro)
        {
            filtro = filtro.Trim().ToLower();
            filtro = RemoveAccents(filtro);
            return filtro;
        }
    }
}
