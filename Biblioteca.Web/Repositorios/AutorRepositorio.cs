using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Repositorios;

public class AutorRepositorio
{
    private readonly BibliotecaContext _context;

    public AutorRepositorio(BibliotecaContext context)
    {
        _context = context;
    }

    // Autores activos con sus libros activos (include filtrado), filtro opcional por nombre
    public async Task<List<Autor>> ListarAsync(string? buscar)
    {
        var consulta = _context.Autors
            .AsNoTracking()
            .Include(a => a.Libros.Where(l => l.Activo))
            .Where(a => a.Activo);

        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(a => a.Nombre.Contains(buscar));

        return await consulta
            .OrderBy(a => a.Nombre)
            .ToListAsync();
    }

    public async Task<int> ContarActivosAsync()
    {
        return await _context.Autors.CountAsync(a => a.Activo);
    }

    public async Task<Autor?> ObtenerAsync(int id)
    {
        return await _context.Autors
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAutor == id && a.Activo);
    }

    // Autor con sus libros activos (include filtrado y ordenado)
    public async Task<Autor?> ObtenerConLibrosAsync(int id)
    {
        return await _context.Autors
            .AsNoTracking()
            .Include(a => a.Libros.Where(l => l.Activo).OrderBy(l => l.Titulo))
            .FirstOrDefaultAsync(a => a.IdAutor == id && a.Activo);
    }

    public async Task<int> ContarLibrosActivosAsync(int idAutor)
    {
        return await _context.Libros.CountAsync(l => l.IdAutor == idAutor && l.Activo);
    }

    public async Task CrearAsync(Autor autor)
    {
        autor.Activo = true;
        _context.Autors.Add(autor);
        await _context.SaveChangesAsync();
    }

    // Se busca la entidad (con seguimiento) y se copian solo los campos editables
    public async Task<bool> ActualizarAsync(Autor autor)
    {
        var actual = await _context.Autors.FindAsync(autor.IdAutor);
        if (actual == null || !actual.Activo)
            return false;

        actual.Nombre = autor.Nombre;
        actual.Nacionalidad = autor.Nacionalidad;
        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: Activo = 0, nunca un DELETE
    public async Task<bool> DesactivarAsync(int id)
    {
        var autor = await _context.Autors.FindAsync(id);
        if (autor == null || !autor.Activo)
            return false;

        autor.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
