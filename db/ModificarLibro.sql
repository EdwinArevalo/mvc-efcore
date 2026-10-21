-- Demo "Si la base de datos cambia": ejecutar DESPUÉS de la demo principal,
-- luego regenerar el modelo (scripts/scaffold.sh) y mostrar la nueva propiedad.
USE BibliotecaDB;
GO

ALTER TABLE Libro ADD Sinopsis NVARCHAR(300) NULL;
GO
