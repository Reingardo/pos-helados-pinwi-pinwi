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
