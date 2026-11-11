# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno. La U5 cerró con 115 de 115 tests en verde.

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** CalculoTotales, el de la misión del IVA. Los tests están en `tests/PedidosWeb.Core.Tests/Totales/CalculoTotalesBordeTests.cs`.
- **Tests que propuso Claude:** 5 casos de borde que el golden master no cubre, cada uno con un valor esperado calculado a mano.
- **Cambios que hice para probarlos** (uno por test, en `src/PedidosWeb.Core/Totales/CalculoTotales.cs`, deshechos al terminar):
  - **R1:** flete sin la condición `lineas.Count == 0`. Falla `Calcular_SinLineas_TodoEnCero`. El golden master **no** la detecta.
  - **R2:** `BaseDescuento` sin `Trim().ToUpperInvariant()`. Falla `Calcular_CategoriaOfeEnMinusculasConEspacios_NoEntraEnBaseDescuento`. El golden master **no** la detecta.
  - **R3:** IVA con `* 0.19m` fijo en vez de `* tasa`. Falla `Calcular_TasaIvaDistinta_UsaLaTasaRecibida`, que da 19.665 en vez de 10.350. El golden master **no** la detecta, porque siempre usa 0,19. Es la falla del viernes de la misión.
  - **R4:** descuento sobre `subtotal` en vez de sobre `baseDescuento`. Falla `Calcular_VipSoloConOfertas_DescuentoCero`, que da 2.159 en vez de 0. También fallan 13 casos del golden master.
  - **R5:** `RedondearAlPeso` con `Math.Ceiling`. Falla `Calcular_DescuentoBajoMedioPeso_RedondeaHaciaAbajo`, que da 30.001 en vez de 30.000. También fallan 26 casos del golden master.
  - R1 y R2 las hice a mano en Rider. R3, R4 y R5 las aplicó Claude Code a mi pedido, y yo verifiqué el resultado de cada una.
- **Tests que me quedé:** los 5, porque cada uno falló al menos una vez. Quedan 120 tests en verde.
- **Tests que borré y por qué:** ninguno. No hubo ningún test que pasara con y sin el cambio.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** <enlace al primer run en verde>
- **`claude.yml` en `main`:** <sí / no>
- **Revisé:** permisos mínimos ☐ · timeout ☐ · versiones fijas ☐ · token solo como secreto ☐

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
