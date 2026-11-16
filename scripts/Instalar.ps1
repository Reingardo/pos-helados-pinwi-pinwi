$ErrorActionPreference = 'Stop'
$source = Join-Path $PSScriptRoot 'PinwiPOS'
$destination = Join-Path $env:LOCALAPPDATA 'Programs\PinwiPOS'
if (!(Test-Path (Join-Path $source 'Pinwi.Pos.exe'))) { throw 'Extrae todo el ZIP antes de instalar.' }
New-Item -ItemType Directory -Force $destination | Out-Null
Copy-Item "$source\*" $destination -Recurse -Force
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut((Join-Path ([Environment]::GetFolderPath('Desktop')) 'Helados Pinwi Pinwi.lnk'))
$shortcut.TargetPath = Join-Path $destination 'Pinwi.Pos.exe'
$shortcut.WorkingDirectory = $destination
$shortcut.Save()
Write-Host 'Instalado. Abre Helados Pinwi Pinwi desde el escritorio.'
