**Store actualizado por solicitud posterior:** el envío contiene 1.1.1.0, Validated y reenviado a certificación. Publicación manual retenida; no hay aprobación ni firma disponible confirmadas. Leer el último apartado de DISTRIBUCION-STORE.md. No pulsar Publish now sin nueva indicación.

**Actualización de continuidad — Operación Segura 1.1.1.0 / SQLite v8.** Leer primero `ESTADO-OPERACION-V8.md`, `MANUAL-OPERACION.md` y la sección vigente de AGENTS.md. Entrega nueva: `dist/OperacionSegura-v111/PinwiPOS/Pinwi.Pos.exe`, ZIP `dist/PinwiPOS-OperacionSegura-v111.zip`. Cerrar versiones previas; la migración crea respaldo previo. Nunca abrir v7 sobre datos v8. Se implementaron recuperación desde UI, roles, devoluciones/cancelación, movimientos, descuentos adicionales, pendientes, reportes completos y respaldos por calendario. La impresión física y revisión manual exhaustiva siguen pendientes. No publicar en Store. Las notas siguientes conservan la historia.

**Último ajuste solicitado:** el botón al lado de Cobro rápido ahora siempre dice «Productos», reemplazando Inicio/Volver a inicio. También se ajustó el encabezado general y la indicación de esquimales. Esta decisión posterior prevalece sobre los textos históricos de navegación. Publicación local compilada: dist/ProductosCobroRapido/PinwiPOS/Pinwi.Pos.exe. Accesos del escritorio actualizados; cerrar la versión abierta antes de iniciar esta. Se conservaron todos los cambios previos y la base no se modificó.

**Actualización local más reciente:** las tarjetas de Cobro rápido ahora incluyen su imagen a la izquierda y muestran nombre, menudeo y mayoreo. Usa la imagen personalizada si se configuró; en caso contrario, la imagen local de la gama. Vista de prueba aislada renderizada y revisada; compilación/publicación correctas. Ejecutable nuevo: dist/CobroRapidoFotos/PinwiPOS/Pinwi.Pos.exe. Accesos del escritorio actualizados. Cerrar el POS anterior antes de abrirlo. No se modificó la base ni se sobrescribió la versión abierta. No se cambió el paquete enviado a Store; el usuario prioriza la entrega local.

**Última verificación local:** el usuario autorizó expresamente desactivar Smart App Control para usar el POS local sin pagar ni publicar; la herramienta no pudo abrir Seguridad de Windows y el usuario realizó personalmente el cambio y avisó «ya». No se modificó la seguridad mediante scripts. Después, la batería completa de pruebas pasó, incluidas altas/fotos con respaldo independiente, paquetes, cobro rápido, mezcla de sabores por gama, umbrales, gastos, caja, borrador, idempotencia y recálculo. Se renderizó y revisó la interfaz compacta final de cobro rápido. El ejecutable publicado de dist/CobroRapido/PinwiPOS/Pinwi.Pos.exe abrió y permaneció activo con título del POS. Los accesos directos del escritorio se actualizaron a CobroRapido. La base ahora puede estar migrada a v7: no abrir Productos/Paquetes/Gastos/Actualizacion sobre ella. No se hicieron ventas de prueba en la operación ni se capturó su contraseña. Falta probar impresión física y el flujo completo interactivo con el usuario. El usuario no desea publicar por ahora; Store permanece con publicación manual retenida y no se debe pulsar Publish now.
**Estado más reciente:** el POS v1.0.7.0 con cobro rápido, mayoreo por gama, paquetes y edición de fotos fue enviado a certificación de Microsoft Store. Estado confirmado: In certification / Pre-processing; NO publicado ni firmado para instalar todavía. El usuario autorizó distribución pública gratuita y términos IARC; publicación manual retenida hasta Publish now. Detalles y autorizaciones en DISTRIBUCION-STORE.md. Los datos operativos no se modificaron.

**Distribución gratuita pendiente:** se preparó un MSIX borrador con los cambios actuales y validación de MakeAppx aprobada. La firma gratuita puede obtenerse tras certificación de Microsoft Store; el alta individual es gratuita. El usuario debe completar la verificación en https://storedeveloper.microsoft.com. No hay cuenta creada ni paquete enviado/firmado. Consultar DISTRIBUCION-STORE.md. No presentar el MSIX provisional como instalable o desbloqueado. La versión Productos que se abrió sigue sin cobro rápido. Los datos operativos no se modificaron en esta preparación.
**Cobro rápido y mayoreo por gama (16 de noviembre de 2026):** el usuario cambió la regla: sumar sabores distintos de la misma gama tanto en venta normal como en cobro rápido. Desde 10 piezas por gama o $400 a menudeo se aplica mayoreo. Bolis de agua/leche comparten gama; heladitos de agua/leche también. Esquimales conservan conteo por presentación y no aparecen en cobro rápido. No recalcular ventas históricas. Botón «Cobro rápido» en Venta, con carrito y cobro compartidos. Siete entradas sin sabor: bolis agua/leche, bollos, paletas agua, paletas leche, artesanales, nieves y sándwiches. Los paquetes siguen en Inicio. Se guardan ventas por gama sin inventar sabor ni descontar existencias de un sabor concreto. Gamas rápidas y sabores de la misma gama suman cantidades si se combinan en el carrito. Precios de cobro rápido son independientes, visibles y editables en Productos y precios; al migrar se toman de los productos existentes solo si toda la gama coincide, en otro caso la gama queda deshabilitada hasta que el administrador confirme sus precios. Tabla quick_products y esquema v7, respaldo antes de migrar. No regresar a ejecutables anteriores una vez migrada la base.

