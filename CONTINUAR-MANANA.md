# Contexto para continuar el POS

Actualizado el 16 de noviembre de 2026. El usuario continuará en otra sesión. Leer este archivo, `AGENTS.md`, `README.md` y `ESTADO-PRUEBAS-GASTOS.md` antes de realizar cambios.

## Producto y decisiones del usuario

POS de escritorio para **Helados Pinwi Pinwi**, exclusivo de Windows, completamente local y utilizable sin internet. C# / .NET 10 LTS, WPF, MVVM sencillo, SQLite mediante Microsoft.Data.Sqlite. Sin Electron ni servicios remotos para vender.

Conservar los datos comerciales: nombre, logo proporcionado, WhatsApp **833 393 2616**, Facebook **https://www.facebook.com/heladospinwi**, menú, precios e imágenes. No distribuir usuarios, contraseñas ni ventas de prueba. La configuración inicial pide crear al administrador; no hay credenciales predeterminadas.

El menú real contiene **76 productos** en 10 categorías. La cifra anterior de 86 era un error y fue corregida. Cada sabor y presentación es un producto distinto.

Reglas de mayoreo:

- Desde 10 piezas del mismo producto/sabor, todas esas piezas llevan mayoreo.
- Desde $400 valorados a menudeo, antes de descuentos y sin cargos adicionales, toda la compra lleva mayoreo.
- No revertir el mayoreo porque el total ajustado quede debajo de $400.
- Recalcular al cambiar cantidades. No sumar sabores distintos para alcanzar 10 piezas.
- Descuento porcentual general autorizado; actualmente excluye partidas a mayoreo. Los demás tipos de descuento siguen pendientes.

El usuario pidió una UI mucho más moderna. Se usa tema claro, rosa y tonos suaves de marca, tarjetas por tipo de producto y carrito lateral. Entrar primero por tipo de producto; no mostrar todos los sabores al abrir. Fotos locales en el inicio y fotos individuales dentro. La navegación debe decir **Inicio** y **Volver a inicio**, no Gamas.

El carrito permite +, −, quitar y escribir cantidades. Muestra la imagen a la izquierda del nombre. Mayoreo y total se recalculan. El control de existencias es solo informativo: **permitir cobrar sin existencias**, con aviso rojo. La edición de precios no exige capturar inventario.

## Implementado en el código actual

- Catálogo real precargado idempotentemente, imágenes locales y 75 imágenes individuales descargadas del repositorio de referencia.
- El sabor Arroz con leche usa imagen de categoría por falta de asociación individual.
- Registro inicial e inicio de sesión del administrador con PBKDF2 SHA-256, salt aleatorio y 600,000 iteraciones. Todavía no hay cuentas separadas de cajero.
- Caja con fondo inicial, venta transaccional, pagos en efectivo/tarjeta externa/mixtos y cambio.
- Pantalla única de cobro: importes sugeridos y captura manual; ejemplo $470 pagados con $500 → cambio $30. Tarjeta requiere confirmación de autorización externa.
- Ticket con logo, agradecimiento y Facebook/WhatsApp con iconos. Vista previa y formatos 58/80 mm.
- Historial y CSV con nombre real del usuario de sesión. Ventas antiguas sin ese dato muestran «No registrado».
- Carrito guardado tras cambios válidos, recuperación al iniciar y eliminación atómica del borrador al cobrar. No guardar contraseñas del descuento; pedir reautorización si se recupera.
- SQLite WAL y synchronous=FULL. Respaldos consistentes después de cambios (agrupados aproximadamente un segundo) y cada 5 minutos con el programa abierto, incluso sin actividad; retención de 288 copias.
- Cortes históricos con conteo, efectivo esperado, diferencia, venta del turno y acumulado del día a la hora del cierre, en horario de Ciudad de México.
- Nota y CSV diarios almacenados en la base y exportables a archivos locales.
- **Último cambio:** botón Registrar gastos junto a abrir/cerrar caja; importe, concepto, proveedor/persona, origen y usuario. Gastos pagados de caja restan del efectivo esperado; gastos externos no afectan ese efectivo. Todos restan del resultado de ventas menos gastos. Requiere turno abierto y evita duplicados.
- Reporte del día con ventas, menos gastos, resultado y cada concepto, además de CSV separado de gastos. No llamarlo utilidad contable. Los cortes anteriores no se reescriben.
- Esquema actual **versión 4**, con respaldo previo a migrar versiones anteriores.

## WhatsApp: alcance real

El destino es el número del negocio. Al cerrar caja se puede abrir una nota preparada y existe botón Enviar por WhatsApp. **El usuario necesita internet y pulsar Enviar en WhatsApp.** No existe envío desatendido, adjunto automático ni confirmación remota de entrega. «Marcar como enviado» es confirmación manual y se registra como tal. Las notas largas se copian completas para pegarlas, evitando truncar el enlace.

El usuario pidió envío diario automático al cerrar caja, pero no se configuró WhatsApp Business ni hay credenciales para hacerlo. No afirmar que esa parte está resuelta. No enviar reportes reales durante pruebas.

## Validación y bloqueo de Windows

La aplicación y las pruebas compilan sin errores ni advertencias.

Las pruebas de lógica llegaron a ejecutarse satisfactoriamente con base temporal aislada: precios, cantidades, cambio, ventas sin existencias, autenticación, idempotencia, cortes, recuperación de borrador, CSV, integridad del respaldo, gastos y el caso **$5,000 − $1,300 = $3,700**. También se probaron gastos de caja frente a externos y conservación de cortes anteriores.

