using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Models;

[Table("Libro")]
[Index(nameof(Isbn), IsUnique = true)]
[Index(nameof(IdAutor))]
public class Libro
{
    [Key]
    public int IdLibro { get; set; }

    [Required]
    [StringLength(13, MinimumLength = 13)]
    [Column(TypeName = "char(13)")]
    public string Isbn { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = null!;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal Precio { get; set; }

    public int AnioPublicacion { get; set; }

    public int IdAutor { get; set; }

    public bool Activo { get; set; } = true;

    [ForeignKey(nameof(IdAutor))]
    public virtual Autor IdAutorNavigation { get; set; } = null!;
}
