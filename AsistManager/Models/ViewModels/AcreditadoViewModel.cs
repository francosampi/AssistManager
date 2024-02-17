using System.ComponentModel.DataAnnotations;

namespace AsistManager.Models.ViewModels
{
    public class AcreditadoViewModel
    {
        public int IdEvento { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es requerido")]
        [Display(Name = "DNI")]
        public string? Dni { get; set; }

        [Required(ErrorMessage = "El CUIT es requerido")]
        [Display(Name = "CUIT")]
        public string? Cuit { get; set; }

        public string? Celular { get; set; }

        public string? Grupo { get; set; }

        public bool? Habilitado { get; set; }

        public bool? Alta { get; set; }
    }
}
