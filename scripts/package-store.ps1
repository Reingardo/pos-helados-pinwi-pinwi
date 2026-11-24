[CmdletBinding()]
param(
 [string]$IdentityName,
 [string]$Publisher,
 [string]$PublisherDisplayName,
 [version]$Version='1.1.2.0',
 [string]$SourceDirectory,
 [string]$MakeAppxPath,
 [switch]$Preview
)
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
if($Preview){$IdentityName='PinwiPOS.Preview';$Publisher='CN=PinwiPOSPreview';$PublisherDisplayName='Identidad provisional - no publicar'}
if(!$IdentityName -or !$Publisher -or !$PublisherDisplayName){throw 'Copia Identity Name, Publisher y PublisherDisplayName de Identidad del producto en Partner Center, o usa -Preview para validar un borrador local.'}
if($IdentityName -notmatch '^[A-Za-z0-9.-]{3,50}$'){throw 'Identity Name debe coincidir con Partner Center y tener entre 3 y 50 caracteres.'}
if($Version.Revision -ne 0){throw 'La versión para Store debe tener cuatro partes y terminar en .0.'}
if(!$SourceDirectory){$SourceDirectory=Join-Path $repo 'dist\Store-v112\PinwiPOS'}
$source=(Resolve-Path -LiteralPath $SourceDirectory).Path
foreach($required in @('Pinwi.Pos.exe','Pinwi.Pos.dll','coreclr.dll','catalog.json','packages.json')){if(!(Test-Path -LiteralPath (Join-Path $source $required))){throw "Publicación incompleta: falta $required"}}
$private=Get-ChildItem -LiteralPath $source -Recurse -File | Where-Object {$_.Extension -in '.db','.db-wal','.db-shm','.sqlite','.sqlite3','.pfx','.p12','.key','.env','.csv','.log' -or $_.Name -like '.env.*'}
if($private){throw 'La carpeta contiene datos o claves que no deben distribuirse.'}
foreach($binary in @('Pinwi.Pos.exe','Pinwi.Pos.dll')){if([version](Get-Item -LiteralPath (Join-Path $source $binary)).VersionInfo.FileVersion -ne $Version){throw "La versión del manifiesto no coincide con $binary. No se preparó el paquete."}}
if(!$MakeAppxPath){$tool=Get-Command makeappx.exe -ErrorAction SilentlyContinue;if($tool){$MakeAppxPath=$tool.Source}}
if(!$MakeAppxPath){$MakeAppxPath=Get-ChildItem (Join-Path $repo '.tools\windows-sdk-buildtools\bin') -Filter makeappx.exe -Recurse -ErrorAction SilentlyContinue | Where-Object {$_.Directory.Name -eq 'x64'} | Sort-Object FullName -Descending | Select-Object -First 1 -ExpandProperty FullName}
if(!$MakeAppxPath){throw 'Falta MakeAppx de Microsoft Windows SDK. Indica su ruta con -MakeAppxPath.'}
if((Get-AuthenticodeSignature -LiteralPath $MakeAppxPath).Status -ne 'Valid'){throw 'La firma de MakeAppx no es válida.'}
$base=Join-Path $repo 'dist\Store'
$stage=Join-Path $base ('staging-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage -Force | Out-Null
Get-ChildItem -LiteralPath $source -Force | Copy-Item -Destination $stage -Recurse
Copy-Item (Join-Path $repo 'packaging\Store\Assets') (Join-Path $stage 'StoreAssets') -Recurse
$manifest=Get-Content (Join-Path $repo 'packaging\Store\AppxManifest.template.xml') -Raw
$manifest=$manifest.Replace('__IDENTITY__',[Security.SecurityElement]::Escape($IdentityName)).Replace('__PUBLISHER__',[Security.SecurityElement]::Escape($Publisher)).Replace('__DISPLAY_PUBLISHER__',[Security.SecurityElement]::Escape($PublisherDisplayName)).Replace('__VERSION__',$Version.ToString())
Set-Content -LiteralPath (Join-Path $stage 'AppxManifest.xml') -Value $manifest -Encoding utf8
$suffix=if($Preview){'BORRADOR-SIN-FIRMA'}else{'PARA-REVISION-STORE'}
$output=Join-Path $base ("PinwiPOS-$Version-$suffix-$([guid]::NewGuid().ToString('N').Substring(0,8)).msix")
& $MakeAppxPath pack /d $stage /p $output
if($LASTEXITCODE -ne 0){throw 'MakeAppx rechazó el manifiesto o el paquete. No subir a Store.'}
Write-Host "Paquete validado por MakeAppx: $output"
Write-Host 'SIN FIRMA: no instalar localmente ni sustituir la versión operativa. Falta identidad de Store (si es borrador), validación funcional, envío y aprobación de Microsoft.'
