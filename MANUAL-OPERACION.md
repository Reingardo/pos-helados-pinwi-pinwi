# Manual de operación — Helados Pinwi Pinwi POS 1.1

Aplicación local para Windows x64. La venta y los respaldos no necesitan internet. Los pagos con tarjeta se realizan en una terminal externa; el POS solo los registra.

## Actualizar y entrar

1. Cierra el POS anterior; no necesitas cerrar el turno para actualizar la aplicación.
2. Extrae todo el ZIP nuevo. Abre `PinwiPOS/Pinwi.Pos.exe`, o ejecuta `Instalar.ps1` para instalar por usuario y crear el acceso del escritorio.
3. La primera apertura actualiza la base a v8 y crea un respaldo antes de migrarla. Conserva ventas, precios, fotos, turnos y cuentas. No vuelvas a abrir versiones anteriores con esta base.
4. Selecciona tu usuario y escribe su contraseña. Una instalación limpia pide crear al primer administrador; no tiene cuentas ni contraseñas predeterminadas.

Los datos viven en `%LOCALAPPDATA%/PinwiPOS`, separados del programa. No copies únicamente el EXE ni reemplaces la carpeta de datos al actualizar.

La edición Microsoft Store 1.1.2.0 usa una carpeta distinta, `%USERPROFILE%\PinwiPOS-Store`. Al primer inicio ofrece traer un respaldo validado o comenzar una instalación independiente. No toma ni modifica automáticamente los datos de la entrega local. Para cambiar de edición, sigue el [procedimiento de traslado](MANUAL-INSTALACION.md#edición-microsoft-store-1120-primer-inicio-y-traslado). Store ya está publicado: [instalación oficial](https://apps.microsoft.com/detail/9N0ZB1RQ1RTR). El ZIP local continúa sin firma.

## Vender

Abre caja con el fondo inicial. En **Productos** elige gama y sabor, o usa **Cobro rápido** para registrar solo la gama. Ajusta piezas con +, − o captura la cantidad y pulsa Enter. El lector de códigos debe funcionar como teclado y terminar con Enter en la búsqueda.

El mayoreo inicial se activa desde 10 piezas de la misma gama, mezclando sabores y venta normal/rápida, o $400 valorados a menudeo. Bolis de agua/leche comparten gama, igual que bollos/heladitos. Esquimales cuentan por presentación. El carrito muestra el motivo de mayoreo. Los paquetes se venden completos.

Las existencias son informativas: el aviso rojo no bloquea cobros y el saldo se limita a cero. Cobro rápido no descuenta un sabor específico. Los precios rápidos son independientes; una gama pendiente de confirmación debe configurarse en Productos y precios.

Pulsa **Cobrar**, selecciona efectivo, tarjeta o mixto y revisa el cambio antes de confirmar. Para tarjeta, confirma la autorización de la terminal externa. Un error de impresión conserva la venta: reimprime desde Historial, sin cobrar de nuevo.

Atajos: **F2** buscar, **F4** cobrar, **F8** guardar pendiente, **F12** bloquear/cambiar usuario.

## Descuentos y pendientes

En **Descuentos** elige toda la venta o una partida y captura porcentaje o importe fijo. Cada aplicación requiere motivo y autorización del administrador. Se descuenta primero por partida y después sobre la venta. El descuento fijo general se reparte en centavos entre partidas elegibles. Inicialmente se excluye mayoreo; el administrador puede cambiar la acumulación.

Si un cambio en el carrito vuelve inválido un descuento fijo, el POS retira los descuentos y muestra un aviso para volver a aplicarlos. No permite totales negativos.

**Guardar F8** aparta el carrito con una referencia; **Pendientes** recupera uno de tus carritos guardados cuando el actual está vacío. Se conservan cantidades y descuentos, pero nunca la contraseña de autorización. Al recuperar se usan los precios actuales y se vuelve a autorizar el descuento al cobrar. Cada usuario conserva sus propios pendientes. El carrito activo también se recupera automáticamente después de cerrar.

## Gastos, entradas y retiros

**Registrar gastos** registra concepto, importe, proveedor/persona y origen. Los gastos pagados de caja reducen el efectivo esperado; los externos no lo reducen. Ambos se restan del resultado de ventas menos gastos.

**Entrada / retiro** registra dinero agregado para cambio o retirado para depósito, con autorización y motivo. Modifica el efectivo esperado, sin tratarlo como venta ni gasto. No se permite retirar más efectivo del esperado.

Al cerrar caja captura el efectivo contado y revisa la diferencia. El cálculo contempla fondo, ventas en efectivo, gastos de caja, entradas, retiros y devoluciones en efectivo. El corte conserva su nota y sus CSV de ventas, gastos y ajustes del día al momento del cierre. Los cortes anteriores no se reescriben.

## Devoluciones y cancelaciones

En **Historial y reportes**, selecciona una operación de tipo Venta y pulsa **Devolución / cancelación**. Captura unidades, motivo y autorización. Se muestra el importe exacto a devolver antes de registrar. Se utiliza el precio y descuento originales, aunque el catálogo haya cambiado.

Las devoluciones parciales conservan los centavos y no pueden superar las unidades vendidas. En pagos mixtos, el reembolso se divide proporcionalmente entre efectivo y tarjeta originales. Confirma la devolución de tarjeta en la terminal externa. Debe existir efectivo suficiente en el turno actual para la parte en efectivo.

Marca reintegro de inventario únicamente si recibiste producto que corresponde reintegrar. No se asignan sabores ni componentes a paquetes o cobro rápido. Cancelar devuelve toda la venta sin asumir reintegro de inventario; si ya hubo devoluciones parciales, devuelve las unidades restantes. La venta original y su ticket permanecen en el historial, junto a la operación de ajuste.

## Reportes y usuarios

Filtra por fechas, usuario y método de pago. Las fechas se interpretan en horario de Ciudad de México. Se muestran 100 operaciones por página, con totales del periodo completo. **Exportar operaciones** exporta todo el filtro, sin límite de 1,000 ventas; las devoluciones aparecen negativas. **Exportar productos** guarda el desglose por producto, gama y paquete. Las operaciones antiguas sin clasificación se identifican como tales; no se inventan sabores ni ahorros históricos.

Un cajero consulta/exporta sus propias operaciones. En **Administración**, el administrador crea cuentas, cambia roles o desactiva usuarios y configura negocio, umbrales, acumulación de descuentos, papel de 58/80 mm, impresora y respaldo adicional. Debe conservarse al menos un administrador activo. **F12** bloquea el acceso y permite cambiar de usuario conservando el carrito anterior.

En **Productos y precios** puedes añadir/editar productos y fotos, activar/desactivar productos y asignar un código de barras real. Desactivar no elimina ventas anteriores. La foto queda incluida en los respaldos.

## Respaldar y restaurar

Los respaldos automáticos se crean después de cambios agrupados y cada cinco minutos. Se conservan hasta 288 copias recientes, la primera copia de cada uno de los últimos 30 días con respaldo y la primera de cada una de las últimas 12 semanas con respaldo. Una carpeta o USB adicional recibe copias consistentes; si falla, se conserva la copia local y aparece una advertencia. El USB debe estar conectado para recibir nuevas copias.

Para una copia manual usa **Respaldo → Crear respaldo**. Las bases contienen cuentas y datos del negocio: guárdalas en un lugar de confianza. Las copias en el mismo disco no cubren la pérdida física de ese disco.

Para restaurar: entra como administrador, cierra el turno y guarda o vacía el carrito. Selecciona un `.db` en **Restaurar respaldo**, revisa su resumen y confirma. Autoriza con la contraseña del administrador actual. El POS valida el archivo, prepara una copia aislada compatible y conserva el estado previo en `Respaldos/antes-restaurar-*.db`. La restauración reemplaza el estado actual por el de la copia seleccionada; los cambios posteriores a esa copia no se fusionan.

Al terminar, el programa se cierra. Ábrelo de nuevo e inicia sesión con una cuenta del respaldo. Un archivo corrupto, incompatible o sin cuenta recuperable se rechaza. No borres ni reviertas manualmente la base para resolver un error.

## Alcance y verificaciones pendientes

WhatsApp prepara una nota para envío manual con internet; no hay envío desatendido ni confirmación remota de entrega. Resultado de ventas menos gastos no significa utilidad contable.

La impresión física de 58/80 mm y la instalación completa en un segundo equipo limpio desconectado siguen pendientes. También quedan inventario avanzado con recetas, costos/mermas y pedidos a domicilio. La distribución local sigue sin firma. La última revisión registrada de Store, el 23 de noviembre de 2026, confirmó el envío a certificación de la corrección 1.1.2.0 y publicación automática tras aprobación; no demuestra aprobación ni disponibilidad pública.
