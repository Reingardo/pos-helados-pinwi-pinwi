# Actualización de interfaz y carrito

- Inicio de venta por ocho gamas: Bolis / Pinwis, Bollos / Heladitos, Paletas de agua, Paletas de leche, Paletas artesanales, Nieve, Sándwich y Esquimales.
- Las gamas usan las fotografías locales. Al entrar se muestran sus sabores con imágenes individuales y un botón para volver.
- Búsqueda por nombre, categoría o código dentro de la gama; desde el inicio busca en todo el catálogo.
- Se incorporaron 75 imágenes del repositorio de referencia, convertidas a PNG y empaquetadas localmente. Arroz con leche usa la foto general por falta de correspondencia individual.
- La procedencia y la marca de imagen generada se conservan en `Assets/image-sources.json`. Algunas imágenes del repositorio son ilustraciones generadas, no fotografías reales.
- En «Tu venta», cada partida permite sumar, restar, quitar y escribir cantidades. Enter o salir del campo confirma la edición. Cero elimina la partida. Se rechazan negativos, decimales y cantidades superiores a 100,000.
- Los cambios recalculan mayoreo, ahorro, descuentos y total.
- Navegación y botones con esquinas redondeadas, mejor jerarquía tipográfica y carrito en panel independiente.

## Verificación

Compilación y pruebas de precios e integración. Pruebas adicionales de edición a 20, regreso a 9 y eliminación con cero. Se inspeccionó visualmente la nueva pantalla de gamas; la comprobación de todos los controles por automatización quedó limitada por detección de entrada simultánea del usuario.

La nueva entrega está en `dist/Actualizacion/PinwiPOS/Pinwi.Pos.exe` y `dist/PinwiPOS-Actualizado.zip`. Se conserva la entrega anterior para evitar reemplazar su ejecutable mientras está abierto. Ambas versiones utilizan la misma base local del negocio; ejecutar una sola a la vez.
