# Distribución gratuita mediante Microsoft Store

## Estado al 16 de noviembre de 2026

El usuario no desea pagar por firma. Se encontró una alternativa oficial gratuita: alta individual en https://storedeveloper.microsoft.com y firma de paquetes MSIX por Microsoft Store después de certificación. Esto corrige la nota anterior de que no había una vía gratuita identificada. No confundir esta opción con Artifact Signing/Azure ni con Microsoft Ignite.

Se abrió el portal correcto y se observó la pantalla de elección Individual developer / Company account, ambas marcadas Free. No se creó cuenta, no se aceptaron términos ni se enviaron datos de identidad. El usuario debe completar la elección, inicio de sesión y verificación. Después hacen falta la reserva de nombre y los valores Identity Name, Publisher y PublisherDisplayName del producto en Partner Center.

## Trabajo preparado

- `packaging/Store/AppxManifest.template.xml`: WPF clásico x64, Windows 11, es-MX y runtime incluido en la publicación. Capacidades runFullTrust y unvirtualizedResources. Solo se excluye de virtualización `%LOCALAPPDATA%\PinwiPOS` para conservar la carpeta y evitar otra base de datos o eliminación de datos al desinstalar. Esta capacidad necesita justificación y aprobación en Store; debe probarse su comportamiento real antes de usar datos operativos.
- Iconos derivados del logo local en `packaging/Store/Assets`.
- `scripts/package-store.ps1`: toma la publicación self-contained con todos los cambios de `dist/CobroRapido/PinwiPOS`, rechaza archivos de base o claves, copia a una carpeta de staging nueva, genera manifiesto y ejecuta MakeAppx con validación normal (sin omitirla). No modifica la publicación original ni el sistema.
- MakeAppx obtenido del paquete oficial NuGet Microsoft.Windows.SDK.BuildTools 10.0.28000.2957; se comprobó firma válida de Microsoft.
- Borrador creado correctamente: `dist/Store/PinwiPOS-1.0.7.0-BORRADOR-SIN-FIRMA-b0024fa7.msix`. Usa identidad provisional: no subir ni instalar. El resultado es validación estructural y de empaquetado, no certificación ni ejecución aprobada.

## Regenerar con identidad de Store

```powershell
./scripts/package-store.ps1 -IdentityName '<Name de Partner Center>' -Publisher '<Publisher de Partner Center>' -PublisherDisplayName '<Nombre público de editor>'
```

Se debe ejecutar la batería final de pruebas e interfaz en un entorno Windows permitido antes de solicitar publicación. No recompilar/renombrar para evadir Smart App Control, instalar certificados autofirmados como raíz ni desactivar seguridad. El paquete de borrador contiene la misma DLL publicada que Windows bloqueó, no un reemplazo para eludir la comprobación.

El envío y publicación externa requieren revisar primero el resultado concreto con el usuario. Aún no se subió ningún archivo ni se eligió disponibilidad pública. Revisar distribución no listada o audiencia restringida según opciones reales de Partner Center y preferencia del usuario. No publicar datos operativos, usuarios, claves, bases ni respaldos. El POS sigue funcionando offline; Store sería el canal de instalación/distribución. Falta validar instalación/actualización sobre copia aislada de base, persistencia externa, impresora y todas las funciones de la última interfaz.

Fuentes oficiales consultadas:
- https://learn.microsoft.com/en-us/windows/apps/publish/whats-new-individual-developer
- https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/code-signing-options
- https://blogs.windows.com/windowsdeveloper/2025/09/10/free-developer-registration-for-individual-developers-on-microsoft-store/
- https://learn.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-manual-conversion
- https://learn.microsoft.com/en-us/windows/msix/desktop/flexible-virtualization
## Registro confirmado y producto reservado

El 16 de noviembre de 2026 se confirmó la sesión del usuario en Google Chrome y el acceso al panel Apps and games de Partner Center. Se reservó «Helados Pinwi Pinwi POS» y se creó Submission 1, ambos en borrador. Store ID: 9N0ZB1RQ1RTR. Identity Name: EdRos.HeladosPinwiPinwiPOS; Publisher: CN=BE31C71B-F8D9-4853-9554-9F8ABF834F72; PublisherDisplayName: EdRos. Son identificadores del producto/editor, no credenciales. No se accedió a tokens ni contraseñas.

Se generó y validó con MakeAppx el paquete con identidad real: dist/Store/PinwiPOS-1.0.7.0-PARA-REVISION-STORE-d0e6f46d.msix, registro en artifacts/store-package-build.log. Continúa sin firma y NO se instaló. No se subió a Microsoft ni se envió a certificación. La última batería de pruebas continuó bloqueada por Windows, así que no afirmar validación funcional completa.

