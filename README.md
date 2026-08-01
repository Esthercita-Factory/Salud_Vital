# SaludVital

Aplicacion de consola en C#/.NET para la gestion de la salud y el bienestar de mascotas. El proyecto es una solucion .NET organizada en dos proyectos: la aplicacion principal y su proyecto de pruebas.

## Requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) o superior
- Un sistema operativo compatible (Windows, Linux o macOS)

## Estructura del proyecto

```
SaludVital/
|-- SaludVital/                  # Proyecto principal (aplicacion de consola)
|   |-- Models/                  # Modelos de dominio
|   |   `-- Mascota.cs           # Modelo de Mascota
|   `-- Program.cs               # Punto de entrada de la aplicacion
|-- SaludVital.Tests/            # Proyecto de pruebas unitarias (xUnit)
|   `-- UnitTest1.cs             # Pruebas de ejemplo
|-- SaludVital.slnx              # Archivo de solucion
```

## Compilar y ejecutar

### Restaurar dependencias

```bash
dotnet restore
```

### Ejecutar la aplicacion

```bash
dotnet run --project SaludVital
```

### Ejecutar las pruebas

```bash
dotnet test
```

## Licencia

Este proyecto aun no cuenta con una licencia definida.
