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