Paquete nuevo: `dist/CobroRapido/PinwiPOS/Pinwi.Pos.exe`, sin firma. Pasaron pruebas de lógica de cobro rápido, mayoreo entre sabores, caja, corte, borrador, idempotencia y conservación de existencias por sabor. Se renderizó y revisó una primera vista. Después del ajuste final de tarjetas compactas y pruebas adicionales, Windows bloqueó la biblioteca de pruebas (0x800711C7), también al repetir ejecución normal. Compilación final sin errores ni advertencias y publicación completadas; la última interfaz compacta, las nuevas comprobaciones adicionales y la apertura del paquete quedan pendientes de ejecución por firma. No se cambió seguridad ni se alteraron los datos operativos; los accesos directos previos no se cambiaron para esta entrega pendiente de verificación. Cerrar la instancia actual antes de abrir otra.
**Actualización de productos y fotos (16 de noviembre de 2026):** «Productos y precios» ahora ofrece «+ Añadir producto» y «Editar producto y foto». Formulario con nombre, categoría existente/nueva, menudeo, mayoreo, foto opcional, motivo y autorización del administrador. Al guardar se actualizan catálogo, inicio de Venta y productos del carrito. Las fotos se normalizan a PNG y se conservan como BLOB dentro de SQLite; la carpeta Imagenes es una caché reconstruible. Los respaldos de base incluyen las fotos personalizadas. Altas con existencias cero, sin bloquear ventas. Esquema versión 6 con respaldo previo: no regresar a ejecutables anteriores tras migrar. Paquete actual: `dist/Productos/PinwiPOS/Pinwi.Pos.exe`. Pruebas aisladas de lógica, alta/edición, autorización, duplicados y respaldo con foto restaurada aprobadas; formularios renderizados y revisados. No se probó aún el selector de archivos de forma interactiva ni la apertura de este paquete sobre datos operativos o la impresión física. Sin firma; no se cambió seguridad. Cerrar el POS abierto antes de iniciar la actualización.
**Actualización de paquetes (16 de noviembre de 2026):** se agregó «Paquetes» al inicio: Pinwi $500, Bollos $480, Fiesta $695, Esquimales $445 y Surtido $1,090, con imágenes locales del repositorio de referencia. Ahora son 76 productos individuales más 5 paquetes. La base migra a versión 5 con respaldo previo, sin sobrescribir precios existentes. Pruebas de lógica y migración aprobadas y vistas de Inicio/Paquetes renderizadas y revisadas. Publicado en `dist/Paquetes/PinwiPOS`; cerrar el POS actual antes de abrir esta versión. Una vez migrada la base, no abrir versiones de `dist/Gastos` o `dist/Actualizacion`. El paquete no tiene firma y todavía no se verificó su apertura sobre la base operativa ni impresión física. Los paquetes se registran completos, sin elegir sabores ni descontar sus componentes individuales.
# Contexto para continuar el POS

**Última actualización:** la versión anterior de `dist/Actualizacion` se cerraba al inicio porque la base ya requiere una versión más reciente, según el registro de .NET. Se inició el ejecutable de `dist/Gastos/PinwiPOS` y se corrigieron los accesos directos para apuntar allí, sin alterar seguridad ni datos. No ofrecer nuevamente el enlace anterior para esta base. La apertura del proceso está confirmada; la revisión completa de UI e impresión sigue pendiente. Las notas de firma inferiores conservan la historia de intentos previos.

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
## Registro confirmado y producto reservado

El 16 de noviembre de 2026 se confirmó la sesión del usuario en Google Chrome y el acceso al panel Apps and games de Partner Center. Se reservó «Helados Pinwi Pinwi POS» y se creó Submission 1, ambos en borrador. Store ID: 9N0ZB1RQ1RTR. Identity Name: EdRos.HeladosPinwiPinwiPOS; Publisher: CN=BE31C71B-F8D9-4853-9554-9F8ABF834F72; PublisherDisplayName: EdRos. Son identificadores del producto/editor, no credenciales. No se accedió a tokens ni contraseñas.

Se generó y validó con MakeAppx el paquete con identidad real: dist/Store/PinwiPOS-1.0.7.0-PARA-REVISION-STORE-d0e6f46d.msix, registro en artifacts/store-package-build.log. Continúa sin firma y NO se instaló. No se subió a Microsoft ni se envió a certificación. La última batería de pruebas continuó bloqueada por Windows, así que no afirmar validación funcional completa.

Se revisó disponibilidad: hay audiencia privada y distribución por enlace, pero no se guardó ninguna configuración. El formulario muestra por defecto audiencia pública/mercados mundiales; antes de enviar a certificación hay que elegir explícitamente disponibilidad, precio gratuito y revisión manual del usuario. La vista Application overview confirma todos los apartados aún Not started y Submit for certification deshabilitado. La reserva del nombre y el borrador no equivalen a firma, aprobación ni aplicación instalable.

## Cambio de alcance posterior
El usuario indicó que no desea publicar por ahora porque el POS no está terminado y solicita una versión local con TODOS los cambios, que abra. No continuar publicación ni pulsar Publish now. La compilación completa se volvió a verificar sin errores ni advertencias; el paquete local continúa sin firma. Se pidió consentimiento explícito para una posible desactivación de Smart App Control como alternativa local sin certificado; la solicitud pendiente NO equivale a autorización y no se cambió ninguna protección. Cualquier respuesta posterior del usuario manda sobre esta nota.
