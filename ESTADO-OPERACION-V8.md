# Entrega Operación Segura — 16 de noviembre de 2026

Código de aplicación 1.1.1.0, SQLite v8. Entrega local en `dist/OperacionSegura-v111/PinwiPOS`; ZIP `dist/PinwiPOS-OperacionSegura-v111.zip`. No publicar en Store sin una nueva indicación del usuario.

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

Microsoft Store permanece sin cambios y con publicación retenida. No afirmar firma, certificación aprobada ni disponibilidad pública. Binarios, bases, cuentas, respaldos y evidencia de pruebas son locales e ignorados por Git.
