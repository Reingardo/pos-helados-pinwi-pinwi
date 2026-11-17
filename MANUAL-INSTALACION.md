# Instalar Helados Pinwi Pinwi POS en otras computadoras

Guía de la versión local 1.1.1.0. La instalación completa en una segunda computadora y la impresión física siguen pendientes de comprobar. Este manual corresponde al ZIP local, que continúa sin firma. No equivale a un instalador aprobado de Microsoft Store.

## 1. Qué necesitas

- Una computadora con Windows de 64 bits. La entrega se ha abierto en Windows 11; no se han verificado todas las combinaciones de equipos y periféricos.
- El archivo `PinwiPOS-OperacionSegura-v111.zip`, copiado desde la computadora original a una USB o a la computadora de destino.
- Un lugar con permisos de escritura para extraerlo, por ejemplo una carpeta llamada `PinwiPOS` dentro de Documentos.

El ZIP ya incluye .NET, SQLite, catálogo e imágenes. Para abrir y vender no necesitas instalar el SDK de desarrollo ni descargar un runtime. Los pagos con tarjeta se operan por separado en una terminal externa.

## 2. Abrir el programa sin instalar

1. En la computadora de destino, copia el ZIP a una carpeta local. No lo ejecutes directamente desde la USB.
2. Haz clic derecho sobre el ZIP y elige **Extraer todo**.
3. Abre la carpeta extraída y después la carpeta **PinwiPOS**.
4. Ejecuta **Pinwi.Pos.exe**. Conserva juntos todos sus archivos y subcarpetas; no copies solamente el EXE.
5. En un equipo sin datos previos, crea el primer administrador con tu nombre y una contraseña de al menos ocho caracteres. No hay contraseña predeterminada. Conserva esa contraseña.

Si aparece la pantalla habitual de inicio de sesión, esa cuenta de Windows ya contiene una base del POS. No borres esa base para volver a comenzar. Entra con una cuenta existente o utiliza una restauración autorizada.

## 3. Instalar y crear un acceso de escritorio (opcional)

1. Cierra cualquier ventana del POS en esa computadora. El turno puede permanecer abierto.
2. Dentro de la carpeta extraída, localiza **Instalar.ps1**, al mismo nivel que la carpeta **PinwiPOS**.
3. Haz clic derecho y elige **Ejecutar con PowerShell**. En algunos menús de Windows aparece dentro de **Mostrar más opciones**.
4. El instalador copia el programa en `%LOCALAPPDATA%\Programs\PinwiPOS` y crea **Helados Pinwi Pinwi** en el escritorio.
5. Abre el acceso del escritorio. La instalación es por usuario de Windows; los datos del negocio se conservan en una carpeta separada.

Si PowerShell impide ejecutar el script, puedes usar la apertura directa del apartado 2, siempre que Windows permita ejecutar el programa. Si Windows bloquea el EXE por firma o confianza, conserva el mensaje exacto para resolver la distribución; no desactives protecciones ni instales certificados desconocidos. La firma de esta entrega local sigue pendiente.

## 4. Elegir cómo usar la otra computadora

**Instalación independiente:** crea su administrador y comienza sin las ventas de la computadora original. Incluye el catálogo comercial inicial. Cada PC conserva sus propios precios, cuentas, ventas y cortes.

**Traslado de la computadora principal:** utiliza un respaldo para llevar el estado del negocio. El respaldo contiene cuentas, precios, ventas, cortes, fotos y configuración incluidos en esa copia. No fusiona datos de dos equipos.

El POS no sincroniza computadoras ni sucursales. No compartas `pinwi.db` por una carpeta de red, OneDrive u otra carpeta sincronizada para trabajar simultáneamente. Si sustituyes la computadora principal, deja de registrar operaciones en la anterior cuando generes el respaldo definitivo.

## 5. Llevar tus datos mediante un respaldo

En la computadora original:

