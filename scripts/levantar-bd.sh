#!/usr/bin/env bash
# Levanta SQL Server en Docker y espera a que esté listo.
# La base BibliotecaDB la crea EF Core al aplicar las migraciones (dotnet ef database update).
set -euo pipefail
cd "$(dirname "$0")/.."

docker compose up -d

echo "Esperando a que SQL Server esté listo..."
for i in $(seq 1 40); do
  estado=$(docker inspect -f '{{.State.Health.Status}}' biblioteca-sqlserver 2>/dev/null || echo "starting")
  [ "$estado" = "healthy" ] && break
  sleep 3
done
[ "$estado" = "healthy" ] || { echo "SQL Server no arrancó. Revisa: docker logs biblioteca-sqlserver"; exit 1; }

echo "Listo: SQL Server en localhost,1433."
