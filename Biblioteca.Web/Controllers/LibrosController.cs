using System.Text.RegularExpressions;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers;

public class LibrosController : Controller
{
    private readonly LibroRepositorio _libros;

    public LibrosController(LibroRepositorio libros)
    {
        _libros = libros;
    }

    public async Task<IActionResult> Index(int? idAutor, string? buscar)
    {
        await CargarAutoresAsync(idAutor);
        ViewBag.Buscar = buscar;
        return View(await _libros.ListarAsync(idAutor, buscar));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var libro = await _libros.ObtenerAsync(id.Value);
        if (libro == null) return NotFound();

        return View(libro);
    }

    public async Task<IActionResult> Create()
    {
        await CargarAutoresAsync();
        return View(new Libro { AnioPublicacion = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Isbn,Titulo,Precio,AnioPublicacion,IdAutor")] Libro libro)
    {
        // La propiedad de navegación no viene del formulario: se quita de la validación
        ModelState.Remove(nameof(Libro.IdAutorNavigation));
        await ValidarReglasAsync(libro);

        if (!ModelState.IsValid)
        {
            await CargarAutoresAsync(libro.IdAutor);
            return View(libro);
        }

        await _libros.CrearAsync(libro);
        TempData["Mensaje"] = $"Libro \"{libro.Titulo}\" registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var libro = await _libros.ObtenerAsync(id.Value);
        if (libro == null) return NotFound();

        await CargarAutoresAsync(libro.IdAutor);
        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdLibro,Isbn,Titulo,Precio,AnioPublicacion,IdAutor")] Libro libro)
    {
        if (id != libro.IdLibro) return NotFound();

        ModelState.Remove(nameof(Libro.IdAutorNavigation));
        await ValidarReglasAsync(libro);

        if (!ModelState.IsValid)
        {
            await CargarAutoresAsync(libro.IdAutor);
            return View(libro);
        }

        if (!await _libros.ActualizarAsync(libro))
            return NotFound();

        TempData["Mensaje"] = $"Libro \"{libro.Titulo}\" actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var libro = await _libros.ObtenerAsync(id.Value);
        if (libro == null) return NotFound();

        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _libros.ObtenerAsync(id);
        if (libro == null) return NotFound();

        await _libros.DesactivarAsync(id);
        TempData["Mensaje"] = $"Libro \"{libro.Titulo}\" eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // Reglas de negocio: formato, rangos y consultas a la base de datos
    private async Task ValidarReglasAsync(Libro libro)
    {
        if (libro.IdAutor <= 0)
            ModelState.AddModelError(nameof(Libro.IdAutor), "Seleccione un autor.");
        else if (!await _libros.ExisteAutorActivoAsync(libro.IdAutor))
            ModelState.AddModelError(nameof(Libro.IdAutor), "El autor seleccionado no existe o está inactivo.");

        if (string.IsNullOrWhiteSpace(libro.Isbn))
        {
            ModelState.Remove(nameof(Libro.Isbn));
            ModelState.AddModelError(nameof(Libro.Isbn), "El ISBN es obligatorio.");
        }
        else
        {
            if (!Regex.IsMatch(libro.Isbn, @"^\d{13}$"))
                ModelState.AddModelError(nameof(Libro.Isbn), "El ISBN debe tener exactamente 13 dígitos.");
            else if (await _libros.ExisteIsbnAsync(libro.Isbn, libro.IdLibro))
                ModelState.AddModelError(nameof(Libro.Isbn), "Ya existe un libro con este ISBN.");
        }

        if (string.IsNullOrWhiteSpace(libro.Titulo))
        {
            ModelState.Remove(nameof(Libro.Titulo));
            ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        }
        else if (libro.Titulo.Length > 150)
        {
            ModelState.AddModelError(nameof(Libro.Titulo), "El título no puede superar los 150 caracteres.");
        }

        if (libro.Precio <= 0 || libro.Precio > 999999.99m)
            ModelState.AddModelError(nameof(Libro.Precio), "El precio debe ser mayor que 0 y no superar 999999.99.");

        if (libro.AnioPublicacion < 1400 || libro.AnioPublicacion > DateTime.Now.Year)
            ModelState.AddModelError(nameof(Libro.AnioPublicacion), $"El año debe estar entre 1400 y {DateTime.Now.Year}.");
    }

    private async Task CargarAutoresAsync(int? seleccionado = null)
    {
        var autores = await _libros.ObtenerAutoresParaListaAsync();
        ViewBag.Autores = new SelectList(autores, nameof(Autor.IdAutor), nameof(Autor.Nombre), seleccionado);
    }
}
