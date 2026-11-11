# Spec breve · HU-03

> Hecha desde `docs/plantilla-spec-breve.md`. Estado: aprobada por el Product Owner (Julián Gutiérrez) el 2026-11-11.

## HU-03 · Cálculo de totales igual al legado

**Historia:** Como contabilidad, quiero que el portal calcule los totales del pedido igual que Pedidos Legado, para que ningún cliente pague un peso distinto al migrar.

## Qué hace

`CalculoTotales.Calcular` (en `PedidosWeb.Core.Totales`) replica `CalcularTotales` de `legado/src/UCalculoTotales.pas`:

- **Entrada:** las líneas del pedido (cantidad, precio unitario y categoría), si el cliente es VIP, la región, el flete base de la región y la tasa de IVA.
- **Salida:** subtotal, base del descuento, % de descuento, descuento, flete, neto, IVA y total.

Los montos son pesos chilenos (CLP) en `decimal`. El contrato es `migracion/golden-master.csv`: 60 casos, uno por pedido, con lo que calcula hoy el legado, **defectos incluidos**.

## Criterios de aceptación

- **CA-01 · Igual al legado** · Dado cada uno de los 60 casos de `migracion/golden-master.csv`, cuando calculo sus totales, entonces el descuento, el flete, el IVA y el total son iguales a los del legado, peso a peso.

## Restricciones

- La lógica vive en `PedidosWeb.Core`, sin dependencias de infraestructura.
- El CSV **no se edita para que un test pase**. Solo cambia al corregir un defecto y cada caso que cambia se registra en `docs/migracion/defectos-corregidos.md`.
- No se toca `legado/` ni `PedidosWeb.sln`.
- Se construye en 3 etapas, con un commit por etapa: congelar (tests en rojo), migrar (igual al legado) y corregir R05 (commit aparte).

## Fuera de alcance

- Exponer el cálculo en la API y guardarlo en una base de datos.
- Corregir la diferencia de umbrales entre pantalla (`>=`) y cálculo (`>`): se registra y la decide el negocio.
