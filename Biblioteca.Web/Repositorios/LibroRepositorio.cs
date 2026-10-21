using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Repositorios;

public class LibroRepositorio
{
    private readonly BibliotecaContext _context;

    public LibroRepositorio(BibliotecaContext context)
    {
        _context = context;
    }

    // Libros activos con su autor, filtro opcional por autor y por título, orden por título
    public async Task<List<Libro>> ListarAsync(int? idAutor, string? buscar)
    {
        var consulta = _context.Libros
            .AsNoTracking()
            .Include(l => l.IdAutorNavigation)
            .Where(l => l.Activo);

        if (idAutor.HasValue)
            consulta = consulta.Where(l => l.IdAutor == idAutor.Value);

        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(l => l.Titulo.Contains(buscar));

        return await consulta
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<List<Libro>> ListarMasCarosAsync(int cantidad)
    {
        return await _context.Libros
            .AsNoTracking()
            .Include(l => l.IdAutorNavigation)
            .Where(l => l.Activo)
            .OrderByDescending(l => l.Precio)
            .Take(cantidad)
            .ToListAsync();
    }

    // Agregados que se calculan en la base de datos: Count, Average y Sum
    public async Task<int> ContarActivosAsync()
    {
        return await _context.Libros.CountAsync(l => l.Activo);
    }

    public async Task<decimal> PrecioPromedioAsync()
    {
        return await _context.Libros
            .Where(l => l.Activo)
            .AverageAsync(l => (decimal?)l.Precio) ?? 0;
    }

    public async Task<decimal> ValorCatalogoAsync()
    {
        return await _context.Libros
            .Where(l => l.Activo)
            .SumAsync(l => (decimal?)l.Precio) ?? 0;
    }

    public async Task<Libro?> ObtenerAsync(int id)
    {
        return await _context.Libros
            .AsNoTracking()
            .Include(l => l.IdAutorNavigation)
            .FirstOrDefaultAsync(l => l.IdLibro == id && l.Activo);
    }

    // Datos de la lista desplegable de autores (solo activos)
    public async Task<List<Autor>> ObtenerAutoresParaListaAsync()
    {
        return await _context.Autors
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombre)
            .Select(a => new Autor { IdAutor = a.IdAutor, Nombre = a.Nombre })
            .ToListAsync();
    }

    public async Task<bool> ExisteAutorActivoAsync(int idAutor)
    {
        return await _context.Autors.AnyAsync(a => a.IdAutor == idAutor && a.Activo);
    }

    // Incluye libros inactivos: el UNIQUE de la tabla también los alcanza
    public async Task<bool> ExisteIsbnAsync(string isbn, int? excluirIdLibro = null)
    {
        return await _context.Libros
            .AnyAsync(l => l.Isbn == isbn && l.IdLibro != (excluirIdLibro ?? 0));
    }

    public async Task CrearAsync(Libro libro)
    {
        libro.Activo = true;
        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Libro libro)
    {
        var actual = await _context.Libros.FindAsync(libro.IdLibro);
        if (actual == null || !actual.Activo)
            return false;

        actual.Isbn = libro.Isbn;
        actual.Titulo = libro.Titulo;
        actual.Precio = libro.Precio;
        actual.AnioPublicacion = libro.AnioPublicacion;
        actual.IdAutor = libro.IdAutor;
        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: Activo = 0, nunca un DELETE
    public async Task<bool> DesactivarAsync(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null || !libro.Activo)
            return false;

        libro.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
