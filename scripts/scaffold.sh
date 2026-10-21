#!/usr/bin/env bash
# Database First: genera/regenera Models/ a partir de BibliotecaDB.
# Es el mismo comando que se muestra en clase (dotnet ef dbcontext scaffold).
set -euo pipefail
cd "$(dirname "$0")/../Biblioteca.Web"

dotnet tool restore
dotnet ef dbcontext scaffold \
  "Server=localhost,1433;Database=BibliotecaDB;User Id=sa;Password=Biblioteca_2026;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models \
  --context BibliotecaContext \
  --no-onconfiguring \
  --force
