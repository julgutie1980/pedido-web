# Matriz de decisión de Pedidos Legado

<!--
Etapa 3 · Decidir (modo por defecto).
1. Reparte los pesos entre los 5 criterios. Deben sumar 100 %.
2. Puntúa cada estrategia de 1 a 5 para cada módulo (5 = la mejor opción para ese criterio)
   y calcula el puntaje ponderado. Pídele a Claude que haga las cuentas, y revísalas.
3. La recomendación de cada módulo debe salir de la matriz, no de la intuición.
Punto de control E3: pesos que suman 100 %, al menos 4 módulos puntuados,
recomendación que se desprende de la matriz y resumen de 5 líneas.
-->

## Pesos

| Criterio | Qué mide | Peso |
|---|---|---|
| Costo | Cuánto cuesta en horas y dinero | 20 % |
| Complejidad | Qué tan difícil es técnicamente | 15 % |
| Riesgo | Qué puede salir mal en el camino | 30 % |
| Impacto | Cuánto valor entrega al negocio | 25 % |
| Personas | Conocimiento disponible en el equipo | 10 % |
| **Total** |  | **100 %** |

Por qué estos pesos:
- **Riesgo (30 %)**: 9 de los 15 riesgos de [riesgos.md](riesgos.md) son de prioridad Alta (inyección SQL, clave en el código, pedido sin transacción, folio MAX+1).
- **Impacto (25 %)**: la toma de pedidos es lo que vende.
- **Costo (20 %)**: el sistema es chico (unas 2.375 líneas), así que no es lo que decide.
- **Complejidad (15 %)**: la lógica es simple; lo difícil es separarla de los formularios.
- **Personas (10 %)**: Delphi 7 es conocimiento escaso y el equipo trabaja en .NET. Pesa menos porque afecta a todos los módulos casi por igual.

Escala: 5 = la mejor opción para ese criterio (Costo 5 = barato, Complejidad 5 = simple, Riesgo 5 = poco riesgo, Impacto 5 = mucho valor, Personas 5 = el equipo sabe hacerlo).
Estrategias:
- **Mantener**: dejar el código Delphi tal cual.
- **Refactorizar**: corregirlo dentro de Delphi.
- **Modernizar**: llevarlo a PedidosWeb (.NET 10, `decimal`, xUnit) conservando las reglas.
- **Reemplazar**: cambiarlo por un producto o servicio estándar (ERP, CRM, BI).

## Matriz

<!-- Anota el puntaje ponderado de cada estrategia (1–5 por criterio × peso). -->

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar | Recomendación |
|---|---|---|---|---|---|
| M1 Cálculo de totales (UCalculoTotales, UUtilidades · 336 líneas) | 2,80 | 3,00 | **4,35** | 2,50 | **Modernizar** |
| M2 Toma de pedidos (UFrmPedidos · 646 líneas) | 2,50 | 2,75 | **3,65** | 2,40 | **Modernizar** |
| M3 Stock (UStock · 93 líneas) | 2,50 | 3,00 | **3,75** | 2,15 | **Modernizar** |
| M4 Maestros (UFrmClientes, UFrmProductos · 631 líneas) | 2,80 | 3,00 | **3,90** | 2,80 | **Modernizar** |
| M5 Informes (UFrmInformes · 297 líneas) | 3,10 | 3,00 | 3,50 | **3,80** | **Reemplazar** |
| M6 Datos y globales (UDMPrincipal, UGlobales · 206 líneas) | 2,50 | 3,40 | **3,50** | 2,75 | **Modernizar** (ver nota) |

UFrmPrincipal (el menú, 166 líneas) queda fuera de la matriz: el portal web lo reemplaza con su propia navegación.

**Nota M6:** Modernizar le gana a Refactorizar por solo 0,10, menos que el margen de error de los puntajes. Se respeta la matriz, pero mientras convivan los dos sistemas conviene un arreglo puntual en el legado: sacar la clave del código (RS-02) y parametrizar `EjecutarSQL` (RS-01).

**Orden sugerido por puntaje:** M1 (4,35) → M4 (3,90) → M3 (3,75) → M2 (3,65) → M6 (3,50). M5 se reemplaza en paralelo.

### Detalle de puntajes

Fórmula: Costo × 0,20 + Complejidad × 0,15 + Riesgo × 0,30 + Impacto × 0,25 + Personas × 0,10.
Columnas: **C** = Costo, **X** = Complejidad, **R** = Riesgo, **I** = Impacto, **P** = Personas.

**M1 Cálculo de totales**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 2 | 1 | 2 | 2,80 | No cuesta nada, pero deja vivos el `>` contra `>=` (RS-08) y el `Double` (RS-10). |
| Refactorizar | 4 | 4 | 3 | 2 | 2 | 3,00 | Se puede unificar en Delphi, pero no deja nada para el portal. |
| Modernizar | 4 | 4 | 4 | 5 | 5 | 4,35 | Es lógica pura, sin BD ni UI, y se puede probar con xUnit. `CalculadoraTotalPedido` ya tiene el subtotal y pasa a `decimal`. |
| Reemplazar | 2 | 3 | 2 | 3 | 3 | 2,50 | Son reglas propias de la empresa (OFE, VIP, regiones 11 y 12); un producto estándar no las trae. |

