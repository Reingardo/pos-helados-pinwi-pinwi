**Distribución gratuita pendiente:** se preparó un MSIX borrador con los cambios actuales y validación de MakeAppx aprobada. La firma gratuita puede obtenerse tras certificación de Microsoft Store; el alta individual es gratuita. El usuario debe completar la verificación en https://storedeveloper.microsoft.com. No hay cuenta creada ni paquete enviado/firmado. Consultar DISTRIBUCION-STORE.md. No presentar el MSIX provisional como instalable o desbloqueado. La versión Productos que se abrió sigue sin cobro rápido. Los datos operativos no se modificaron en esta preparación.
# Firma del POS: preparación y requisito pendiente

El 16 de noviembre de 2026 el usuario confirmó residencia legal en México. La documentación oficial consultada de Microsoft limita Public Trust individual de Artifact Signing a Estados Unidos y Canadá; México tampoco figura entre sus países admitidos para organizaciones. No crear una cuenta de firma de Azure esperando resolver este caso.

No se encontró certificado de firma en CurrentUser/My ni SignTool instalado. Se agregó scripts/sign-release.ps1 para un certificado de firma de código confiable accesible mediante el almacén de Windows (por ejemplo, token o proveedor compatible). Comprueba vigencia, uso para firma y cadena de confianza; copia la publicación a una carpeta nueva, firma Pinwi.Pos.exe y Pinwi.Pos.dll con SHA-256 y sello de tiempo, y verifica ambas firmas. No instala certificados ni modifica políticas. Validada sintaxis y rechazo del caso sin certificado; firma real pendiente de certificado/proveedor y SignTool.

La alternativa investigada es Certum Standard Code Signing in the Cloud, precio publicado desde 209 EUR. Requiere adquisición y verificación de identidad; la elegibilidad concreta y el importe final deben confirmarse en su proceso antes del pago. No se creó cuenta, no se enviaron datos personales y no se hizo compra. Tampoco se firmaron todavía los archivos. La firma no sustituye las pruebas de apertura, interfaz e impresión.

Fuentes:
- https://learn.microsoft.com/en-us/azure/artifact-signing/quickstart
- https://shop.certum.eu/standard-code-signing-in-the-cloud.html
- https://support.certum.eu/en/code-signing-required-documents/
- https://learn.microsoft.com/en-us/dotnet/framework/tools/signtool-exe
