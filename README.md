# SaludVital

Aplicación web ASP.NET Core MVC para gestionar los pacientes de una **clínica
veterinaria** ("Patitas Felices"). Escrito íntegramente en español, buscando la
solución más simple que funcione.

## Tecnologías

| Pieza | Versión |
|---|---|
| .NET | 10.0 |
| ASP.NET Core MVC | 10.0 |
| Bootstrap | 5.3.x |
| xUnit | 2.9.3 |
| Docker | imágenes `sdk:10.0` y `aspnet:10.0` |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.1 |

## Cómo ejecutarla

```bash
dotnet run --project SaludVital
```

Luego abre <http://localhost:5159>. La raíz del sitio es directamente el
catálogo de mascotas.

> Los datos se guardan **en memoria**, así que los cambios se pierden al
> reiniciar la aplicación. Al arrancar siempre aparecen las diez mascotas de
> ejemplo.

## Cómo ejecutar las pruebas

```bash
dotnet test
```

Son 70 pruebas y tardan menos de dos segundos. Cubren el CRUD completo,
las validaciones, la búsqueda y el historial de consultas, así que no hace
falta comprobar nada a mano en el navegador.

## Funcionalidad

CRUD completo de mascotas:

| Acción | Ruta | Qué hace |
|---|---|---|
| Catálogo | `/` o `/Mascotas` | Rejilla de tarjetas con los pacientes |
| Ficha | `/Mascotas/Detalles/{id}` | Datos completos + historial de consultas |
| Crear | `/Mascotas/Crear` | Formulario de alta |
| Editar | `/Mascotas/Editar/{id}` | Formulario de modificación |
| Eliminar | `/Mascotas/Eliminar/{id}` | Página de confirmación antes de borrar |

Historial de consultas médicas (el equivalente al "registro de ventas" de una
tienda):

| Acción | Ruta | Qué hace |
|---|---|---|
| Nueva consulta | `/Consultas/Crear?mascotaId={id}` | Registra fecha, motivo, diagnóstico y tratamiento |
| Editar | `/Consultas/Editar/{id}` | Modifica una consulta |
| Eliminar | `/Consultas/Eliminar/{id}` | Confirmación antes de borrar |

Además:

- **Búsqueda** en el catálogo por nombre, especie, raza o dueño.
- **Modo claro y oscuro**, con botón en la cabecera. Si nunca eliges, sigue la
  preferencia del sistema operativo.
- **Validación** en cliente y servidor, con todos los mensajes en español.
- **Indicador de estado** de cada mascota (Activo/Inactivo).

## Estructura

```
SaludVital/
├── Models/
│   ├── Mascota.cs               Entidad con campos extra y validaciones
│   ├── Consulta.cs              Historial de consultas médicas
│   ├── SinNumerosAttribute.cs   Validación propia (nombres sin dígitos)
│   └── ErrorViewModel.cs        Página de error
├── Infra/
│   └── DecimalModelBinder.cs    Enlace de decimales con coma y punto
├── Repositories/
│   ├── IRepositorioMascotas.cs / RepositorioMascotasEnMemoria.cs
│   └── IRepositorioConsultas.cs / RepositorioConsultasEnMemoria.cs
├── Controllers/
│   ├── MascotasController.cs    CRUD + búsqueda
│   ├── ConsultasController.cs   CRUD del historial
│   └── HomeController.cs        Sólo la página de error
├── Views/
│   ├── Mascotas/  Index, Detalles, Crear, Editar, Eliminar, _CamposMascota
│   └── Consultas/ Crear, Editar, Eliminar, _CamposConsulta
├── wwwroot/css/site.css         Estilos propios (prefijo sv-)
└── Program.cs                   Configuración y arranque

SaludVital.Tests/
├── RepositorioMascotasEnMemoriaTests.cs   13 pruebas
├── RepositorioConsultasEnMemoriaTests.cs  10 pruebas
├── MascotaValidacionTests.cs              12 pruebas
├── ConsultaValidacionTests.cs              6 pruebas
├── MascotasControllerTests.cs             10 pruebas
├── ConsultasControllerTests.cs             7 pruebas
└── SaludVitalIntegracionTests.cs          12 pruebas
```