Se revisó disponibilidad: hay audiencia privada y distribución por enlace, pero no se guardó ninguna configuración. El formulario muestra por defecto audiencia pública/mercados mundiales; antes de enviar a certificación hay que elegir explícitamente disponibilidad, precio gratuito y revisión manual del usuario. La vista Application overview confirma todos los apartados aún Not started y Submit for certification deshabilitado. La reserva del nombre y el borrador no equivalen a firma, aprobación ni aplicación instalable.

## Carga autorizada completada
El usuario autorizó explícitamente subir el paquete sin publicar. Tras habilitar personalmente el permiso de archivos de la extensión en Chrome, se cargó el MSIX con identidad real y se pulsó Save. Partner Center confirmó Packages: Complete y el archivo en estado Validated. Aviso: runFullTrust y unvirtualizedResources requieren aprobación. Submission 1 sigue In draft y Submit for certification deshabilitado. No se envió a certificación ni se publicó, no hay firma/instalación aprobada todavía. Faltan ficha, propiedades, clasificación, disponibilidad y opciones de envío, además de validación funcional pendiente. No confundir aceptación estructural con aprobación para ejecutar.

## Envío a certificación realizado

El usuario pidió continuar, solicitó la versión actualizada y cambió explícitamente la disponibilidad a pública («hazlo publico»), reemplazando la preferencia privada. Se completó disponibilidad pública mundial con precio base MXN 0; categoría Business / Sales + marketing; privacidad en texto basada en operación local, respaldos y WhatsApp opcional; ficha es-MX con descripción funcional y una captura de venta de datos ficticios (Ana), sin datos operativos. Las declaraciones de respaldo OneDrive y grabación/broadcast se desmarcaron. No se afirmó validación de accesibilidad ni se inventaron requisitos de RAM/CPU.

Se guardaron justificaciones completas (menos de 500 caracteres cada una) de runFullTrust y unvirtualizedResources, y notas de prueba con límites reales: impresión física, migración MSIX y última interfaz compacta aún sin verificar por bloqueo local de la compilación sin firma. La publicación quedó MANUAL: «Don't publish this submission until I select Publish now».

El usuario confirmó en la pregunta expresa que acepta términos IARC, es mayor de edad y autoriza compartir nombre de editor/correo con IARC para la clasificación. Se guardó la clasificación resultante: 3+ / Everyone. No se creó ni pidió contraseña para ello.

El portal marcó todos los apartados Complete y el paquete Validated. Se pulsó Submit for certification dentro del alcance autorizado de publicación. Se confirmó el estado «In certification» con Submission Complete y Pre-processing In progress. Certification y Publishing aún Not started en esa primera comprobación. El portal indica que suele tomar horas y puede tardar hasta tres días hábiles. El producto solo comenzará a publicarse cuando se pulse Publish now. No está publicado ni se obtuvo todavía un paquete firmado instalable, y no se migró ni modificó la base de operación.

Continuar en Chrome, Partner Center: https://partner.microsoft.com/en-us/dashboard/products/9N0ZB1RQ1RTR/overview . Producto 9N0ZB1RQ1RTR, Submission 1. Revisar estado y posibles observaciones antes de ofrecer enlace de instalación como disponible. La autorización del usuario es para hacerlo público, pero no afirmar que una revisión pendiente ya aprobó el programa. No crear una automatización para este hito. La versión anterior operativa sigue en dist/Productos; nunca abrir una versión con esquema menor que la base.


## Actualización autorizada a 1.1.1.0 — 16 de noviembre de 2026

El usuario pidió sustituir el envío anterior por la versión más reciente con todos los cambios. Se usó Chrome, perfil edgar, con la sesión existente. Se canceló la certificación de 1.0.7.0 para recuperar el borrador y se preparó `dist/Store/PinwiPOS-1.1.1.0-PARA-REVISION-STORE-0fe4f4ea.msix` desde `dist/OperacionSegura-v111/PinwiPOS`, con la misma identidad real del producto y versión 1.1.1.0.

MakeAppx terminó con validación aprobada. Se verificó que EXE, DLL y catálogos contenidos coinciden por SHA-256 con la entrega local, y que el MSIX no incluye bases, claves ni reportes. El script de empaquetado ahora usa la entrega actual y rechaza discrepancias entre versión del manifiesto y binarios.

Se cargó el MSIX y el panel principal lo marcó Validated. La página de carga quedó temporalmente en Analyzing package; al confirmarse el resultado en el panel principal se recargó esa página, se guardó y se confirmó que solo quedó 1.1.1.0. La 1.0.7.0 se retiró del envío, conservándose sus archivos locales históricos.

