using System;
using System.Collections.Generic;

namespace AsistManager.Models;

public partial class Egreso
{
    public int Id { get; set; }

    public int IdAcreditado { get; set; }

    public int IdIngreso { get; set; }

    public DateTime FechaOperacion { get; set; }

    public virtual Acreditado IdAcreditadoNavigation { get; set; } = null!;

    public virtual Ingreso IdIngresoNavigation { get; set; } = null!;
}