**Pero Windows Smart App Control bloquea de manera intermitente los binarios nuevos por firma (0x800711C7).** Una repetición normal de pruebas pasó sin modificar seguridad; nuevas compilaciones de la herramienta de vistas volvieron a bloquearse, y el ejecutable publicado con gastos no pudo abrirse. No confundir una pasada de lógica con apertura verificada del último ejecutable.

- Smart App Control estaba activo.
- La sesión no tenía privilegios de administrador.
- No se encontraron certificados de firma y el usuario confirmó que no tiene certificado ni cuenta de firma.
- No se desactivaron protecciones, no se instalaron certificados y no se aplicaron atajos para saltar el bloqueo.
- La apertura completa del último ejecutable y la revisión visual de gastos están pendientes.
- La impresión física también está pendiente de verificar con la impresora real.

Detalles: `ESTADO-PRUEBAS-GASTOS.md`. Las capturas previas de la carpeta local ignorada `artifacts/previews` pueden ser anteriores al cambio de gastos; no tomarlas como evidencia nueva.

## Cómo retomar y reproducir

En un equipo Windows con SDK .NET 10:

```powershell
dotnet restore src/Pinwi.Pos
dotnet build tests/Pinwi.Tests -c Release
dotnet run --project tests/Pinwi.Tests -c Release --no-build
dotnet run --project tests/Pinwi.Tests -c Release --no-build -- --render
```

`--render` genera imágenes de ejemplo con datos aislados en `artifacts/previews`; `--ui` abre una vista de prueba aislada. Estas opciones pertenecen al proyecto de pruebas, no permiten omitir el inicio de sesión del programa de operación.

En la computadora original el SDK está instalado por usuario en `%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe` y no siempre está en PATH. `scripts/publish.ps1` lo detecta. No volver a descargar el SDK innecesariamente.

`scripts/publish.ps1` ejecuta pruebas y publica un paquete Windows x64 self-contained en `dist/Actualizacion/PinwiPOS`, más ZIP e instalador por usuario. **No sobrescribir una versión en ejecución ni presentar un paquete sin validar como listo.** `dist/` no se versiona: se debe reconstruir.

En el equipo original, antes de subir el código:

- `dist/Actualizacion/PinwiPOS` contiene la versión anterior que Windows sí permitió abrir, sin registrar gastos.
- `dist/Gastos/PinwiPOS` y `dist/PinwiPOS-Gastos-PendienteFirma.zip` contienen la actualización bloqueada con gastos.
- Se dejó abierta la versión anterior y un acceso directo del escritorio llamado «Helados Pinwi Pinwi - version verificada».
- No abrir ambas versiones sobre la misma base. No volver a una versión anterior una vez migrada la base.

Estos binarios, accesos directos y carpetas son locales; no estarán en un clon nuevo de GitHub.

## Archivos principales

| Archivo | Función |
| --- | --- |
| `src/Pinwi.Pos/catalog.json` | Catálogo comercial de 76 productos |
| `src/Pinwi.Pos/Models.cs` | Productos, carrito y reglas de precios |
| `src/Pinwi.Pos/Store.cs` | Esquema, usuarios, ventas, precios y respaldos |
| `src/Pinwi.Pos/Protection.cs` | Borradores y respaldos automáticos |
| `src/Pinwi.Pos/Cuts.cs` | Cortes, acumulado diario, notas y CSV |
| `src/Pinwi.Pos/Expenses.cs` | Gastos y límites del día comercial |
| `src/Pinwi.Pos/ExpenseWindow.*` | Formulario de gastos |
| `src/Pinwi.Pos/MainWindow.*` | UI y flujo principal |
| `src/Pinwi.Pos/MainWindow.Protection.cs` | Recuperación, cortes y WhatsApp |
| `src/Pinwi.Pos/MainWindow.Expenses.cs` | UI de gastos del día |
| `src/Pinwi.Pos/Payment*` | Cobro, sugerencias y cambio |
| `src/Pinwi.Pos/Receipt*` | Ticket con logo, contactos y formatos |
| `tests/Pinwi.Tests/Program.cs` | Pruebas ejecutables y vistas aisladas |
| `imagenes producto/` | Imágenes originales por categoría |
| `src/Pinwi.Pos/Assets/` | Imágenes empaquetadas, logo y procedencia |

## Datos que no deben subirse

Las bases del negocio viven en `%LOCALAPPDATA%\PinwiPOS`, fuera del repositorio. No subir esa carpeta, bases, respaldos, usuarios, hashes de contraseñas, ventas, cortes o gastos de operación/prueba. Tampoco certificados privados, tokens ni archivos de entorno.

Los productos, precios, teléfono, Facebook y logo sí deben quedar versionados. Las pruebas crean sus propios usuarios y datos ficticios en carpetas temporales únicas; no son registros precargados en producción. No borrar datos locales del usuario para limpiar un commit.

## Pendientes prioritarios

1. Resolver la firma/distribución admitida por Windows y verificar apertura de la actualización completa, sin desactivar seguridad.
2. Repetir pruebas de lógica y revisión visual de gastos/cortes; probar impresión física.
3. Mantener el contexto y las limitaciones reales en README; no dar por cumplido todo AGENTS.md.
4. Después: cuentas y roles de cajero, descuentos fijos y por partida, cancelaciones/devoluciones, alta de productos, domicilio, reportes avanzados y restauración desde la interfaz.

El repositorio de referencia de la web es https://github.com/Reingardo/helados-pinwi-pinwi. Solo se usó como fuente de recursos; el usuario eligió **un repositorio separado para el POS**. No modificar ni reemplazar esa web al continuar.
