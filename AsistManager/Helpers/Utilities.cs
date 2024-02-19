namespace AsistManager.Helpers
{
    public static class Utilities
    {
        //Verificar si un campo del Excel es boolean o no
        public static bool? EsCampoVerdadero(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            valor = valor.ToLower();

            if (valor == "si")
            {
                return true;
            }

            if (valor == "no")
            {
                return false;
            }

            if (bool.TryParse(valor, out bool result))
            {
                return result;
            }

            return null;
        }
    }
}
