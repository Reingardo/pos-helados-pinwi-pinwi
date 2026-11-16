[CmdletBinding()]
param(
 [Parameter(Mandatory=$true)][string]$CertificateThumbprint,
 [string]$SourceDirectory=(Join-Path (Split-Path $PSScriptRoot -Parent) 'dist\CobroRapido\PinwiPOS'),
 [string]$OutputDirectory=(Join-Path (Split-Path $PSScriptRoot -Parent) 'dist\Firmado\PinwiPOS'),
 [string]$SignToolPath,
 [uri]$TimestampServer='http://timestamp.digicert.com'
)
$ErrorActionPreference='Stop'
$thumbprint=$CertificateThumbprint.Replace(' ','')
if($thumbprint -notmatch '^[A-Fa-f0-9]{40}$'){throw 'Indica la huella SHA-1 del certificado de firma emitido por el proveedor.'}
$certificate=Get-Item -LiteralPath "Cert:\CurrentUser\My\$thumbprint" -ErrorAction SilentlyContinue
if(!$certificate -or !$certificate.HasPrivateKey){throw 'No está disponible el certificado con su clave de firma. Inicia la sesión del token o proveedor de firma.'}
if($certificate.NotBefore -gt (Get-Date) -or $certificate.NotAfter -le (Get-Date)){throw 'El certificado no está vigente.'}
if(!($certificate.EnhancedKeyUsageList | Where-Object {$_.ObjectId -eq '1.3.6.1.5.5.7.3.3'})){throw 'El certificado no autoriza firma de código.'}
$chain=[System.Security.Cryptography.X509Certificates.X509Chain]::new()
try { if(!$chain.Build($certificate)){throw 'No se pudo validar la cadena de confianza del certificado.'} } finally {$chain.Dispose()}
$source=(Resolve-Path -LiteralPath $SourceDirectory).Path
$output=[IO.Path]::GetFullPath($OutputDirectory)
if($output.Equals($source,[StringComparison]::OrdinalIgnoreCase) -or $output.StartsWith($source.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Usa una carpeta de salida separada de la aplicación original.'}
if(Test-Path -LiteralPath $output){throw 'La carpeta de salida ya existe. Elige otra para conservar la entrega anterior.'}
if(!$SignToolPath){$tool=Get-Command signtool.exe -ErrorAction SilentlyContinue;if($tool){$SignToolPath=$tool.Source}}
if(!$SignToolPath){$kits=Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin';$SignToolPath=Get-ChildItem -LiteralPath $kits -Filter signtool.exe -Recurse -ErrorAction SilentlyContinue | Where-Object {$_.Directory.Name -eq 'x64'} | Sort-Object FullName -Descending | Select-Object -First 1 -ExpandProperty FullName}
if(!$SignToolPath -or !(Test-Path -LiteralPath $SignToolPath)){throw 'Falta SignTool de Windows SDK. Indica su ruta con -SignToolPath.'}
$files=@('Pinwi.Pos.exe','Pinwi.Pos.dll')
foreach($file in $files){if(!(Test-Path -LiteralPath (Join-Path $source $file))){throw "Falta $file en la aplicación publicada."}}
New-Item -ItemType Directory -Path $output | Out-Null
Get-ChildItem -LiteralPath $source -Force | Copy-Item -Destination $output -Recurse
foreach($file in $files){
 $path=Join-Path $output $file
 & $SignToolPath sign /sha1 $thumbprint /fd SHA256 /tr $TimestampServer.AbsoluteUri /td SHA256 $path
 if($LASTEXITCODE -ne 0){throw "Falló la firma de $file. La copia de salida está incompleta; no distribuirla."}
 & $SignToolPath verify /pa /all /v $path
 if($LASTEXITCODE -ne 0){throw "No se pudo verificar la firma de $file. No distribuir la salida."}
 $signature=Get-AuthenticodeSignature -LiteralPath $path
 if($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Thumbprint -ne $thumbprint -or !$signature.TimeStamperCertificate){throw "Firma o sello de tiempo inválidos para $file."}
}
Write-Host "Archivos propios firmados y verificados en $output. Falta probar la apertura y el flujo del POS antes de distribuir."
