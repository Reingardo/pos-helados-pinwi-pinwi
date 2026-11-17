# Helados Pinwi Pinwi — POS para Windows

Aplicación de escritorio local en C# / .NET 10, WPF y SQLite. Funciona sin internet y conserva menú, imágenes, precios, ventas y fotos en el equipo del negocio. Repositorio independiente: https://github.com/Reingardo/pos-helados-pinwi-pinwi.

**Entrega local actual: Operación Segura 1.1.1.0, base v8.** Lee [AGENTS.md](AGENTS.md) y el [estado de validación](ESTADO-OPERACION-V8.md) antes de continuar. Microsoft Store tiene la 1.1.1.0 en certificación. Por autorización posterior del usuario, comenzará a publicarse automáticamente cuando Microsoft la apruebe; todavía no se confirma disponibilidad pública.

## Abrir o actualizar

Cierra la versión anterior y abre `dist/OperacionSegura-v111/PinwiPOS/Pinwi.Pos.exe`. El ZIP es `dist/PinwiPOS-OperacionSegura-v111.zip`; extrae todo su contenido. `Instalar.ps1` instala por usuario y crea el acceso de escritorio. El paquete es Windows x64 self-contained y no necesita descargar .NET para operar.

La apertura migra a v8 con respaldo previo. Los datos están en `%LOCALAPPDATA%/PinwiPOS`, separados del programa. No abras entregas anteriores sobre la base migrada. No borres ni reviertas datos para resolver errores. El paquete local sigue sin firma; no se cambian protecciones de Windows.

Una instalación limpia crea al primer administrador sin credenciales predeterminadas y sin ventas de prueba. Selecciona el usuario al entrar; las cuentas de cajero se crean desde Administración.

## Funciones disponibles

- 76 productos individuales iniciales, cinco paquetes y siete entradas de cobro rápido por gama, más altas del administrador. Fotos locales y personalizadas incluidas en respaldo.
- Mayoreo desde 10 piezas de la misma gama, mezclando sabores y venta normal/rápida; esquimales por presentación. Umbral global desde $400 a menudeo, sin perderlo al bajar el total ajustado. Umbrales configurables para ventas nuevas.
- Cobro en efectivo, tarjeta externa o mixto, sugerencias y cambio antes de confirmar. Existencias informativas sin bloqueo y sin valores negativos.
- Descuentos generales y por partida, fijos o porcentuales, con autorización. Acumulación con mayoreo configurable y reparto exacto de centavos.
- Carrito recuperable, varios pendientes con referencia por usuario y confirmación transaccional sin duplicación por reintentos.
- Apertura/cierre de caja, gastos de caja/externos, entradas y retiros, cortes conservados y exportaciones diarias.
- Devoluciones parciales y cancelación a precios históricos; conciliación con caja y tarjeta originales, sin borrar la venta.
- Historial por fechas, usuario y método; 100 operaciones por página con totales completos. CSV de todo el periodo y desglose de productos/gamas/paquetes, sin corte en 1,000 ventas. Cajero consulta sus propias operaciones.
- Cuentas de administrador/cajero, bloqueo/cambio de sesión y autorización en la lógica de operaciones sensibles.
- Alta/edición de productos y fotos, disponibilidad y código de barras real de teclado. Configuración de negocio, impresora y papel de 58/80 mm.
- Respaldos consistentes con la API SQLite: hasta 288 recientes, 30 diarios y 12 semanales; destino externo adicional y restauración validada desde interfaz con copia previa.
- Instancia única por base; instalador y publicación no sobrescriben una entrega en ejecución.

Consulta el [manual de cajero y administrador](MANUAL-OPERACION.md) para procedimientos, atajos y condiciones de restauración/devolución.

## Datos y privacidad

SQLite usa WAL, claves foráneas y `synchronous=FULL`. El dinero se calcula en decimal y se almacena en centavos enteros. Ventas, partidas y movimientos se confirman en una transacción. Las contraseñas utilizan PBKDF2 SHA-256, salt aleatorio y 600,000 iteraciones; no se guardan en borradores.

No subir cuentas, contraseñas, bases, respaldos, ventas, cortes ni gastos de operación o prueba. `dist`, `artifacts`, `.tools` y binarios permanecen ignorados. El catálogo comercial y las imágenes/procedencia sí se versionan. Las pruebas crean datos ficticios temporales aislados.

WhatsApp prepara una nota para envío manual con internet y confirmación manual; no hay envío desatendido ni entrega remota verificada. Resultado de ventas menos gastos no equivale a utilidad contable.

## Compilar y validar

Requiere SDK .NET 10 y NuGet durante el desarrollo. El programa publicado incluye su runtime. En este equipo el SDK por usuario está en `%LOCALAPPDATA%/Microsoft/dotnet/dotnet.exe`.

```powershell
dotnet run --project tests/Pinwi.Tests -c Release
dotnet run --project tests/Pinwi.Tests -c Release -- --render
./scripts/publish.ps1
```

El script genera la entrega y guarda `artifacts/pruebas-operacion.txt`. `--ui` pertenece solo a las pruebas y abre PRUEBA AISLADA con una base temporal, sin usar datos del negocio. GitHub Actions compila, prueba y produce un paquete sin firma; eso no verifica impresoras ni significa aprobación de Store.

## Pendiente

Impresión física de 58/80 mm, lector físico, instalación completa en un segundo Windows limpio sin internet y revisión manual de todas las combinaciones administrativas. Inventario avanzado con costos/recetas/mermas, domicilio y configuración fiscal quedan pendientes. No presentar el alcance completo de AGENTS.md como terminado.
