using System.ComponentModel.DataAnnotations;

namespace AsistManager.Models.ViewModels
{
    public class EventoViewModel
    {
        [Required(ErrorMessage = "El nombre del evento es requerido")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La fecha del evento es requerida")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }
    }
}
