# AGENTS.md · Pedidos Web

## Proyecto
Portal web de pedidos de una distribuidora; moderniza Pedidos Legado (Delphi + DBISAM).
Hoy: biblioteca de dominio y API REST en ASP.NET Core (.NET 10) con 45 pedidos de ejemplo en memoria.

## Comandos
- Compilar: `dotnet build`
- Tests: `dotnet test`
- Levantar la API: `dotnet run --project src/PedidosWeb.Api` (http://localhost:5080, `GET /salud`)

## Estilo de código
- Nombres del dominio en español (`FormatearFolio`, `PedidoRepositorioEnMemoria`, `ObtenerPorId`).
- Dinero en pesos chilenos: `decimal` sin decimales; nunca `double` ni `float`.
- Fechas sin hora: `DateOnly`.
- `src/PedidosWeb.Core`: modelos, reglas y repositorios, sin dependencias de infraestructura.
- `src/PedidosWeb.Api`: solo endpoints (Minimal API en `Program.cs`) que usan Core.

## Tests
- xUnit, en `tests/PedidosWeb.Core.Tests/`, una carpeta por área y un archivo `<Clase>Tests.cs` por clase.
- Nombre del test: `Metodo_Escenario_ResultadoEsperado`.
- Se ejecutan con `dotnet test` después de cada cambio y antes de dar algo por terminado.

## Seguridad
- Nunca secretos, claves ni cadenas de conexión en el código ni en el repo (`.env` y `*.pem` están en `.gitignore`).

## Reglas de trabajo
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Un test xUnit por criterio de aceptación.
- No hacer commit sin tests en verde.
- Sin preguntar puedes: leer y explorar el repo, compilar, ejecutar tests y editar `src/`, `tests/`, `docs/` y `specs/`.
- Pregunta siempre antes de: hacer commit, push o merge; agregar paquetes NuGet; crear o cambiar de rama;
  tocar archivos fuera de `src/`, `tests/`, `docs/` y `specs/`.
