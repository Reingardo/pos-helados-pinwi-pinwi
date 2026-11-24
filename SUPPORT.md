# Soporte de Helados Pinwi Pinwi POS

Helados Pinwi Pinwi POS es una aplicación de escritorio de EdRos para Windows. Este repositorio es el sitio público del proyecto, con su código fuente, documentación y canal de soporte técnico.

## Contactar con soporte técnico

Para informar de un problema o hacer una consulta sobre la aplicación, utiliza el [canal de soporte del proyecto en GitHub Issues](https://github.com/Reingardo/pos-helados-pinwi-pinwi/issues). Para crear una incidencia necesitas iniciar sesión en GitHub. Puedes consultar la documentación sin tener una cuenta.

Incluye únicamente:

- Versión del POS, por ejemplo 1.1.1.0.
- Versión de Windows y pantalla donde sucede el problema.
- Pasos para reproducirlo y el texto del mensaje de error, sin información personal o del negocio.
- Si se trata de impresión, modelo de impresora y ancho de papel, sin adjuntar un ticket real.

Las incidencias son públicas. No adjuntes bases de datos, respaldos, usuarios, contraseñas, ventas, cortes ni gastos reales o de prueba. Tampoco publiques datos de clientes. El soporte no requiere tu contraseña del POS ni acceso remoto a la computadora.

## Manuales y ayuda

- [Instalación en otra computadora, actualización y traslado de datos](MANUAL-INSTALACION.md).
- [Manual para cajero y administrador](MANUAL-OPERACION.md).
- [Funciones disponibles y límites actuales](README.md).

## Si tus datos no aparecen después de instalar

1. Comprueba que estás usando la misma cuenta de Windows y una versión del POS compatible con la base existente.
2. Conserva la carpeta de datos y tus respaldos. No borres `pinwi.db` ni reviertas una migración para resolver el problema.
3. Revisa el procedimiento de respaldo y restauración del manual. La restauración reemplaza el estado de destino y requiere autorización.
4. Describe el problema en el canal de soporte sin subir el archivo de base ni el respaldo.

La entrega local 1.1.1.0 conserva sus datos en `%LOCALAPPDATA%\PinwiPOS`, separados de la instalación. La edición de Store 1.1.2.0 usa `%USERPROFILE%\PinwiPOS-Store`, fuera de AppData, sin solicitar exclusiones de virtualización. Son bases independientes: en el primer inicio de Store puedes importar un respaldo validado creado desde la versión local. El origen se conserva; no hay traslado automático ni fusión de operaciones. Cierra la versión anterior y deja de vender en ella después del traspaso definitivo.

## Alcance

Las ventas y la consulta local no necesitan internet. El canal de soporte de GitHub y el envío manual opcional por WhatsApp sí lo necesitan. La aplicación no sincroniza bases entre computadoras ni procesa pagos bancarios. La aprobación de Microsoft Store, la firma disponible y la compatibilidad con una impresora física deben comprobarse por separado; una compilación correcta no demuestra esas condiciones.

---

## Developer and app support (English)

EdRos maintains the Helados Pinwi Pinwi POS project in this public repository. App support and problem reports are available through [GitHub Issues](https://github.com/Reingardo/pos-helados-pinwi-pinwi/issues), with installation and operation guides linked above. Do not submit databases, backups, credentials, business transactions or customer information. The application uses local accounts and does not require a GitHub account for its normal offline operation.
