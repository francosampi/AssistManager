CREATE DATABASE IF NOT EXISTS AsistManager;
USE AsistManager;

CREATE TABLE IF NOT EXISTS evento (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    fecha_inicio DATETIME NOT NULL
);

CREATE TABLE IF NOT EXISTS acreditado (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    id_evento INT,
    nombre VARCHAR(51),
    apellido VARCHAR(51),
    DNI VARCHAR(51),
    CUIT VARCHAR(51),
    habilitado BIT,
    celular VARCHAR(51),
    grupo VARCHAR(51),
    alta BIT,
    FOREIGN KEY (id_evento) REFERENCES evento(id)
);

CREATE TABLE IF NOT EXISTS ingreso (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    id_acreditado INT NOT NULL,
    fecha_operacion DATETIME NOT NULL,
    FOREIGN KEY (id_acreditado) REFERENCES acreditado(id)
);

CREATE TABLE IF NOT EXISTS egreso (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    id_acreditado INT NOT NULL,
    fecha_operacion DATETIME NOT NULL,
    FOREIGN KEY (id_acreditado) REFERENCES acreditado(id)
);
