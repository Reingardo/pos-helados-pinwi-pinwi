$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$dotnet = Join-Path $env:LOCALAPPDATA 'Microsoft\dotnet\dotnet.exe'
if (!(Test-Path $dotnet)) { $dotnet = 'dotnet' }
& $dotnet run --project "$repo\tests\Pinwi.Tests" -c Release
if ($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas' }
& $dotnet publish "$repo\src\Pinwi.Pos" -c Release -r win-x64 --self-contained true -o "$repo\dist\Actualizacion\PinwiPOS"
if ($LASTEXITCODE -ne 0) { throw 'Falló la publicación' }
Copy-Item "$PSScriptRoot\Instalar.ps1" "$repo\dist\Actualizacion\Instalar.ps1" -Force
Compress-Archive -Path "$repo\dist\Actualizacion\PinwiPOS","$repo\dist\Actualizacion\Instalar.ps1" -DestinationPath "$repo\dist\PinwiPOS-Actualizado.zip" -Force
