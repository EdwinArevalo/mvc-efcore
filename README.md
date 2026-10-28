# Biblioteca — ASP.NET Core MVC + EF Core (Code First)

Demo de la **Semana 10** (Desarrollo de Aplicaciones Empresariales Avanzadas). Misma estructura que la guía
de laboratorio (Hotel.Web), con el dominio **Autor / Libro** del PPT:

- Code First: el esquema lo definen los modelos de `Models/` (con **DataAnnotations**) y se crea con **migraciones** de EF Core.
- Acceso a datos solo en `Repositorios/` (LINQ + `SaveChangesAsync`); los controladores **no** usan el `DbContext`.
- Eliminación lógica (`Activo = 0`), patrón Post/Redirect/Get y mensajes con `TempData`.

## Requisitos

- .NET SDK 10 (`dotnet --version`)
- Docker Desktop. En Mac con chip Apple: *Settings → General → Use Rosetta for x86_64/amd64 emulation*
  (la imagen de SQL Server es amd64 y corre emulada).
- VS Code con la extensión C# Dev Kit.

## Ejecutar

```bash
./scripts/levantar-bd.sh        # levanta SQL Server en Docker (la BD aún no existe)
cd Biblioteca.Web
dotnet tool restore             # instala dotnet-ef desde el manifiesto local
dotnet ef database update       # aplica las migraciones y crea BibliotecaDB
dotnet run                      # http://localhost:5297
```

En VS Code: abrir la carpeta `Mvc.Net` y pulsar **F5** (usa `.vscode/launch.json`).

Cadena de conexión (también en `Biblioteca.Web/appsettings.json`):

```
Server=localhost,1433;Database=BibliotecaDB;User Id=sa;Password=Biblioteca_2026;TrustServerCertificate=True
```

## Migraciones (Code First)

Desde la carpeta del proyecto (`Biblioteca.Web`):

```bash
dotnet ef migrations add NombreDeLaMigracion   # genera Migrations/ a partir de los modelos
dotnet ef database update                      # aplica las migraciones pendientes
```

Para cambiar el esquema se edita el modelo (por ejemplo, agregar `Sinopsis` a `Libro`), se crea otra migración y se
aplica. Un campo nuevo también hay que agregarlo a mano en el `[Bind]` del controlador, en `ActualizarAsync` del
repositorio y en el formulario `_Form.cshtml`.

## Estructura

```
Mvc.Net/
├── docker-compose.yml          SQL Server 2022
├── scripts/                    levantar-bd.sh
└── Biblioteca.Web/
    ├── Models/                 Autor · Libro (DataAnnotations) · BibliotecaContext
    ├── Migrations/             generado por dotnet ef migrations add
    ├── Repositorios/           AutorRepositorio · LibroRepositorio (todo el LINQ)
    ├── Controllers/            Home · Autores · Libros
    └── Views/
```

## Notas

- `ModelState.Remove(nameof(Libro.IdAutorNavigation))`: la propiedad de navegación no viene del formulario y,
  al ser no anulable, haría fallar `ModelState.IsValid`.
- Las reglas (formato de ISBN, precio, año, longitudes, duplicados) se validan en los controladores con
  `ModelState.AddModelError`; las vistas son fuertemente tipadas y se enlazan a las entidades.
- `Bind(...)` en las acciones POST evita que se modifiquen campos como `Activo` desde el formulario.
- Usa `net10.0`. Para .NET 8: cambiar `TargetFramework` y las versiones de los paquetes EF a `8.*`.
