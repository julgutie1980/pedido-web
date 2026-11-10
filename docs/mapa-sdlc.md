# Mapa de uso de IA en el SDLC · Pedidos Web

> Paso 4 del taller de la Unidad 1, a partir de `plantilla-mapa-sdlc.md`.
> Para cada etapa: qué tarea concreta se delega a Claude, qué artefacto queda versionado,
> quién decide y qué revisa, qué puede salir mal y en qué evento de Scrum ocurre.
> Modelo: el caso de Valentina y TurnoVet (`caso-scrum-y-sdd.pdf`).

**Autor:** Julián Gutiérrez
**Fecha:** 2026-11-09

| Etapa | Se delega a Claude | Artefacto versionado | Revisa una persona | Riesgo | Evento Scrum |
|---|---|---|---|---|---|
| Requisitos | Leer el código y las reglas de Pedidos Legado y proponer las reglas del negocio en una tabla con ejemplos (como R1–R5 de `FormatearFolio`). | `README.md` (sección de reglas y tabla de ejemplos). | El Product Owner confirma cada regla con el negocio; lo que no esté escrito se pregunta (regla 8 de `AGENTS.md`). | Que la IA invente o suponga una regla que el legado no tiene, y los folios dejen de coincidir con los pedidos antiguos. | Refinamiento del Product Backlog. |
| Diseño | Proponer la firma, la ubicación de la clase en `PedidosWeb.Core` y el plan de cambios en máximo 5 viñetas. | Firma documentada en el README y en el XML doc de la clase; reglas del equipo en `AGENTS.md`. | El desarrollador aprueba el plan antes de que se edite cualquier archivo. | Diseño acoplado a infraestructura, o uso de `double`/`float` para montos. | Sprint Planning. |
| Código | Implementar la función a partir de las reglas, solo dentro de `src/`. | `src/PedidosWeb.Core/Folios/FolioFormatter.cs`. | El desarrollador lee y entiende el diff completo antes del commit. | Código que pasa los casos base pero omite reglas (validación de A–Z, orden de validación R5). | Durante el Sprint (trabajo diario). |
| Pruebas | Escribir un test xUnit por cada regla y ejemplo del README, incluidos los casos de error. | `tests/PedidosWeb.Core.Tests/Folios/FolioFormatterTests.cs`. | El desarrollador verifica que cada fila de la tabla de ejemplos tenga su test y que `dotnet test` quede en verde. | Tests escritos para que pase la implementación de la IA y no para probar la regla del negocio. | Durante el Sprint; se comprueba contra la Definition of Done. |
| Revisión y despliegue | Resumir el diff y proponer el mensaje de commit. No hace commit, push ni merge (regla 3). | Rama `unidad-NN` en GitHub y su merge a `main`. | Una persona revisa el diff, hace el commit, el push y el merge. | Integrar cambios que nadie leyó, o archivos fuera de `src/`, `tests/` y `docs/`. | Sprint Review. |
| Mejora continua | Proponer ajustes a `AGENTS.md` y a la forma de trabajo según los errores encontrados. | `AGENTS.md` (lo edita una persona) y `docs/`. | El equipo decide qué reglas nuevas se agregan. | Reglas que crecen sin control o que nadie cumple. | Sprint Retrospective. |

## Evidencia del paso 3

| | Sin IA | Con Claude Code |
|---|---|---|
| Tiempo (min) | 45 | 15 |
| Errores encontrados | 4: aceptaba `Ñ` y letras con tilde en la sucursal; `null` provocaba `NullReferenceException` en vez de `ArgumentException`; validaba la sucursal antes que el número (incumplía R5); el código quedó sin depurar y con validaciones repetidas. | 1: los 3 tests base pasaban, pero faltaban los tests de R1, R2, R3 y R5, por lo que una regla olvidada no se habría detectado. |
| Qué tuve que corregir | Reordenar las validaciones (primero R1), validar A–Z de forma explícita, manejar `null` y vacío, y limpiar el código. | Revisar el diff y pedir los 12 tests que faltaban (uno por ejemplo de la tabla del README). Con eso quedaron validadas todas las reglas y `dotnet test` pasó 15/15. |
