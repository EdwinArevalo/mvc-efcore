using System.Diagnostics;
using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class HomeController : Controller
{
    private readonly AutorRepositorio _autores;
    private readonly LibroRepositorio _libros;

    public HomeController(AutorRepositorio autores, LibroRepositorio libros)
    {
        _autores = autores;
        _libros = libros;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalAutores = await _autores.ContarActivosAsync();
        ViewBag.TotalLibros = await _libros.ContarActivosAsync();
        ViewBag.PrecioPromedio = await _libros.PrecioPromedioAsync();
        ViewBag.ValorCatalogo = await _libros.ValorCatalogoAsync();

        return View(await _libros.ListarMasCarosAsync(5));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
