# Carrito, cortes y protección de datos

## Carrito

Cada partida muestra la imagen local del producto a la izquierda del nombre. Se conservan cantidad editable, botones de sumar/restar y el aviso informativo de existencias.

El carrito se guarda en SQLite al agregar, quitar y cambiar cantidades o descuentos. Mientras se escribe una cantidad válida, también se guarda el último valor válido. Una cantidad vacía o inválida no sustituye el valor guardado. Los cambios de búsqueda o navegación no modifican ventas.

Al volver a iniciar sesión se recupera la venta pendiente con los precios vigentes. Los descuentos recuperados requieren autorización antes de cobrar; las contraseñas no se guardan con el carrito. El identificador de venta evita recuperar como pendiente o duplicar una operación ya cobrada.

## Caja y cortes

Al cerrar caja se conservan el conteo, fondo, venta del turno, efectivo esperado y diferencia. La pantalla muestra un historial de cortes que persiste entre aperturas del programa.

Se genera una nota con dos secciones claramente separadas:

- Ventas y conciliación del turno cerrado.
- Acumulado de las ventas del día hasta ese momento, en horario de Ciudad de México.

La nota y el detalle completo de ventas del día se guardan en la base. Se exportan a `resumen.txt` y `ventas-del-dia.csv` dentro de `%LOCALAPPDATA%\PinwiPOS\Cortes\corte-XXXXXX`. El CSV abre en Excel y conserva el usuario por venta. No se aplica el límite de 1,000 ventas de la pantalla de historial a este reporte.

Los cortes anteriores a esta actualización conservan sus importes; no se inventa quién los cerró ni una nota histórica que no se había generado.

## WhatsApp

Destino: **833 393 2616**, con código de país México.

La casilla «Abrir WhatsApp con la nota al cerrar caja» está activada inicialmente. Al cerrar un turno abre una nota preparada para ese número. El botón «Enviar por WhatsApp» permite volver a prepararla para cualquier corte nuevo seleccionado.

**El usuario debe pulsar Enviar en WhatsApp y contar con internet.** Abrir la nota no equivale a haberla enviado. El POS conserva el estado pendiente; «Marcar como enviado» registra únicamente la confirmación manual del usuario, sin afirmar una verificación remota.

El texto preparado contiene el resumen. Para compartir también el detalle, «Abrir nota y CSV del corte» muestra los archivos que se pueden adjuntar desde WhatsApp. El enlace no adjunta archivos automáticamente.

El envío desatendido, los reintentos remotos y la confirmación de entrega requieren una integración autorizada con WhatsApp Business, conectividad y configuración de cuenta. No se han configurado ni simulado. No se realiza envío diario si el POS está cerrado ni se programa una hora arbitraria: el evento definido es cerrar caja cada día.

## Autoguardado y respaldos

- SQLite confirma las operaciones y el carrito mediante transacciones con `synchronous=FULL` y diario WAL.
- Tras modificaciones se solicita una copia consistente. Las ráfagas se agrupan durante aproximadamente un segundo; el guardado de la base no espera ese segundo.
- Con el POS abierto, también se crea una copia cada cinco minutos aunque no haya modificaciones.
- Se conservan hasta 288 copias en `%LOCALAPPDATA%\PinwiPOS\Respaldos\Automaticos`.
- El estado de respaldo y los errores se muestran en **Respaldo**. Los errores se registran localmente y no se presentan como copias exitosas.
- La migración a versión 3 crea una copia previa de las bases anteriores y conserva ventas, usuarios, precios y cortes.
- La recuperación protege los cambios guardados; no garantiza protección frente a daño físico del disco o del equipo. Una copia en USB sigue siendo útil.

## Verificación

Pruebas de guardado y recuperación del carrito, eliminación transaccional del borrador al cobrar, prevención de duplicados, persistencia de cortes, suma diaria, generación de nota y CSV, confirmación manual de envío e integridad del respaldo SQLite. El temporizador se verifica con un intervalo corto de prueba; el intervalo del programa es cinco minutos.

El flujo de WhatsApp no se envió a la empresa durante las pruebas y no se afirma entrega automática. Las vistas de carrito y cortes se renderizan con datos aislados de ejemplo.
