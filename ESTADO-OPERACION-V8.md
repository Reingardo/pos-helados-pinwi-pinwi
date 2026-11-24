# Entrega Operación Segura — 16 de noviembre de 2026

Código de aplicación 1.1.1.0, SQLite v8. Entrega local en `dist/OperacionSegura-v111/PinwiPOS`; ZIP `dist/PinwiPOS-OperacionSegura-v111.zip`. El usuario autorizó después la publicación en Store; está configurada para comenzar automáticamente tras aprobar la certificación.

## Cambios

- Migración aditiva con respaldo previo; conserva el catálogo, imágenes, precios, ventas y cortes anteriores.
- Instancia única por base y detección de versiones anteriores abiertas. Publicación e instalador rechazan sobrescribir el programa en ejecución.
- Restauración desde la interfaz: inspección, confirmación, autorización, preparación aislada, copia previa y reemplazo transaccional mediante la API SQLite. Requiere reiniciar sesión con las cuentas restauradas.
- Cuentas de administrador/cajero, desactivación y bloqueo/cambio de sesión. Autorización en la lógica para operaciones sensibles; reportes de cajero limitados a sus operaciones.
- Devoluciones parciales/cancelación con importes originales, redondeo acumulativo y distribución exacta de pagos mixtos. Caja usa el turno del reembolso, sin reescribir venta ni corte anterior.
- Entradas/retiros de efectivo independientes de ventas y gastos. El corte y gasto de caja consideran todos los movimientos y reembolsos.
- Descuentos fijos/porcentuales, generales y por partida; reparto de centavos y acumulación configurable. Snapshot de reglas y precios aplicados en ventas nuevas.
- Varios pendientes por usuario con recuperación atómica y sin contraseñas persistidas.
- Reportes por periodo, usuario y método; paginación y exportación completa de operaciones/productos. Clasificación de gama, paquete, producto individual o dato histórico desconocido.
- Configuración de negocio, mayoreo, papel, impresora y respaldo adicional. Identidad de tickets nuevos conservada para reimpresión histórica.
- Activación/desactivación de productos, códigos de barras de teclado, atajos y motivo de mayoreo visible.
- Copias recientes/diarias/semanales y destino externo con advertencia de fallo. Caché de fotos y cálculo de carrito; cobro, reportes, autorizaciones de formularios y respaldos costosos fuera del hilo visual. Cierre espera a que termine el respaldo.

## Evidencia reproducible

`scripts/publish.ps1` ejecuta las pruebas antes de publicar y guarda el registro local en `artifacts/pruebas-operacion.txt`.

La batería incluye pruebas anteriores de precios/pagos/idempotencia/migraciones/gastos/fotos y casos nuevos de roles, instancia, permisos, descuentos y centavos, devolución parcial mixta, rollback mediante fallo SQLite, cancelación, conciliación de corte, pendientes, exportación de 1,005 ventas y límites horarios de México, restauración real aislada y fallo de destino externo.

`tests/Pinwi.Tests --render` genera vistas locales en `artifacts/previews`. `--ui` abre una sesión de pruebas con datos temporales y título PRUEBA AISLADA; solo existe en el proyecto de pruebas, no en el ejecutable de operación.

Se revisó interactivamente con datos aislados: navegación, cobro rápido, edición de cantidad y mayoreo de 10 bolis a $50, pago de $100 y cambio de $50, confirmación de una sola venta, ticket, reportes de operaciones y gamas, administración y respaldo. Se revisan además renders de los formularios nuevos. No se automatizan las credenciales reales del usuario ni se registran operaciones ficticias en su base.

## Límites de la validación

No equivale a una revisión manual de todas las combinaciones de formularios administrativos. La lógica de restauración y devoluciones se prueba con bases temporales; no se restaura la base del negocio para probarlas. Impresión física 58/80 mm, lector físico e instalación completa en un segundo Windows limpio sin internet siguen pendientes.

