# Plan de migración · Pedidos Legado → Pedidos Web

Estrategia: **strangler fig**, un módulo a la vez, en el orden de la matriz de decisión de la U4 (`docs/diagnostico/matriz-decision.md`). Cada etapa parte con un golden master del legado y no avanza hasta tenerlo en verde.

## 1. Etapas

1. **Cálculo de totales en C#** (M1, puntaje 4,35, `src/PedidosWeb.Core/Totales/`). Hecho en la Unidad 5: 60 de 60 casos en verde y R05 corregido.
2. **Exponer el cálculo** en la API (`POST /api/pedidos/totales`), para que el portal y la conciliación usen el mismo código.
3. **Maestros de clientes y productos** (M4, 3,90). Antes de migrar, se unifica la validación de RUT (RS-09).
4. **Stock** (M3, 3,75). Movimientos registrados y actualizaciones atómicas (RS-06, RS-07).
5. **Toma y grabado de pedidos** (M2, 3,65). Transacción y folio seguro (RS-04, RS-05). El grabado usa `CalculoTotales`, con lo que se elimina el cálculo duplicado.
6. **Datos y globales** (M6, 3,50). Mientras tanto, en el legado se saca la clave del código y se parametriza el SQL (RS-01, RS-02).
- En paralelo, los **informes** (M5) se reemplazan por exportación o BI.

## 2. Convivencia

- El legado sigue siendo el **sistema de registro** hasta el corte de cada etapa. El portal calcula en paralelo y no graba.
- Un proceso nocturno toma los pedidos del día del legado (`PEDIDOS`, `PEDIDO_LINEAS`), los recalcula con `CalculoTotales` y compara descuento, flete, IVA y total, pedido por pedido.
- Se aceptan dos tipos de diferencia explicada, documentados en `defectos-corregidos.md`:
  - **±1 peso por monto** debido a R05 (D1).
  - **Umbrales** H1, mientras el negocio no decida.
- Cualquier otra diferencia es un defecto: se investiga y, si corresponde, se convierte en un caso nuevo del golden master.

## 3. Riesgos

| Riesgo | Mitigación |
|---|---|
| Reglas de descuento duplicadas que no coinciden: `>` frente a `>=` y Navidad (RS-08, H1) | Decisión de Comercial y Contabilidad antes de la etapa 5. Hasta entonces, el C# replica `UCalculoTotales` y los 13 casos de umbral quedan identificados en la conciliación. |
| Dinero en `Double` y montos que entran al SQL con `FloatToStr` (RS-10) | Todo el C# usa `decimal`. La conciliación nocturna detecta cualquier deriva y el golden master fija el contrato. |
| Diferencias de ±1 peso por R05 durante la convivencia (D1) | Tolerancia acordada con Contabilidad y diferencias clasificadas como «R05» en el informe de conciliación. |
| Pedido grabado sin transacción y folio con MAX+1 (RS-04, RS-05) | No se migra el grabado hasta la etapa 5, con transacción y una secuencia para el folio. Mientras tanto, la conciliación avisa de los pedidos con total 0. |
| Un cambio en el C# rompe el contrato sin que nadie lo note | Hook `PostToolUse` con `dotnet test` en cada edición y golden master en CI (U6). El CSV no se edita para que un test pase. |

## 4. Criterio de corte

- **Corte de una etapa:** 20 días hábiles seguidos de conciliación con **0 diferencias sin explicar**, más la aprobación de Contabilidad. Recién entonces el portal pasa a ser el sistema de registro para ese módulo, y el legado lo deja en solo lectura.
- **Vuelta atrás:** el legado no se apaga hasta el corte de la última etapa. Si aparece una diferencia no explicada después del corte, se reactiva el flujo del legado para ese módulo (un cambio de configuración), se agrega el caso al golden master y se vuelve a cortar cuando esté en verde.