Se actualizaron descripción, novedades y descripción corta de la ficha es-MX, además de las instrucciones de prueba para certificadores. No se cargaron credenciales. Las notas distinguen pruebas locales/aisladas aprobadas de impresión física, lector físico e instalación/actualización MSIX en un segundo equipo, que siguen sin verificar. Se conservaron las justificaciones de runFullTrust y unvirtualizedResources.

Se comprobó la opción «Don't publish this submission until I select Publish now». Se pulsó Submit for certification y el portal confirmó **In certification**, Submission Complete, Pre-processing In progress, Certification Not started y Publishing Not started. El aviso indica que la publicación comenzará solo al seleccionar Publish now. No se pulsó ese botón, no está publicado y no se obtuvo todavía un paquete firmado instalable. El EXE/DLL locales continúan sin firma. La base de operación y la entrega local no se instalaron ni sustituyeron durante este trámite.

Continuar desde https://partner.microsoft.com/en-us/dashboard/products/9N0ZB1RQ1RTR/overview. Mantener publicación retenida; no confundir Validated o In certification con aprobación, firma disponible o publicación.


## Autorización posterior para publicar — 16 de noviembre de 2026

El usuario indicó expresamente «publicalo», reemplazando la retención manual anterior. Se revisó el producto 9N0ZB1RQ1RTR en Chrome, perfil edgar. La versión enviada continúa en certificación: Certification In progress y Publishing Not started; todavía no hay disponibilidad pública confirmada.

Tras revisar Modify publishing time, el portal mostró «Your product will start publishing as soon as it passes certification». Se recargó la página y se confirmó que esa configuración persistía. Por tanto, la publicación comenzará automáticamente cuando Microsoft apruebe; no hace falta mantener el freno manual histórico. No se pulsó un botón Publish now habilitado ni se afirmó una publicación ya completada.

Evidencia local ignorada: `artifacts/store-v111-publicacion-automatica.jpg`. La aprobación, el inicio efectivo de Publishing y la disponibilidad en Store siguen pendientes de Microsoft. No modificar ni cancelar esta autorización sin nueva indicación del usuario.

## Rechazo 10.6.3 y reconsideración enviada — 19 de noviembre de 2026

Se revisó el informe https://partner.microsoft.com/en-us/dashboard/products/9N0ZB1RQ1RTR/certification/reports/11691c1a-cd5c-4e35-9b8c-257cd290185c. Microsoft rechazó la capacidad restringida `unvirtualizedResources` según la información aportada. Solicitó información nueva o ampliada y un contacto de soporte o sitio del desarrollador válido en Properties > Support info. No fue un diagnóstico de firma criptográfica defectuosa. El envío volvió a In draft y ofrecía Resubmit for certification.

El usuario pidió encargarse de la corrección. Se publicó `SUPPORT.md` en el repositorio público propio y se enlazó desde README. Identifica al editor EdRos, enlaza los manuales y el canal habilitado de GitHub Issues, y advierte contra publicar bases, respaldos, credenciales o transacciones. No se inventaron correo, dirección ni compromisos de atención. Se comprobó que la página era accesible públicamente.

Se guardaron y comprobaron en Properties:

- Website: https://github.com/Reingardo/pos-helados-pinwi-pinwi
- Support contact info: https://github.com/Reingardo/pos-helados-pinwi-pinwi/blob/main/SUPPORT.md

Se sustituyó la justificación corta de `unvirtualizedResources` por una solicitud de reconsideración nueva, dentro del límite de 500 caracteres. Se mantuvo `runFullTrust`. Las notas de certificación se ampliaron dentro de 6,000 caracteres (5,776 guardados y comprobados tras volver a abrir la página): historial autoritativo existente de la distribución local, riesgo de una vista de datos separada al virtualizar, conservación de registros y respaldos tras desinstalar, exclusión únicamente de `%LOCALAPPDATA%\PinwiPOS`, permisos normales del usuario y ausencia de exclusiones de registro o AppData general. Se describió un escenario de revisión aislado sin afirmar que la migración MSIX en segundo equipo ya estuviera verificada. Se conservaron instrucciones funcionales y límites de impresión/lector físicos. No se cargaron credenciales.

Se retiró de las notas la instrucción histórica de retener la publicación, para hacerla coherente con la autorización posterior del usuario y la opción automática seleccionada. No se modificaron el manifiesto, los binarios, la carpeta de operación ni la base de datos. La reconsideración utiliza el mismo paquete `PinwiPOS-1.1.1.0-PARA-REVISION-STORE-0fe4f4ea.msix`, que seguía Validated.

