# Instalar Helados Pinwi Pinwi POS 1.1.2.0

La versión actual usa SQLite v8. Microsoft Store ya está publicado; el ZIP es una alternativa local sin firma. La instalación completa en una segunda PC limpia, sin internet, y la impresión física siguen pendientes de comprobar.

## Opción recomendada: Microsoft Store

1. En la computadora de destino con **Windows 11 de 64 bits**, abre [Helados Pinwi Pinwi POS en Microsoft Store](https://apps.microsoft.com/detail/9N0ZB1RQ1RTR).
2. Pulsa **Ver en Microsoft Store / Obtener / Instalar**, según la pantalla. Necesitas internet para descargarlo.
3. Abre **Helados Pinwi Pinwi POS** desde Inicio.
4. En la bienvenida, elige **Comenzar una instalación independiente** si esa PC tendrá sus propios datos. Confirma y crea al primer administrador con tu nombre y una contraseña de al menos ocho caracteres. No hay contraseña predeterminada.
5. Si vas a trasladar un negocio existente, elige **Traer un respaldo del POS** y sigue el apartado de traslado de esta guía.

La venta, consulta y respaldos funcionan sin internet después de instalar. Los pagos con tarjeta se realizan en una terminal externa. WhatsApp requiere internet y pulsar Enviar manualmente.

## Alternativa: descargar el ZIP local

1. [Descarga PinwiPOS-1.1.2.0-Windows-x64.zip](https://github.com/Reingardo/pos-helados-pinwi-pinwi/releases/download/v1.1.2.0/PinwiPOS-1.1.2.0-Windows-x64.zip) desde [la publicación oficial de GitHub](https://github.com/Reingardo/pos-helados-pinwi-pinwi/releases/tag/v1.1.2.0). No elijas **Source code**: esos archivos son para desarrolladores.
2. Copia el ZIP a una carpeta local de la PC de destino; también puedes llevarlo en USB. Haz clic derecho y elige **Extraer todo**.
3. Abre la carpeta **PinwiPOS** y ejecuta **Pinwi.Pos.exe**. Conserva juntos todos los archivos y subcarpetas; no copies solo el EXE.
4. En una cuenta de Windows sin datos previos, crea al primer administrador. Si aparece el inicio de sesión habitual, esa cuenta ya tiene datos: entra con su usuario; no borres la base para comenzar otra vez.

El ZIP incluye .NET, SQLite, catálogo e imágenes. No necesitas el SDK de desarrollo ni descargar componentes para operar. **El ZIP no tiene firma de distribución** y Windows puede bloquearlo. Si ocurre, utiliza Microsoft Store; no desactives protecciones ni instales certificados desconocidos. La instalación correcta en una PC no garantiza autorización en otra.

### Acceso en el escritorio con el ZIP

1. Cierra todas las ventanas del POS.
2. En la carpeta extraída, al mismo nivel que **PinwiPOS**, haz clic derecho en **Instalar.ps1 → Ejecutar con PowerShell**. Puede aparecer en **Mostrar más opciones**.
3. El instalador copia el programa a `%LOCALAPPDATA%\Programs\PinwiPOS` y crea **Helados Pinwi Pinwi** en el escritorio. Instala solo para tu usuario de Windows y conserva los datos separados.
4. Abre el acceso. Si Windows impide ejecutar el script, puedes usar el EXE directamente cuando Windows lo permita, o instalar desde Store.

## Edición Microsoft Store 1.1.2.0: primer inicio y traslado

La edición Store guarda la base y los respaldos en `%USERPROFILE%\PinwiPOS-Store`. La edición local usa `%LOCALAPPDATA%\PinwiPOS`. **Son datos independientes y no se sincronizan.** Una actualización entre versiones locales conserva la carpeta local; cambiar a Store requiere importar un respaldo si quieres conservar el negocio.

En la computadora original:

1. Termina las operaciones que deban ir en el traslado. Para el traslado definitivo, cierra el turno registrando el conteo real.
2. Entra en **Respaldo → Crear respaldo** y conserva el archivo `.db` en una carpeta local o USB. Usa la opción del POS; no copies a mano la base abierta ni sus archivos `-wal` o `-shm`.
3. Guarda otra copia y conserva la computadora original. Deja de registrar operaciones allí después del respaldo definitivo.
4. Cierra el POS antes de abrir otra edición.

En la instalación nueva de Store:

1. En la bienvenida elige **Traer un respaldo del POS**.
2. Selecciona el `.db` generado por el POS y confirma el traslado. Se valida una copia aislada y se migra si corresponde. El archivo original permanece intacto.
3. Entra con una cuenta y contraseña que existieran en el respaldo.
4. Revisa precios, fotos, historial, cuentas y pendientes. Ajusta impresora y carpeta adicional de respaldos si eran específicas de la otra PC. Comprueba la recuperación antes de seguir vendiendo.

Si eliges **Salir** en la bienvenida, no se crea una base de operación. La importación inicial rechaza bases corruptas, versiones futuras y copias sin administrador recuperable; no sustituye datos existentes. Conserva aparte los cortes exportados y la colección de respaldos anterior: la importación lleva el estado contenido en el `.db`, no esos archivos adicionales.

### Si la PC de destino ya tiene datos, o usas el ZIP

1. En una instalación local limpia, crea al administrador inicial para entrar. En una instalación existente, entra con su administrador actual.
2. La caja debe estar cerrada y el carrito vacío o guardado como pendiente.
3. Abre **Respaldo → Restaurar respaldo**, elige el archivo y revisa su resumen.
4. Confirma solo si deseas reemplazar los datos actuales; no se fusionan operaciones. Autoriza con la contraseña del administrador actual del destino.
5. El POS valida y conserva una copia previa, restaura y se cierra. Vuelve a abrirlo e inicia sesión con las cuentas del respaldo, que sustituyen a las anteriores.
6. Comprueba los datos antes de operar. No borres las carpetas originales.

Los respaldos contienen cuentas y datos del negocio. No los subas a GitHub. Cada PC conserva sus propios datos: el POS no sincroniza sucursales. **No compartas `pinwi.db` por red, OneDrive ni carpetas sincronizadas para trabajar simultáneamente.**

## Actualizar una instalación local existente

1. Crea un respaldo manual. Cierra el POS y conserva la entrega anterior.
2. Extrae el ZIP nuevo en otra carpeta, o ejecuta su instalador con el POS cerrado. No reemplaces ni borres la carpeta de datos.
3. Abre el programa nuevo y usa tu cuenta habitual. La versión 1.1.2.0 mantiene SQLite v8; una migración necesaria desde esquemas anteriores crea un respaldo previo.
4. Revisa que todo se conserve. No abras ejecutables antiguos incompatibles sobre una base migrada.

## Antes de comenzar a cobrar

- Revisa los precios normales y de cobro rápido. Los umbrales iniciales son 10 piezas por gama o $400 valorados a menudeo; el administrador puede modificarlos.
- Crea los cajeros en **Administración**. **F12** bloquea o cambia de usuario.
- Abre caja con el fondo real. Las existencias son informativas y no bloquean el cobro.
- Configura la impresora instalada en Windows y papel de 58 u 80 mm. Prueba la impresión real; un fallo no requiere volver a cobrar.
- Configura y comprueba un destino externo de respaldos. Una copia en el mismo disco no protege frente a la pérdida de ese disco.

## Si algo no coincide

- **Pide crear administrador y esperabas tus datos:** comprueba la cuenta de Windows y la edición que abriste. Store y ZIP usan carpetas diferentes; importa o restaura el respaldo correcto.
- **La base necesita una versión más reciente:** usa una entrega compatible; no borres ni reviertas la base.
- **Hay otra instancia abierta:** utiliza o cierra la ventana existente antes de cambiar de edición.
- **No imprime:** la venta permanece guardada; revisa papel, controlador e impresora y reimprime desde Historial.
- **Windows bloquea el ZIP:** instala desde Microsoft Store.

[Manual de cajero y administrador](MANUAL-OPERACION.md) · [Guía con imágenes](docs/GUIA-VISUAL.md) · [Soporte](SUPPORT.md)
