param([ValidatePattern('^[A-Za-z0-9_-]+$')][string]$Delivery = 'OperacionSegura-v111')
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$dotnet = Join-Path $env:LOCALAPPDATA 'Microsoft\dotnet\dotnet.exe'
if (!(Test-Path -LiteralPath $dotnet)) { $dotnet = 'dotnet' }
$package = [IO.Path]::GetFullPath((Join-Path $repo "dist\$Delivery"))
$output = Join-Path $package 'PinwiPOS'
$running = Get-Process 'Pinwi.Pos' -ErrorAction SilentlyContinue | Where-Object { $_.Path -and $_.Path.StartsWith($output + '\', [StringComparison]::OrdinalIgnoreCase) }
if ($running) { throw 'La entrega está en ejecución. Ciérrala antes de volver a compilar esa carpeta.' }
New-Item -ItemType Directory -Force (Join-Path $repo 'artifacts') | Out-Null
& $dotnet run --project "$repo\tests\Pinwi.Tests" -c Release 2>&1 | Tee-Object (Join-Path $repo 'artifacts\pruebas-operacion.txt')
if ($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas; no se generó la entrega.' }
& $dotnet publish "$repo\src\Pinwi.Pos" -c Release -r win-x64 --self-contained true -o $output
if ($LASTEXITCODE -ne 0) { throw 'Falló la publicación local.' }
Copy-Item -LiteralPath "$PSScriptRoot\Instalar.ps1" -Destination (Join-Path $package 'Instalar.ps1') -Force
Copy-Item -LiteralPath "$repo\README.md","$repo\MANUAL-OPERACION.md","$repo\MANUAL-INSTALACION.md","$repo\ESTADO-OPERACION-V8.md" -Destination $package -Force
$archive = Join-Path $repo "dist\PinwiPOS-$Delivery.zip"
$packageItems = @($output,(Join-Path $package 'Instalar.ps1'),(Join-Path $package 'README.md'),(Join-Path $package 'MANUAL-OPERACION.md'),(Join-Path $package 'MANUAL-INSTALACION.md'),(Join-Path $package 'ESTADO-OPERACION-V8.md'))
$installationPdf = Join-Path $repo 'output\pdf\PinwiPOS-Manual-Instalacion.pdf'
if (Test-Path -LiteralPath $installationPdf) {
  $installationCopy = Join-Path $package 'Manual-Instalacion-PinwiPOS.pdf'
  Copy-Item -LiteralPath $installationPdf -Destination $installationCopy -Force
  $packageItems += $installationCopy
}
Compress-Archive -LiteralPath $packageItems -DestinationPath $archive -Force
Write-Host "Entrega local: $output"
Write-Host "Paquete sin firma: $archive"
