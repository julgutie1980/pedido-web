# REVIEW.md · Reglas de revisión de Pedidos Web

Valen para toda revisión: la de la IA (subagente `revisor` y workflow `claude-review.yml`)
y la de una persona. Se leen siempre desde `main`. El contexto general del proyecto está en
AGENTS.md; si algo choca, manda AGENTS.md.

## Qué es Importante aquí (bloquea el merge)
- Dinero como `double` o `float`, o con decimales: en CLP va `decimal` sin decimales.
- Fechas de negocio con `DateTime` en vez de `DateOnly`.
- Secretos, claves, tokens o cadenas de conexión escritos en código, workflows o docs.
- Un criterio de aceptación de la spec (`specs/<historia>/spec.md`) sin su test, o un test que no prueba lo que dice.
- `src/PedidosWeb.Core` con dependencias de infraestructura (ASP.NET, archivos, red, base de datos).
- Cambios en `legado/` (solo lectura).

Estilo, nombres o refactors son, como mucho, Detalle.

## Revisar siempre
1. **Un test por criterio de aceptación**, con el nombre `Metodo_Escenario_ResultadoEsperado`, que falle si la regla se rompe.
2. **Bordes y entradas inválidas:** límites inclusivos o exclusivos según la spec, valores nulos o vacíos, rangos invertidos; cada error con un mensaje que explique el problema.
3. **Capas y nombres:** la lógica va en Core y los endpoints en Api (`Program.cs`); los nombres del dominio van en español.

## No reportar
- Lo que el CI ya valida: que compile y que los tests pasen.
- Formato, espacios u orden de `using` que no cambian el comportamiento.
- Preferencias personales sin un riesgo concreto.
- Código que el PR no tocó, salvo que el cambio lo rompa.

## Tope y evidencia
- Todo hallazgo Importante se reporta. Detalles: **máximo 5** por revisión.
- Cada afirmación sobre el comportamiento lleva su cita `archivo:línea`; sin evidencia, no hay hallazgo.
- Cada hallazgo dice: qué pasa, qué regla de este archivo o de AGENTS.md rompe, y una propuesta.
- Si no hay hallazgos, dilo en una línea. En español. Nunca escribas «@claude» (dispara `claude.yml`).
