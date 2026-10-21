using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class AutoresController : Controller
{
    private readonly AutorRepositorio _autores;

    public AutoresController(AutorRepositorio autores)
    {
        _autores = autores;
    }

    public async Task<IActionResult> Index(string? buscar)
    {
        ViewBag.Buscar = buscar;
        return View(await _autores.ListarAsync(buscar));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _autores.ObtenerConLibrosAsync(id.Value);
        if (autor == null) return NotFound();

        return View(autor);
    }

    public IActionResult Create()
    {
        return View(new Autor());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Nacionalidad")] Autor autor)
    {
        ValidarReglas(autor);

        if (!ModelState.IsValid)
            return View(autor);

        await _autores.CrearAsync(autor);
        TempData["Mensaje"] = $"Autor \"{autor.Nombre}\" registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _autores.ObtenerAsync(id.Value);
        if (autor == null) return NotFound();

        return View(autor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdAutor,Nombre,Nacionalidad")] Autor autor)
    {
        if (id != autor.IdAutor) return NotFound();

        ValidarReglas(autor);

        if (!ModelState.IsValid)
            return View(autor);

        if (!await _autores.ActualizarAsync(autor))
            return NotFound();

        TempData["Mensaje"] = $"Autor \"{autor.Nombre}\" actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _autores.ObtenerAsync(id.Value);
        if (autor == null) return NotFound();

        ViewBag.CantidadLibros = await _autores.ContarLibrosActivosAsync(id.Value);
        return View(autor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var autor = await _autores.ObtenerAsync(id);
        if (autor == null) return NotFound();

        // Regla de negocio: no se desactiva un autor que aún tiene libros activos
        var cantidad = await _autores.ContarLibrosActivosAsync(id);
        if (cantidad > 0)
        {
            ViewBag.CantidadLibros = cantidad;
            ModelState.AddModelError(string.Empty,
                $"No se puede eliminar: el autor tiene {cantidad} libro(s) activo(s). Elimine primero sus libros.");
            return View(autor);
        }

        await _autores.DesactivarAsync(id);
        TempData["Mensaje"] = $"Autor \"{autor.Nombre}\" eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidarReglas(Autor autor)
    {
        if (string.IsNullOrWhiteSpace(autor.Nombre))
        {
            ModelState.Remove(nameof(Autor.Nombre));
            ModelState.AddModelError(nameof(Autor.Nombre), "El nombre es obligatorio.");
        }
        else if (autor.Nombre.Length > 100)
        {
            ModelState.AddModelError(nameof(Autor.Nombre), "El nombre no puede superar los 100 caracteres.");
        }

        if (autor.Nacionalidad?.Length > 50)
            ModelState.AddModelError(nameof(Autor.Nacionalidad), "La nacionalidad no puede superar los 50 caracteres.");
    }
}
