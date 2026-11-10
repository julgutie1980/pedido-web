# Inventario de Pedidos Legado

<!--
Etapa 1 · Entender (plan mode).
Una fila por cada archivo de legado/src/ (.pas y .dfm). Puedes agregar legado/PedidosLegado.dpr al final.
- Tipo: Formulario, Data module, Unidad de lógica, Utilidades, Globales…
- Responsabilidad: qué hace, en una frase y con tus palabras.
- Líneas: pídele a Claude que las cuente con una herramienta; no las estimes a ojo.
Punto de control E1: están todos los archivos de legado/src/.
-->

| Archivo | Tipo | Responsabilidad | Líneas |
|---|---|---|---|
| UCalculoTotales.pas | Unidad de lógica | Calcula subtotal, descuento por volumen/VIP (tope 10 %), flete por región, IVA y total de un pedido en memoria. | 147 |
| UDMPrincipal.pas | Data module | Conecta la base DBISAM, abre las tablas y expone `LeerParametro` y `EjecutarSQL` al resto del sistema. | 100 |
| UDMPrincipal.dfm | Data module (diseño) | Declara la base `PEDIDOS`, las tablas CLIENTES, PRODUCTOS, PEDIDOS, PEDIDO_LINEAS, PARAMETROS y dos consultas de uso general. | 65 |
| UFrmClientes.pas | Formulario | Mantenedor de clientes: búsqueda, alta, edición y baja, con validación de RUT propia y control de VIP por nivel de usuario. | 215 |
| UFrmClientes.dfm | Formulario (diseño) | Pantalla de clientes con grilla, campos enlazados y su propia tabla CLIENTES. | 213 |
| UFrmInformes.pas | Formulario | Informe de ventas por rango de fechas y cliente, con totales neto/IVA y exportación a CSV. | 137 |
| UFrmInformes.dfm | Formulario (diseño) | Pantalla de filtros y grilla del informe, con su propia consulta a la base `PEDIDOS`. | 160 |
| UFrmPedidos.pas | Formulario | Toma de pedidos: busca cliente por RUT, agrega líneas, valida, graba pedido y líneas, recalcula el total, descuenta stock y asigna el folio. | 420 |
| UFrmPedidos.dfm | Formulario (diseño) | Pantalla de pedido: datos del cliente, grilla de líneas, observación y panel de totales. | 226 |
| UFrmPrincipal.pas | Formulario (menú principal) | Arranque: pide usuario, fija su nivel, conecta la base, carga parámetros y abre los demás formularios desde el menú. | 104 |
| UFrmPrincipal.dfm | Formulario (diseño) | Menú principal (Archivo, Maestros, Ventas, Informes) y barra de estado. | 62 |
| UFrmProductos.pas | Formulario | Mantenedor de productos con filtro por descripción y bajo mínimo, y validaciones de código, precio y categoría. | 105 |
| UFrmProductos.dfm | Formulario (diseño) | Pantalla de productos con grilla, navegador y su propia tabla PRODUCTOS. | 98 |
| UGlobales.pas | Globales | Variables globales compartidas (usuario, nivel, tasa IVA, flete base, ruta de datos) y sus valores iniciales. | 41 |
| UStock.pas | Unidad de lógica | Consulta y descuenta stock por producto o por pedido (los VIP pueden quedar hasta −20) y registra movimientos en MOV_STOCK. | 93 |
| UUtilidades.pas | Utilidades | Funciones sueltas: redondeo y formato de pesos, RUT, fechas (incluida la fecha de corte de las 18:00) y textos. | 189 |
| PedidosLegado.dpr | Programa | Punto de entrada: registra las unidades y crea el data module y el formulario principal. | 24 |

Líneas contadas con `@(Get-Content <archivo>).Count`, que incluye las líneas en blanco.

## Lo que me llamó la atención

- `legado/README.md` no coincide con el código: habla de Delphi 5, Paradox/BDE y Crystal Reports, pero todas las unidades usan DBISAM (`dbisamtb`) y no hay informes de Crystal. `UDMPrincipal.pas:58` dice «antes era alias BDE».
- `UStock.pas:85` inserta en `MOV_STOCK`, pero esa tabla no está en `legado/db/esquema-dbisam.sql`, y el `try … except end` vacío (`UStock.pas:84-90`) oculta el error.
- La lógica está duplicada: `UFrmPedidos.pas:295-319` recalcula descuento, flete e IVA por su cuenta (con `>=` en vez del `>` de `UCalculoTotales.pas:77-79`, y con un descuento de Navidad que `UCalculoTotales` no tiene). Además, `UFrmClientes.RutOk` valida el RUT de otra forma que `UUtilidades.ValidaRut`.
