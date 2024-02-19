CREATE TABLE evento
(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(255) NOT NULL,
    fecha_inicio DATETIME NOT NULL,
);

CREATE TABLE acreditado
(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    id_evento INT FOREIGN KEY REFERENCES evento(id),
    nombre NVARCHAR(51),
    apellido NVARCHAR(51),
    DNI NVARCHAR(51),
    CUIT NVARCHAR(51),
    habilitado BIT,
    celular NVARCHAR(51),
    grupo NVARCHAR(51),
	alta BIT,
);

CREATE TABLE ingreso
(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    id_acreditado INT FOREIGN KEY REFERENCES acreditado(id) NOT NULL,
    fecha_operacion DATETIME NOT NULL
);

CREATE TABLE egreso
(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    id_acreditado INT FOREIGN KEY REFERENCES acreditado(id) NOT NULL,
    fecha_operacion DATETIME NOT NULL
);