using System.ComponentModel.DataAnnotations;

namespace AsistManager.Models.ViewModels
{
    public class AcreditadoViewModel
    {
        public int IdEvento { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "No se permiten números.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "No se permiten números.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "El DNI debe tener entre 6 y 8 caracteres.")]
        [Display(Name = "DNI")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El CUIT es requerido")]
        [Display(Name = "CUIT")]
        public string Cuit { get; set; }

        public string? Celular { get; set; }

        public string? Grupo { get; set; }

        [Required(ErrorMessage = "El Habilitado es requerido")]
        [Display(Name = "Habilitado")]
        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "El Alta es requerido")]
        [Display(Name = "Alta")]
        public bool Alta { get; set; }
    }
}
