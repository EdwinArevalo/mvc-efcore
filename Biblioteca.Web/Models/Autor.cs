using System;
using System.Collections.Generic;

namespace Biblioteca.Web.Models;

public partial class Autor
{
    public int IdAutor { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Nacionalidad { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
