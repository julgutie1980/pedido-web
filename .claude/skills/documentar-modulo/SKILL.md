---
name: documentar-modulo
description: "Documenta un módulo, servicio o clase de PedidosWeb para el equipo y deja el resultado en docs/modulos/<Modulo>.md con propósito, API pública, ejemplo de uso y diagrama Mermaid. Úsala cuando pidan documentar, explicar o describir un módulo, servicio o clase del proyecto para el equipo, aunque no nombren la Skill."
---

# Documentar un módulo

## Pasos
1. Identifica el módulo pedido y lee su código en `src/PedidosWeb.Core/<Area>/` y sus tests en `tests/PedidosWeb.Core.Tests/<Area>/`.
2. Lista la API pública (constructores y métodos `public`): qué recibe, qué devuelve y qué pasa en los casos borde, según el código y los tests.
3. Escribe `docs/modulos/<Modulo>.md` con las cuatro secciones de abajo; crea `docs/modulos/` si no existe.
4. Revisa que cada afirmación coincida con el código y muestra el archivo al usuario.

## Qué debe tener docs/modulos/<Modulo>.md
- Propósito
- API pública
- Ejemplo de uso
- Diagrama Mermaid

## Reglas
- En español, para el equipo: frases cortas y nombres tal como están en el código.
- Documenta lo que el código hace hoy; no inventes comportamiento ni propongas cambios.
- No modifiques código ni tests: solo escribes en `docs/modulos/`.
- El ejemplo de uso sale de los tests (datos y resultados reales).
- Sin secretos, claves ni cadenas de conexión.
- No hagas commit: lo decide el equipo.
