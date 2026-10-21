# Biblioteca — ASP.NET Core MVC + EF Core (Database First)

Demo de la **Semana 10** (Desarrollo de Aplicaciones Empresariales Avanzadas). Misma estructura que la guía
de laboratorio (Hotel.Web), con el dominio **Autor / Libro** del PPT:

- Base de datos primero: el esquema lo define `db/BibliotecaDB.sql` (sin migraciones, sin procedimientos almacenados).
- Modelo generado con **scaffold** en `Models/` (no se edita a mano).
- Acceso a datos solo en `Repositorios/` (LINQ + `SaveChangesAsync`); los controladores **no** usan el `DbContext`.
- Eliminación lógica (`Activo = 0`), patrón Post/Redirect/Get y mensajes con `TempData`.

## Requisitos

- .NET SDK 10 (`dotnet --version`)
- Docker Desktop. En Mac con chip Apple: *Settings → General → Use Rosetta for x86_64/amd64 emulation*
  (la imagen de SQL Server es amd64 y corre emulada).
- VS Code con la extensión C# Dev Kit.

## Ejecutar

```bash
./scripts/cargar-bd.sh          # levanta SQL Server en Docker y crea BibliotecaDB con datos
cd Biblioteca.Web
dotnet run                      # http://localhost:5297
```

En VS Code: abrir la carpeta `Mvc.Net` y pulsar **F5** (usa `.vscode/launch.json`).
Para volver a dejar la BD como al inicio, ejecutar de nuevo `./scripts/cargar-bd.sh`.

Cadena de conexión (también en `Biblioteca.Web/appsettings.json`):

```
Server=localhost,1433;Database=BibliotecaDB;User Id=sa;Password=Biblioteca_2026;TrustServerCertificate=True
```

## Scaffold (Database First)

Desde la carpeta del proyecto (`Biblioteca.Web`). La herramienta `dotnet-ef` ya está en el manifiesto local
(`dotnet tool restore`); si se prefiere global: `dotnet tool install --global dotnet-ef`.

```bash
dotnet ef dbcontext scaffold \
  "Server=localhost,1433;Database=BibliotecaDB;User Id=sa;Password=Biblioteca_2026;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models --context BibliotecaContext --no-onconfiguring --force
```

- `--no-onconfiguring` evita que se genere `OnConfiguring` (en la guía se borra a mano).
- `--force` sobrescribe `Models/` al regenerar. Los archivos propios están fuera de `Models/`
  (`Repositorios/`, controladores, vistas), por eso no se pierden.
- Atajo: `./scripts/scaffold.sh`. En Visual Studio sería
  `Scaffold-DbContext "<cadena>" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context BibliotecaContext -NoOnConfiguring -Force`.

## Estructura

```
Mvc.Net/
├── docker-compose.yml          SQL Server 2022
├── db/
│   ├── BibliotecaDB.sql        esquema + datos (Autor, Libro)
│   └── ModificarLibro.sql      ALTER TABLE para la demo "si la BD cambia"
├── scripts/                    cargar-bd.sh · scaffold.sh
└── Biblioteca.Web/
    ├── Models/                 generado con scaffold (Autor, Libro, BibliotecaContext)
    ├── Repositorios/           AutorRepositorio · LibroRepositorio (todo el LINQ)
    ├── Controllers/            Home · Autores · Libros
    └── Views/
```

El comando de scaffold listo para copiar en la presentación está en `scripts/comandos.txt`.

## Guion sugerido para la clase

1. Mostrar `db/BibliotecaDB.sql` y ejecutarlo (`./scripts/cargar-bd.sh`).
2. Crear el proyecto y paquetes: `dotnet new mvc -n Biblioteca.Web`,
   `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` y `...Design`.
3. Ejecutar el scaffold y revisar `Models/`: `Autors`, `Libros`, `IdAutorNavigation`.
4. Registrar `AddDbContext` y los repositorios (`AddScoped`) en `Program.cs`.
5. Recorrer `LibroRepositorio` (Where, Include, OrderBy, Select, Any, Count/Sum/Average/Max, Take, `AsNoTracking`).
6. Probar CRUD: ISBN duplicado (`AddModelError`), eliminar autor con libros, eliminación lógica.
7. Cambio de esquema: `./scripts/cargar-bd.sh ModificarLibro.sql` y regenerar con `--force`;
   aparece `Libro.Sinopsis` en `Models/`. (Para volver: `./scripts/cargar-bd.sh`.)

## Notas

- `ModelState.Remove(nameof(Libro.IdAutorNavigation))`: la propiedad de navegación no viene del formulario y,
  al ser no anulable, haría fallar `ModelState.IsValid`.
- Las reglas (formato de ISBN, precio, año, longitudes, duplicados) se validan en los controladores con
  `ModelState.AddModelError`; las vistas son fuertemente tipadas y se enlazan a las entidades.
- `Bind(...)` en las acciones POST evita que se modifiquen campos como `Activo` desde el formulario.
- Usa `net10.0`. Para .NET 8: cambiar `TargetFramework` y las versiones de los paquetes EF a `8.*`.
