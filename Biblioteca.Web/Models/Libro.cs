using System;
using System.Collections.Generic;

namespace Biblioteca.Web.Models;

public partial class Libro
{
    public int IdLibro { get; set; }

    public string Isbn { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public decimal Precio { get; set; }

    public int AnioPublicacion { get; set; }

    public int IdAutor { get; set; }

    public bool Activo { get; set; }

    public virtual Autor IdAutorNavigation { get; set; } = null!;
}
