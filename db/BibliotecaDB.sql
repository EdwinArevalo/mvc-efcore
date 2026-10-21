USE master;
GO

IF DB_ID(N'BibliotecaDB') IS NOT NULL
BEGIN
    ALTER DATABASE BibliotecaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BibliotecaDB;
END
GO

CREATE DATABASE BibliotecaDB;
GO

USE BibliotecaDB;
GO

/* ----------------------------- TABLAS ----------------------------------- */
CREATE TABLE Autor (
    IdAutor          INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Autor PRIMARY KEY,
    Nombre           NVARCHAR(100) NOT NULL,
    Nacionalidad     NVARCHAR(50)  NULL,
    Activo           BIT           NOT NULL
        CONSTRAINT DF_Autor_Activo DEFAULT 1
);
GO

CREATE TABLE Libro (
    IdLibro          INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Libro PRIMARY KEY,
    Isbn             CHAR(13)      NOT NULL
        CONSTRAINT UQ_Libro_Isbn UNIQUE,
    Titulo           NVARCHAR(150) NOT NULL,
    Precio           DECIMAL(8,2)  NOT NULL
        CONSTRAINT CK_Libro_Precio CHECK (Precio > 0),
    AnioPublicacion  INT           NOT NULL
        CONSTRAINT CK_Libro_Anio CHECK (AnioPublicacion BETWEEN 1400 AND 2100),
    IdAutor          INT           NOT NULL
        CONSTRAINT FK_Libro_Autor REFERENCES Autor (IdAutor),
    Activo           BIT           NOT NULL
        CONSTRAINT DF_Libro_Activo DEFAULT 1
);
GO

CREATE INDEX IX_Libro_Autor ON Libro (IdAutor);
GO

/* ----------------------------- DATOS DE PRUEBA -------------------------- */
INSERT INTO Autor (Nombre, Nacionalidad) VALUES
    (N'Mario Vargas Llosa',     N'Peruana'),
    (N'César Vallejo',          N'Peruana'),
    (N'José María Arguedas',    N'Peruana'),
    (N'Ricardo Palma',          N'Peruana'),
    (N'Gabriel García Márquez', N'Colombiana'),
    (N'Julio Cortázar',         N'Argentina'),
    (N'Isabel Allende',         N'Chilena'),
    (N'Jorge Luis Borges',      N'Argentina');
GO

-- Los ISBN son de ejemplo (no corresponden a las ediciones reales).
INSERT INTO Libro (Isbn, Titulo, Precio, AnioPublicacion, IdAutor) VALUES
    ('9780000000011', N'La ciudad y los perros',               59.90, 1963, 1),
    ('9780000000028', N'Conversación en La Catedral',          79.90, 1969, 1),
    ('9780000000035', N'La fiesta del Chivo',                  69.50, 2000, 1),
    ('9780000000042', N'Pantaleón y las visitadoras',          49.90, 1973, 1),
    ('9780000000059', N'Trilce',                               39.00, 1922, 2),
    ('9780000000066', N'Los heraldos negros',                  35.00, 1919, 2),
    ('9780000000073', N'Los ríos profundos',                   45.00, 1958, 3),
    ('9780000000080', N'Todas las sangres',                    65.00, 1964, 3),
    ('9780000000097', N'Tradiciones peruanas',                 55.00, 1872, 4),
    ('9780000000103', N'Cien años de soledad',                 74.90, 1967, 5),
    ('9780000000110', N'El amor en los tiempos del cólera',    68.00, 1985, 5),
    ('9780000000127', N'Crónica de una muerte anunciada',      42.00, 1981, 5),
    ('9780000000134', N'Rayuela',                              72.00, 1963, 6),
    ('9780000000141', N'Bestiario',                            38.50, 1951, 6),
    ('9780000000158', N'La casa de los espíritus',             64.90, 1982, 7),
    ('9780000000165', N'Paula',                                58.00, 1994, 7),
    ('9780000000172', N'Ficciones',                            47.50, 1944, 8),
    ('9780000000189', N'El Aleph',                             46.00, 1949, 8);
GO

SELECT a.Nombre AS Autor, COUNT(l.IdLibro) AS Libros
FROM Autor a LEFT JOIN Libro l ON l.IdAutor = a.IdAutor
GROUP BY a.Nombre
ORDER BY a.Nombre;
GO