Se pulsó **Resubmit for certification** y se observó **In certification**, Submission Complete, Pre-processing In progress, Certification Not started y Publishing Not started. El portal confirmó nuevamente «Your product will start publishing as soon as it passes certification». La captura de comprobación está en `artifacts/store-reconsideracion-v111.jpg` (ignorada por Git). La reconsideración está enviada; la aprobación del permiso, firma descargable y disponibilidad pública siguen pendientes de Microsoft. Continuar desde https://partner.microsoft.com/en-us/dashboard/products/9N0ZB1RQ1RTR/overview sin cancelar ni cambiar la publicación automática.

## Segundo rechazo y corrección 1.1.2.0 — 23 de noviembre de 2026

El correo nuevo se localizó en Action Center > Notifications (aviso del 23 de noviembre). Su enlace abrió el informe https://partner.microsoft.com/en-us/dashboard/products/9N0ZB1RQ1RTR/certification/reports/f3b005b3-0fc6-4401-abe1-4a9451b279b2, que repite 10.6.3: rechazo de unvirtualizedResources y petición de contacto de soporte/sitio válido, sin explicar por qué rechazó los enlaces ya guardados. El panel general todavía decía In certification; ese resumen no sustituyó la observación del informe. El usuario pidió «resulvela».

La corrección retira la capacidad rechazada y todas las exclusiones de virtualización. El único permiso del MSIX es runFullTrust. DataLocation detecta identidad de paquete mediante GetCurrentPackageFullName: la edición empaquetada guarda en `%USERPROFILE%\PinwiPOS-Store`, fuera de AppData; la local mantiene `%LOCALAPPDATA%\PinwiPOS`. No hay traslado silencioso ni base compartida. La bienvenida inicial ofrece importar un respaldo elegido/confirmado o comenzar independiente. La importación usa origen SQLite de solo lectura, snapshot, validación de integridad/esquema/administrador, migración aislada y publicación sin sobrescritura en destino vacío; conserva origen, cuentas, roles, fotos, historial y pendientes. Restauraciones posteriores siguen requiriendo autorización y copia previa. Se corrigió el cierre del archivo de respaldo (`Pooling=false` en destino) tras detectar una retención del archivo en la primera ejecución CI.

La compilación local pasó, pero Windows bloqueó el EXE de pruebas. No se intentó eludir esa política ni modificar seguridad. El CI Windows https://github.com/Reingardo/pos-helados-pinwi-pinwi/actions/runs/45305436247, commit `86308b4`, finalizó **success**, incluidas todas las pruebas anteriores, nuevas pruebas de distribución/importación y renders WPF. Se descargó y revisó solo la vista sin cuentas/operaciones de bienvenida (`artifacts/store-first-use-v112/store-bienvenida.png`). No equivale a prueba interactiva completa del asistente ni a instalar MSIX firmado en segundo equipo limpio; impresora y lector físicos siguen pendientes.

Publicación self-contained separada: `dist/Store-v112/PinwiPOS`. Paquete final: `dist/Store/PinwiPOS-1.1.2.0-PARA-REVISION-STORE-64e33451.msix` (85,198,176 bytes). MakeAppx aprobó la estructura. Se inspeccionó el ZIP: solo runFullTrust, versión 1.1.2.0, sin exclusiones; EXE/DLL/catálogos iguales por SHA-256 a la publicación; sin bases, claves, CSV ni reportes. No se instaló el paquete ni se sustituyeron el EXE local o la base de operación. La base continúa en v8.

Se canceló la certificación anterior para modificar Submission 1, que quedó en **In draft**. Se guardaron notas 1.1.2.0 (5,613 caracteres, comprobadas al reabrir) con la eliminación explícita del permiso, nueva ruta, primer uso, pruebas y límites. Properties guardó privacidad actualizada y enlaces directos: Website `https://github.com/Reingardo/pos-helados-pinwi-pinwi/blob/main/SUPPORT.md`; Support contact `https://github.com/Reingardo/pos-helados-pinwi-pinwi/issues`.

**Pendiente de carga manual:** fileChooser.setFiles falló dos veces por permiso de URL de archivos de la extensión; el usuario dijo haberlo habilitado, pero el rechazo persistió. No se cambió automáticamente ese permiso. No apareció un selector nativo al intentar la apertura normal. Se pidió seleccionar el MSIX final manualmente en Packages; el usuario pidió mostrar la pestaña. El borrador aún contiene **1.1.1.0**, por lo que no debe enviarse todavía. Después de la selección manual hay que comprobar validación de 1.1.2.0, retirar solo 1.1.1.0 del envío, guardar Packages, revisar Submission Options con solo runFullTrust y publicación automática, actualizar novedades es-MX y reenviar. No reenviar el paquete viejo con las notas nuevas. No se ha confirmado firma ni aprobación.