## Docker

La aplicación va empaquetada en un contenedor con un `Dockerfile` de tres
etapas:

1. `compilacion` publica con el SDK completo.
2. `pruebas` parte de la anterior y ejecuta `dotnet test`; sólo se construye si
   se pide por su nombre (`target: pruebas`).
3. `final` parte del runtime de ASP.NET Core y sólo copia la carpeta ya
   publicada, así que la imagen final no lleva ni compilador ni código fuente.

Levantarla en tu máquina:

```bash
docker compose up -d --build
```

Queda escuchando en `127.0.0.1:8080`, sólo desde tu propio equipo. Para
comprobar que está viva:

```bash
curl http://127.0.0.1:8080/salud    # responde: Sana
```

Otros comandos útiles:

```bash
docker compose logs -f      # ver los logs en vivo
docker compose down         # pararla y borrar el contenedor
```

Para ejecutar las pruebas dentro de un entorno limpio:

```bash
docker compose -f compose.pruebas.yaml up --build
```

El contenedor arranca con `ASPNETCORE_ENVIRONMENT=Production` y corre como el
usuario sin privilegios `app` que trae la imagen de Microsoft.

## Decisiones de diseño

**Almacenamiento en memoria.** No hay base de datos todavía: los repositorios
guardan las mascotas en listas. Están detrás de interfaces
(`IRepositorioMascotas`, `IRepositorioConsultas`), así que migrar a Entity
Framework Core es cambiar el registro en `Program.cs`.

**Los repositorios se registran como `AddSingleton`.** Con `AddScoped` habría
una lista nueva en cada petición y todo cambio se perdería al instante.

**Eliminar ocurre en dos pasos.** El `GET` de `Eliminar` sólo muestra la
confirmación; el borrado real lo hace el `POST` a `ConfirmarEliminar`. Nunca se
destruyen datos en respuesta a un `GET`.

**Acciones en español, salvo `Index`.** `Index` mantiene el nombre en inglés
porque es la acción por defecto de la ruta, y así `/` y `/Mascotas` funcionan
sin configuración extra. La ruta por defecto apunta a `Mascotas`, de modo que la
raíz del sitio es directamente el catálogo.

**Cultura fija `es-CO`.** Los decimales se muestran con coma (`10,5 kg`).

**El enlace de `decimal` acepta coma y punto.** El peso se enlaza según el
separador que traiga el formulario: `10,5` se interpreta con la cultura activa
y `10.5` con la invariante. Así el servidor no confunde el punto como separador
de miles (el error típico de `es-CO`). La prueba
`ElPesoConComaDecimal_SeInterpretaBien` fija este comportamiento.

**`PesoEnKg` es `decimal`, no `double`.** `double` es binario y arrastra
errores de redondeo, inaceptables para medidas clínicas.

**Estilos propios sobre Bootstrap.** `site.css` define la paleta con variables
CSS y se apoya en el atributo `data-bs-theme` de Bootstrap 5.3, de modo que los
componentes nativos y los estilos propios cambien de tema a la vez. Las clases
propias llevan el prefijo `sv-` para no chocar con Bootstrap.

## Próximos pasos

- [ ] Persistencia real con Entity Framework Core — **bloquea el uso serio en
      producción**: hoy cada reinicio vuelve a las mascotas de ejemplo
- [ ] Registro de ventas de servicios (consulta, vacunación, hospitalización)
- [ ] Filtros por especie y estado en el catálogo
- [ ] Más campos en `Consulta`: veterinario responsable, temperatura, peso del
      día
- [ ] Autenticación para distinguir dueños de veterinarios

## Licencia

Este proyecto aún no cuenta con una licencia definida.
