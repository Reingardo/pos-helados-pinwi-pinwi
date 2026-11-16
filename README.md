# Helados Pinwi Pinwi — POS para Windows

Repositorio del POS: https://github.com/Reingardo/pos-helados-pinwi-pinwi

**Para continuar en otra sesión, empieza por [CONTINUAR-MANANA.md](CONTINUAR-MANANA.md) y [AGENTS.md](AGENTS.md).** Incluyen las decisiones del usuario, el estado del código, las pruebas y los pendientes.

El repositorio conserva el menú, precios, datos comerciales, logo e imágenes. No incluye bases de datos, cuentas, contraseñas, ventas de prueba ni certificados privados. Una instalación nueva solicita crear al administrador y empieza sin ventas.

Primera versión funcional en C# / .NET 10, WPF y SQLite. Opera localmente sin conexión. **Todavía no implementa todo el alcance de AGENTS.md.**

La versión actual incluye paquetes, alta y edición de productos con foto, y cobro rápido por gama con imágenes. En Venta se navega con **Productos** y **Cobro rápido**. Las fotos personalizadas se incluyen en los respaldos de SQLite. Consulta AGENTS.md para el estado y las limitaciones actuales. La prioridad es la entrega local; Microsoft Store no está publicado.

## Ejecutar

En esta computadora, abre `dist/ProductosCobroRapido/PinwiPOS/Pinwi.Pos.exe`. El paquete incluye .NET; el equipo del negocio no necesita instalar el SDK ni conectarse a internet. Cierra cualquier instancia del POS antes de abrir otra.

Para trasladarlo a otra computadora, utiliza un paquete de la versión actual y extrae **todo** su contenido. El ZIP antiguo `dist/PinwiPOS-Actualizado.zip` no debe usarse con la base migrada. La actualización con gastos sigue identificada como pendiente de firma; su inicio en esta computadora no garantiza autorización en otros equipos. No muevas solamente el `.exe`.

## Primer uso

1. Crea el administrador y una contraseña de al menos ocho caracteres. No hay contraseña predeterminada.
2. En **Productos y precios** puedes editar menudeo y mayoreo, con motivo y contraseña. No es necesario registrar existencias: las advertencias rojas son informativas y permiten vender.
3. Abre **Caja** y registra el fondo inicial.
4. En **Venta**, busca por sabor o categoría y pulsa productos para agregarlos. Usa «− Una» para reducir la partida seleccionada.
5. El precio cambia automáticamente a mayoreo desde 10 piezas del mismo producto o $400 valorados a menudeo. Los sabores diferentes de la misma gama se suman para alcanzar 10, también en cobro normal. En esquimales se cuenta por presentación.
6. Puedes aplicar un descuento porcentual general autorizado; en esta versión solo afecta partidas a menudeo.
7. Pulsa **Cobrar** y elige efectivo, tarjeta o mixto. Selecciona un importe sugerido o escribe con cuánto paga el cliente; revisa el cambio antes de confirmar. Los pagos con tarjeta requieren confirmar su autorización en una terminal externa.
8. Consulta ventas y reimprime desde **Historial y reportes**. El CSV contiene las últimas 1,000 ventas, al igual que esa pantalla.
9. Al terminar, cierra caja e introduce el efectivo contado.
10. Genera un respaldo en una carpeta o USB desde **Respaldo**.

## Datos y seguridad

Base de datos: `%LOCALAPPDATA%\PinwiPOS\pinwi.db`. La instalación y los datos están separados. El catálogo contiene 76 productos reales, todos inicialmente sin existencias. Las imágenes de categoría se incluyen localmente.

La venta y sus partidas se guardan transaccionalmente junto con la salida de existencias. Un identificador evita duplicados por reintentos. Un fallo de impresión no borra la venta. Las contraseñas se almacenan con PBKDF2 SHA-256, salt aleatorio y 600,000 iteraciones.

La primera versión admite únicamente el administrador. No compartas su contraseña para simular cuentas de cajero. Los respaldos `.db` contienen los datos del negocio y deben guardarse en un lugar de confianza.

## Disponible