Inventario avanzado, costos, recetas, mermas, domicilio y configuración fiscal quedan fuera de esta entrega. El POS sigue vendiendo sin bloqueo de existencias, con avisos informativos. WhatsApp sigue requiriendo envío manual.

Por indicación posterior del usuario, Microsoft Store recibió el paquete 1.1.1.0 y se reenvió a certificación. Luego autorizó publicarlo: el portal confirma publicación automática tras aprobación. Todavía no hay disponibilidad pública confirmada. No afirmar firma, certificación aprobada ni disponibilidad pública. Binarios, bases, cuentas, respaldos y evidencia de pruebas son locales e ignorados por Git.

## Corrección de distribución 1.1.2.0 — 23 de noviembre de 2026

Tras el segundo rechazo de unvirtualizedResources, se retiraron ese permiso y la exclusión de AppData. La edición Store 1.1.2.0 usa `%USERPROFILE%\PinwiPOS-Store` y una bienvenida para importar un respaldo validado o empezar independiente. La entrega local 1.1.1.0 y su base no se sustituyeron. El esquema sigue siendo v8. Se documentó el traslado en MANUAL-INSTALACION.md.

El CI Windows 45305436247 (commit 86308b4) pasó compilación, reglas anteriores, pruebas de importación inicial con preservación de historial/fotos/cuentas/roles/configuración/pendientes, rechazo sin reemplazo, reintento y migración aislada v7. También generó renders WPF, incluida la bienvenida. Se corrigió la liberación del archivo de respaldo al cerrar la conexión de destino. La prueba local había sido bloqueada por Windows; no se alteraron protecciones ni se eludió el bloqueo.

MakeAppx validó el MSIX 1.1.2.0 y se comprobaron su contenido y versiones. Partner Center recibió y validó el paquete corregido, se retiró 1.1.1.0 del envío, y Submission Options mostró solo runFullTrust. Tras guardar ficha, soporte, privacidad y notas, se envió a certificación. Resultado observado: **In certification**, Submission Complete, Pre-processing In progress, Certification/Publishing Not started y publicación automática tras aprobación. Firma descargable y disponibilidad pública siguen pendientes. La instalación MSIX firmada en un segundo equipo limpio, el flujo interactivo completo de bienvenida y los periféricos físicos no se presentan como verificados.

## Publicado y descarga 1.1.2.0 — 23 de noviembre de 2026

Este estado prevalece sobre los apartados históricos de certificación pendiente. Partner Center mostró **In Microsoft Store**, ficha de Store de solo lectura y el mensaje de que el producto está disponible según su configuración de descubrimiento. El enlace **View Product on Store** apuntaba al producto `9N0ZB1RQ1RTR`.

- Instalación oficial: https://apps.microsoft.com/detail/9N0ZB1RQ1RTR
- Publicación de GitHub: https://github.com/Reingardo/pos-helados-pinwi-pinwi/releases/tag/v1.1.2.0
- ZIP local completo sin firma: https://github.com/Reingardo/pos-helados-pinwi-pinwi/releases/download/v1.1.2.0/PinwiPOS-1.1.2.0-Windows-x64.zip
- Manual actualizado: `MANUAL-INSTALACION.md`; incluye PC independiente, traslado por respaldo y diferencias de carpetas Store/local.

El ZIP reutiliza la publicación self-contained 1.1.2.0 del código que pasó CI Windows 45305436247 (86308b4), con instalador y manuales actualizados. Los binarios se distribuyen como assets de Releases, no se incorporan al historial Git. No incluye cuentas, bases, respaldos ni operaciones. La publicación de Store no convierte el ZIP local en un binario firmado.

La entrega operativa 1.1.1.0 y sus datos no se sobrescribieron. No se instaló Store ni se migró la base del negocio al publicar estos archivos. Continúan pendientes la instalación completa en un segundo equipo limpio desconectado, impresión y lector físicos. No cambiar protecciones de Windows. Usar Store ante bloqueos del ZIP.
