# Estado de la actualización de gastos

## Cambios en el código

- Botón «Registrar gastos» junto a abrir y cerrar caja.
- Formulario con importe, concepto, proveedor o persona y origen del pago.
- Historial de gastos del día con usuario y fecha.
- Gastos de efectivo descontados del efectivo esperado; pagos externos separados para no alterar el conteo de caja.
- Resumen diario con ventas, menos gastos, resultado y conceptos detallados.
- CSV de gastos junto a la nota y al CSV de ventas.
- Notas extensas copiadas completas para pegarlas en WhatsApp, evitando truncarlas en un enlace largo.
- Navegación «Inicio» y «Volver a inicio».
- Migración a versión 4 con respaldo previo.

## Validación

La compilación de la aplicación y del proyecto de pruebas terminó sin errores ni advertencias.

**No se pudieron ejecutar las pruebas de esta actualización.** Windows Application Control bloqueó la carga de las bibliotecas generadas. El registro de Code Integrity informa que no cumplen los requisitos de firma de la política del equipo. El error de ejecución fue `0x800711C7`.

No se cambiaron las políticas de seguridad ni se intentó desactivarlas. La siguiente acción requiere que la persona que administra la seguridad del equipo autorice o firme estos archivos mediante el procedimiento permitido de Windows. Después debe repetirse la ejecución de pruebas y la revisión visual.

La prueba de gastos preparada cubre el ejemplo de $5,000 en ventas menos $1,300 en gastos, conceptos, pagos de caja frente a externos, validación de importes, duplicados, persistencia y CSV. **Está pendiente de ejecución; no se presenta como aprobada.**

El ejecutable y ZIP en `dist/Actualizacion` conservan la entrega anterior a gastos. No se sustituyeron con una actualización sin validar. Las capturas previas de cortes tampoco representan la pantalla nueva de gastos.

El envío por WhatsApp sigue requiriendo internet y pulsar Enviar; no se configuró envío desatendido.

## Comprobación posterior

Al repetir la ejecución normal, las pruebas de lógica sí pudieron completarse con éxito, incluyendo gastos ($5,000 − $1,300 = $3,700), validaciones, idempotencia, CSV y conservación de cortes. No se cambió la seguridad del equipo y no se atribuye ese resultado a una autorización permanente.

Después de compilar la ampliación de vistas de prueba, Windows volvió a bloquear la biblioteca de pruebas. También se intentó abrir el ejecutable publicado en dist/Gastos/PinwiPOS y Windows bloqueó su biblioteca: la apertura completa y la revisión visual siguen pendientes. La compilación continúa sin errores.

Smart App Control está activo. No se encontraron certificados de firma de código disponibles y el usuario confirmó que no dispone de certificado ni cuenta de firma. No se generó una firma ficticia ni se cambió la configuración de seguridad.

Se dejó la versión anterior abierta y un acceso directo en el escritorio llamado «Helados Pinwi Pinwi - version verificada». Esa versión no incluye gastos. La actualización está separada en dist/Gastos y dist/PinwiPOS-Gastos-PendienteFirma.zip, identificada como pendiente de firma.

Referencia oficial: https://learn.microsoft.com/en-us/windows/apps/develop/smart-app-control/code-signing-for-smart-app-control
## Bloqueo confirmado al abrir CobroRapido

El 16 de noviembre se revisaron los eventos del intento del usuario: Code Integrity 3077/3033 y .NET Runtime 1026 confirman bloqueo de dist/CobroRapido/PinwiPOS/Pinwi.Pos.dll por Smart App Control, error 0x800711C7. Authenticode: NotSigned. La base operativa se consultó en modo solo lectura y continúa en user_version=6; el intento bloqueado no llegó a migrarla. No se cambió seguridad ni se modificó la base. No presentar CobroRapido como apertura validada. La versión Productos es compatible con esquema 6, pero su disponibilidad debe comprobarse antes de afirmar que está abierta. La corrección de distribución requiere firma de un proveedor confiable u otro mecanismo autorizado por el administrador.
**Última verificación local:** el usuario autorizó expresamente desactivar Smart App Control para usar el POS local sin pagar ni publicar; la herramienta no pudo abrir Seguridad de Windows y el usuario realizó personalmente el cambio y avisó «ya». No se modificó la seguridad mediante scripts. Después, la batería completa de pruebas pasó, incluidas altas/fotos con respaldo independiente, paquetes, cobro rápido, mezcla de sabores por gama, umbrales, gastos, caja, borrador, idempotencia y recálculo. Se renderizó y revisó la interfaz compacta final de cobro rápido. El ejecutable publicado de dist/CobroRapido/PinwiPOS/Pinwi.Pos.exe abrió y permaneció activo con título del POS. Los accesos directos del escritorio se actualizaron a CobroRapido. La base ahora puede estar migrada a v7: no abrir Productos/Paquetes/Gastos/Actualizacion sobre ella. No se hicieron ventas de prueba en la operación ni se capturó su contraseña. Falta probar impresión física y el flujo completo interactivo con el usuario. El usuario no desea publicar por ahora; Store permanece con publicación manual retenida y no se debe pulsar Publish now.
