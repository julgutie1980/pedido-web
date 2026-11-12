# Checklist de revisión · pedidos-web

> Completada en la etapa 3 del taller U7, adaptada: sin pareja (auto-revisión), revisión de IA en local
> con el subagente `revisor` y merge local con push a `main` (sin PR en GitHub ni secreto de Claude en Actions).

## Antes de pedir revisión
- [x] `dotnet test` en verde y un test por criterio de aceptación de la spec, con nombre `Metodo_Escenario_ResultadoEsperado`.
- [x] Corrí el subagente `revisor` sobre la rama (reglas leídas desde `main`) y decidí qué corregir antes.
- [x] La descripción tiene Qué cambia, Por qué (mío) y Cómo probarlo, sin `[COMPLETAR]`.
- [x] El cambio tiene un solo propósito y respeta el «Fuera de alcance» de la spec.

## Al revisar (persona)
- [x] ¿Qué casos válidos del negocio no tienen test aunque el código «parezca» correcto? (ej.: rango de un solo día).
- [x] ¿El comportamiento ante entradas raras es el que el negocio espera, no solo el que compila?

## Con los hallazgos de la IA
- [x] Cada hallazgo tiene respuesta: acepto, discutir o descarto, con argumento.
- [x] Descartar vale si cito la regla o la spec (`archivo:línea`); lo que queda fuera se anota para otro PR.

## Merge
- [x] Tests en verde después de aplicar lo acordado y el revisor vuelve a dar «Listo para PR».
- [x] CI en verde en `main` después del push.

## Mi PR de HU-03
- **PR:** sin PR en GitHub (merge local). Rama `hu-03-pedidos-por-fecha`, registro en `docs/calidad/revision-hu-03.md`.
- **Hallazgo de la IA que más discutimos:** duplicación de la regla de rango con `ListadoPedidos.cs:69`. Descartado en este PR: la spec deja fuera de alcance otras historias y la guía pide PR chico; se unifica en un PR propio.
- **Mi hallazgo humano:** el rango de un solo día (`desde == hasta`) no tenía test; un cambio de `>` a `>=` en la validación lo habría roto sin que fallara nada. Se agregó `EntreFechas_DesdeIgualAHasta_DevuelveLosPedidosDeEseDia`.