1. Finaliza las operaciones que deban ir en el traslado. Para el traspaso definitivo, cierra el turno y registra su conteo real.
2. Entra en **Respaldo > Crear respaldo** y guarda el archivo `.db` en una carpeta local o USB.
3. Si lo guardaste localmente, copia ese archivo a la USB. Usa la opción de respaldo del POS; no copies a mano la base abierta ni sus archivos `-wal` y `-shm`.
4. Conserva otra copia y la computadora original hasta comprobar la recuperación. No borres los datos originales.

En la computadora de destino:

1. Instala o abre exactamente esta versión o una versión posterior compatible. Si está limpia, crea un administrador inicial para poder entrar a la aplicación.
2. Entra como administrador. La caja debe estar cerrada y el carrito actual vacío o guardado como pendiente.
3. Abre **Respaldo > Restaurar respaldo**, selecciona el `.db` y revisa el resumen del archivo.
4. Confirma solo si deseas reemplazar los datos actuales por los de esa copia. Los cambios hechos después del respaldo no se fusionan. Autoriza con la contraseña del administrador actual del equipo de destino.
5. El POS valida el respaldo, conserva una copia del estado previo y restaura. Después se cierra.
6. Vuelve a abrirlo e inicia sesión con una cuenta y contraseña que existieran en el respaldo. El administrador inicial del equipo de destino se habrá reemplazado por las cuentas restauradas.
7. Revisa productos, precios, fotos, ventas y cortes antes de continuar la operación. Ajusta la impresora y el destino externo de respaldos si eran específicos de la computadora anterior.

Los respaldos contienen cuentas y datos del negocio. Guárdalos en un lugar de confianza y no los subas al repositorio público de GitHub.

## 6. Actualizar una computadora que ya usa el POS

1. Genera un respaldo manual y conserva la entrega anterior.
2. Cierra el programa. No sobrescribas una versión mientras está en ejecución.
3. Extrae el ZIP nuevo en una carpeta nueva o ejecuta su instalador con el POS cerrado. No reemplaces ni borres la carpeta de datos.
4. Abre el programa nuevo. Si hace falta una migración de esquema, crea su respaldo previo automáticamente. La versión 1.1.1.0 utiliza SQLite v8.
5. Una vez migrada la base, no abras ejecutables anteriores incompatibles sobre ella. Revisa que se conserven tus datos y continúa con tu usuario habitual.

## 7. Antes de comenzar a cobrar

- Revisa el catálogo y confirma los precios normales y de cobro rápido. Los umbrales iniciales son 10 piezas por gama o $400 valorados a menudeo; pueden haberse modificado en configuración.
- Crea las cuentas de cajero que necesites en **Administración** y utiliza **F12** para bloquear o cambiar de usuario.
- Abre caja con el fondo real. Las existencias iniciales pueden estar en cero: son informativas y no bloquean el cobro.
- Configura una impresora instalada en Windows y el ancho de 58 u 80 mm. La impresión física debe comprobarse con el equipo real; un fallo de impresión no requiere cobrar otra vez.
- Configura una carpeta o USB adicional de respaldos si la necesitas. Comprueba que esté conectada y revisa los avisos de respaldo.

La venta y consulta local no requieren internet. WhatsApp sí requiere internet y pulsar Enviar; no hay envío desatendido ni confirmación remota de entrega. Una copia en el mismo disco no protege frente a la pérdida física de ese disco.

## 8. Si algo no coincide

- **Pide crear administrador cuando esperabas ver tus datos:** revisa que uses la misma cuenta de Windows o restaura el respaldo correcto. Cada cuenta de Windows tiene su propia carpeta de datos.
- **Dice que la base necesita una versión más reciente:** cierra el programa y usa una entrega compatible; no borres ni reviertas la base.
- **Hay otra instancia abierta:** utiliza la ventana existente o ciérrala antes de cambiar de versión.
- **No imprime:** la venta permanece guardada; revisa impresora, papel y controlador y reimprime desde Historial.
- **No aparece firma o Windows lo bloquea:** esa PC puede requerir una distribución firmada; la instalación en esta computadora no garantiza autorización en otra.

Ubicación de datos: `%LOCALAPPDATA%\PinwiPOS`. Incluye la base y sus respaldos. Esta guía no sustituye el manual de operación; tampoco confirma que la versión de Microsoft Store ya esté aprobada o disponible.