- Catálogo de 76 productos con fotografías de categoría locales, búsqueda y filtro.
- Precios de menudeo/mayoreo y ajuste automático por cantidad o monto.
- Descuento porcentual general autorizado, excluyendo mayoreo.
- Edición de precios y captura de existencia con bitácora.
- Apertura y cierre de caja, efectivo esperado y diferencia.
- Cobros en efectivo, tarjeta externa y mixtos; cambio y aviso de inventario sin bloqueo.
- Persistencia SQLite, historial, vista de ticket, impresión mediante Windows y exportación CSV.
- Respaldo consistente mediante la API de SQLite.

## Pendiente del alcance completo

- Roles y cuentas de cajero, cambio y bloqueo de sesión.
- Descuentos fijos y por partida, configuración de acumulación y umbrales.
- Cancelaciones, devoluciones, gestión de varias ventas pendientes y domicilio. El carrito actual ya se recupera automáticamente.
- Alta y desactivación de productos, códigos de barras, modificadores, unidades por peso y recetas.
- Movimientos de inventario detallados, costos, mermas y mínimos.
- Entradas/salidas de efectivo. El historial visual de cortes ya está disponible.
- Reportes por rango, sabores, cajeros y utilidades.
- Configuración fiscal y datos del negocio; otros datos fiscales del negocio.
- Restauración desde la interfaz y fotografías personalizadas. Los respaldos automáticos ya están disponibles.
- Migraciones futuras con respaldo previo y actualizaciones empaquetadas.
- Validación completa del flujo en la interfaz, operación en un equipo limpio desconectado e impresión física.

La ventana inicial se verificó visualmente. No se creó una cuenta real ni se ingresaron ventas de prueba en la base de operación. La lógica de ventas se probó con una base temporal aislada. No presentar esta versión como el alcance completo listo para producción.

## Desarrollo

Requiere SDK .NET 10 y acceso a NuGet para restaurar dependencias durante el desarrollo.

GitHub Actions compila y ejecuta las pruebas de lógica en Windows. Si el flujo termina correctamente, publica un artefacto de aplicación **sin firma** y el registro de pruebas; eso no equivale a aprobación de Smart App Control ni valida impresión física.

```powershell
dotnet build src/Pinwi.Pos -c Release
dotnet run --project tests/Pinwi.Tests -c Release
./scripts/publish.ps1
```

Si el SDK se instaló con el script oficial por usuario, está en `%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe`. `publish.ps1` detecta esa ubicación.

Las pruebas verifican 11 escenarios de precios y el flujo de persistencia con catálogo, autenticación, inventario, caja, idempotencia, rollback y respaldo. Sus datos se guardan en una carpeta única bajo `%TEMP%\PinwiTests`, separada de la operación.

## Actualización: cobro y ticket del cliente

El ticket incluye logo local, agradecimiento, Facebook y WhatsApp con iconos. Se puede elegir papel de 58 u 80 mm en la vista previa. El historial y CSV incluyen el usuario real de sesión; por ahora el acceso sigue siendo del administrador existente, no hay cuentas separadas de cajeros. Las ventas anteriores sin nombre muestran «No registrado».

La base actual usa la versión 7, que incluye gastos, paquetes, fotos personalizadas y cobro rápido, conservando datos y creando un respaldo previo en la carpeta Respaldos. Cierra la versión anterior antes de abrir la actualizada. La impresión física todavía debe probarse con la impresora real.

## Actualización: cortes y autoguardado

El carrito incluye imágenes y se guarda automáticamente al modificarlo. La pantalla de Caja conserva los cortes y genera una nota y CSV con las ventas del día al cierre. Cada cinco minutos, incluso sin cambios, y después de modificaciones se crean respaldos consistentes (hasta 288 copias).

«Enviar por WhatsApp» abre una nota dirigida al número del negocio. Requiere internet y pulsar Enviar en WhatsApp; no hay envío desatendido ni adjunto automático. La nota queda pendiente hasta confirmación manual. Consulta [detalles de cortes y respaldos](CAMBIOS-CORTES-Y-RESPALDOS.md).
