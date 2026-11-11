# Defectos corregidos · migración del cálculo de totales

Regla: si un valor esperado de `migracion/golden-master.csv` cambia, ese caso aparece aquí.

## D1 · R05 · Redondeo al peso que baja los ,50 (CORREGIDO)

- **Qué hace el legado:** `Calc3` redondea con `Trunc(AValor + 0.49)`, así que un ,50 baja en vez de subir: 25.767,5 queda en 25.767 (`legado/src/UCalculoTotales.pas:48-51`). Se aplica al descuento (`:114`), al flete (`:98`, `:100`) y al IVA (`:105`). La misma función está copiada en `legado/src/UUtilidades.pas:30`.
- **Corrección:** el C# redondea al peso con `Math.Round(x, 0, MidpointRounding.AwayFromZero)`, es decir, ,50 sube (`src/PedidosWeb.Core/Totales/CalculoTotales.cs`, `RedondearAlPeso`).
- **Por qué:** es un redondeo no estándar que no coincide con lo que espera contabilidad ni con ninguna otra herramienta. En los casos de ,50 el flete y el IVA quedaban un peso por debajo, y el descuento del cliente también. El error es de un peso por monto, pero es sistemático.
- **Casos del golden master que cambian:** 13 (2, 6, 8, 18, 21, 23, 25, 27, 36, 37, 42, 46, 54).
  - 9 casos, por el flete de las regiones 11 y 12: 16.490 × 1,15 = 18.963,5 → 18.964 y 14.990 × 1,15 = 17.238,5 → 17.239. Son los casos 18, 21, 23, 27, 36, 37, 42, 46 y 54; el 21 cambia además el IVA.
  - 2 casos solo por el IVA (2 y 8).
  - 2 casos por el descuento (6 y 25). En estos el total **baja**, porque el descuento sube un peso.
- **Suma de la columna total:** 58.202.135 → 58.202.144 (+9).
- **Commit:** `ba3298a fix: R05 redondeo al peso con ,50 hacia arriba en el cálculo de totales (U5 · Etapa 3)`, separado del `b8bfe56 feat: cálculo de totales en C# igual al legado`.
- **Convivencia:** mientras el legado siga calculando, Pedidos Web y Pedidos Legado pueden diferir en **±1 peso por monto** (descuento, flete o IVA) en los pedidos con fracción ,50. Contabilidad debe aceptar esa tolerancia en la conciliación y clasificar estas diferencias como «R05», no como error.

## H1 · Umbrales del descuento por volumen: `>` en pantalla y `>=` al grabar (HALLAZGO, NO CORREGIDO)

- **Qué encontraste:** el cálculo que se ve en pantalla da descuento solo si la base **supera** el umbral (`if ABase > 1000000` / `> 500000`, `legado/src/UCalculoTotales.pas:77-79`). Al grabar, el formulario recalcula con `>=` (`legado/src/UFrmPedidos.pas:296`, `:298`) y además suma +2 % de Navidad si `gDescNavidad` está activo (`:304-305`). Lo que queda en la base de datos es el total del formulario (`:320`). La diferencia solo se avisa en modo prueba (`:327-331`). El comentario de la cabecera («5 % sobre 400.000», `UCalculoTotales.pas:4`) no coincide con ninguna de las dos versiones.
- **Efecto:** afecta a los pedidos con base de descuento **exactamente** de 500.000 o 1.000.000. En el golden master son 13 casos:
  - Con base de 500.000: casos 8, 11, 19, 23, 50 y 55. Al grabar reciben 25.000 más de descuento.
  - Con base de 1.000.000: casos 26, 32, 35, 39, 42, 48 y 60. Al grabar reciben 30.000 más.
  - En total son 360.000 de descuento de diferencia, más su IVA. El cliente ve un total en pantalla y se le factura otro.
- **Quién decide:** Comercial, junto con Contabilidad: qué umbral vale y si el descuento de Navidad sigue vigente. El C# replica `UCalculoTotales` (`>` y sin Navidad), que es el contrato del golden master. Si el negocio elige `>=`, se corrige como D2 con su propio commit y se actualizan esos 13 casos.
