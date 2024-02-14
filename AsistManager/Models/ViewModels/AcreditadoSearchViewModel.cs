using System.ComponentModel.DataAnnotations;

namespace AsistManager.Models.ViewModels
{
    public class AcreditadoSearchViewModel
    {
        public Acreditado Acreditado { get; set; }
        public Ingreso? Ingreso { get; set; }
    }
}
