using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Web.Models;

[Table("Autor")]
public class Autor
{
    [Key]
    public int IdAutor { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? Nacionalidad { get; set; }

    public bool Activo { get; set; } = true;

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
