# Capturas públicas del POS

Imágenes de la interfaz WPF de la versión 1.1.1.0, preparadas el 20 de noviembre de 2026 para README y la guía visual. Se renderizan los controles reales contra un catálogo temporal nuevo. No se usan la ventana ni la base de operación.

El generador no crea usuarios, contraseñas, turnos, ventas, cortes ni gastos. El único carrito de ejemplo permanece en memoria y no se confirma. Las pantallas principales llevan una leyenda de demostración; los formularios conservan sus campos de autorización vacíos. No se publica una base ni un registro de pruebas.

Para reproducir desde la raíz del repositorio, con SDK .NET 10 en Windows:

```powershell
dotnet run --project tests/Pinwi.Tests -c Release -- --docs
```

Las capturas se generan primero en `artifacts/documentacion` (ignorado por Git). Revísalas visualmente antes de copiar los ocho PNG a esta carpeta. No copies imágenes del modo `--render`: ese modo también incluye operaciones de prueba que no deben publicarse.

Origen de las imágenes comerciales utilizadas dentro de la interfaz: [image-sources.json](../../src/Pinwi.Pos/Assets/image-sources.json). No se generaron nuevas fotografías para esta documentación.
