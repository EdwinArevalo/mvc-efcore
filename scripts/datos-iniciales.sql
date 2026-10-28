USE BibliotecaDB;
GO

IF NOT EXISTS (SELECT 1 FROM Autor)
INSERT INTO Autor (Nombre, Nacionalidad, Activo) VALUES
    (N'Mario Vargas Llosa',     N'Peruana',    1),
    (N'César Vallejo',          N'Peruana',    1),
    (N'José María Arguedas',    N'Peruana',    1),
    (N'Ricardo Palma',          N'Peruana',    1),
    (N'Gabriel García Márquez', N'Colombiana', 1),
    (N'Julio Cortázar',         N'Argentina',  1),
    (N'Isabel Allende',         N'Chilena',    1),
    (N'Jorge Luis Borges',      N'Argentina',  1);
GO

IF NOT EXISTS (SELECT 1 FROM Libro)
INSERT INTO Libro (Isbn, Titulo, Precio, AnioPublicacion, IdAutor, Activo)
SELECT v.Isbn, v.Titulo, v.Precio, v.Anio, a.IdAutor, 1
FROM (VALUES
    ('9780000000011', N'La ciudad y los perros',            59.90, 1963, N'Mario Vargas Llosa'),
    ('9780000000028', N'Conversación en La Catedral',       79.90, 1969, N'Mario Vargas Llosa'),
    ('9780000000035', N'La fiesta del Chivo',               69.50, 2000, N'Mario Vargas Llosa'),
    ('9780000000042', N'Pantaleón y las visitadoras',       49.90, 1973, N'Mario Vargas Llosa'),
    ('9780000000059', N'Trilce',                            39.00, 1922, N'César Vallejo'),
    ('9780000000066', N'Los heraldos negros',               35.00, 1919, N'César Vallejo'),
    ('9780000000073', N'Los ríos profundos',                45.00, 1958, N'José María Arguedas'),
    ('9780000000080', N'Todas las sangres',                 65.00, 1964, N'José María Arguedas'),
    ('9780000000097', N'Tradiciones peruanas',              55.00, 1872, N'Ricardo Palma'),
    ('9780000000103', N'Cien años de soledad',              74.90, 1967, N'Gabriel García Márquez'),
    ('9780000000110', N'El amor en los tiempos del cólera', 68.00, 1985, N'Gabriel García Márquez'),
    ('9780000000127', N'Crónica de una muerte anunciada',   42.00, 1981, N'Gabriel García Márquez'),
    ('9780000000134', N'Rayuela',                           72.00, 1963, N'Julio Cortázar'),
    ('9780000000141', N'Bestiario',                         38.50, 1951, N'Julio Cortázar'),
    ('9780000000158', N'La casa de los espíritus',          64.90, 1982, N'Isabel Allende'),
    ('9780000000165', N'Paula',                             58.00, 1994, N'Isabel Allende'),
    ('9780000000172', N'Ficciones',                         47.50, 1944, N'Jorge Luis Borges'),
    ('9780000000189', N'El Aleph',                          46.00, 1949, N'Jorge Luis Borges')
) AS v (Isbn, Titulo, Precio, Anio, Autor)
JOIN Autor a ON a.Nombre = v.Autor;
GO
