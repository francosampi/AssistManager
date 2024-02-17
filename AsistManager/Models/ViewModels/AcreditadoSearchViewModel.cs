using System.ComponentModel.DataAnnotations;

namespace AsistManager.Models.ViewModels
{
    public class AcreditadoSearchViewModel
    {
        public Acreditado Acreditado { get; set; } = null!;

        public Ingreso? Ingreso { get; set; }

        public Egreso? Egreso { get; set; }
    }
}
