using System;
using System.Collections.Generic;

namespace AsistManager.Models;

public partial class Evento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public virtual ICollection<Acreditado> Acreditados { get; set; } = new List<Acreditado>();
}