**M2 Toma de pedidos**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 1 | 1 | 2 | 2,50 | Es donde se concentran los riesgos Alta (RS-01, RS-04, RS-05). |
| Refactorizar | 3 | 2 | 3 | 3 | 2 | 2,75 | 420 líneas con la lógica dentro del formulario: es caro de arreglar en Delphi. |
| Modernizar | 2 | 2 | 4 | 5 | 5 | 3,65 | Es caro, pero resuelve inyección, transacción y folio por diseño, y es el corazón del portal. |
| Reemplazar | 1 | 2 | 2 | 4 | 3 | 2,40 | Un ERP cuesta mucho y perdería reglas como el corte de las 18:00, el mínimo de 30.000 y la deuda a 60 días. |

**M3 Stock**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 1 | 1 | 2 | 2,50 | Los movimientos se pierden sin aviso (RS-06) y dos ventas simultáneas pueden pisar el stock (RS-07). |
| Refactorizar | 4 | 4 | 3 | 2 | 2 | 3,00 | Son 93 líneas: crear MOV_STOCK y usar `STOCK = STOCK - n` es barato. |
| Modernizar | 3 | 3 | 4 | 4 | 5 | 3,75 | Queda en la misma transacción que el pedido y con tests de la regla VIP −20. |
| Reemplazar | 1 | 2 | 2 | 3 | 3 | 2,15 | Un sistema de bodega (WMS) es desproporcionado para 93 líneas. |

**M4 Maestros**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 2 | 1 | 2 | 2,80 | Con RutOk invertido (RS-09) hay clientes que no se pueden encontrar al tomar el pedido. |
| Refactorizar | 4 | 4 | 3 | 2 | 2 | 3,00 | Basta con hacer que RutOk use ValidaRut, pero el resto sigue en Delphi. |
| Modernizar | 3 | 4 | 4 | 4 | 5 | 3,90 | Es un CRUD simple, `ClientesService` ya existe y queda una sola validación de RUT. |
| Reemplazar | 2 | 3 | 3 | 3 | 3 | 2,80 | Un CRM o ERP podría tener los maestros, pero habría que integrarlo con los pedidos. |

**M5 Informes**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 3 | 1 | 2 | 3,10 | Sus riesgos son de prioridad Media (RS-11, RS-12) y solo lo usa jefatura. |
| Refactorizar | 4 | 4 | 3 | 2 | 2 | 3,00 | Arreglar el IVA y el año en Delphi aporta poco. |
| Modernizar | 3 | 3 | 4 | 3 | 5 | 3,50 | Rehacerlo a mano en el portal es más trabajo que usar una herramienta. |
| Reemplazar | 3 | 4 | 4 | 4 | 4 | 3,80 | Una herramienta de BI o una exportación desde la API da filtros y CSV sin código propio. |

**M6 Datos y globales**

| Estrategia | C | X | R | I | P | Ponderado | Por qué |
|---|---|---|---|---|---|---|---|
| Mantener | 5 | 5 | 1 | 1 | 2 | 2,50 | Clave en el código (RS-02) y `EjecutarSQL` recibe SQL ya armado (RS-01). |
| Refactorizar | 4 | 3 | 4 | 3 | 2 | 3,40 | Sacar la clave del código y parametrizar protege al legado mientras siga en uso. |
| Modernizar | 3 | 3 | 4 | 3 | 5 | 3,50 | Repositorios (`IPedidoRepositorio`) y secretos fuera del repo, como pide AGENTS.md. |
| Reemplazar | 2 | 2 | 3 | 3 | 4 | 2,75 | Cambiar DBISAM por otro motor sin tocar el código no resuelve las globales (RS-15). |

## Resumen ejecutivo

<!-- Exactamente 5 líneas, para alguien que no leerá el resto. -->

1. **Qué es y su estado:** Pedidos Legado es Delphi 7 + DBISAM, 16 archivos y unas 2.375 líneas en `legado/src/`. Las reglas viven dentro de los formularios y el README está desactualizado (dice Delphi 5/Paradox).
2. **Hallazgo principal (con evidencia):** las reglas de dinero están duplicadas y no coinciden: el descuento usa `>` en `UCalculoTotales.pas:77-79` y `>=` en `UFrmPedidos.pas:296-298`, y el RUT tiene 0 y K invertidos entre `UFrmClientes.pas:193-196` y `UUtilidades.pas:73-76`.
3. **Riesgo más urgente:** SQL armado con lo que escribe el usuario (RS-01), clave de la BD en el código (RS-02) y pedido grabado sin transacción con folio MAX+1 (RS-04, RS-05).
4. **Estrategia y por dónde empezar:** modernizar hacia PedidosWeb módulo a módulo, empezando por el cálculo de totales (4,35), que es lógica pura y ya está empezada en `CalculadoraTotalPedido`. Los informes se reemplazan por una herramienta de BI o exportación.
5. **Próximo paso que se pide aprobar:** una historia y su spec para llevar descuento, flete e IVA a `CalculadoraTotalPedido` con xUnit, después de que negocio confirme qué regla vale (`>` o `>=`, y si el descuento de Navidad sigue).
