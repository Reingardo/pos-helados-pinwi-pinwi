# POS de Helados Pinwi Pinwi

## Estado vigente — 16 de noviembre de 2026

**Entrega posterior: Operación Segura 1.1.1.0, SQLite v8.** Esta actualización prevalece sobre las referencias v7 y ProductosCobroRapido inferiores. Leer `ESTADO-OPERACION-V8.md` y `MANUAL-OPERACION.md`.

- Entrega local nueva: `dist/OperacionSegura-v111/PinwiPOS/Pinwi.Pos.exe`; ZIP `dist/PinwiPOS-OperacionSegura-v111.zip`. Cerrar la versión anterior antes de abrirla. La migración a v8 crea respaldo previo; nunca abrir ejecutables antiguos sobre la base v8.
- Se agregaron restauración desde UI validada con copia previa, instancia única, cuentas/roles y bloqueo de sesión, devoluciones/cancelación a precio histórico, entradas/retiros, descuentos fijos/por partida, pendientes por usuario, reportes por periodo paginados y exportaciones completas, configuración y disponibilidad/códigos de barras.
- Respaldos: 288 recientes, 30 diarios y 12 semanales, con destino adicional configurable. Cajero consulta/exporta sus propias operaciones; cambios sensibles requieren autorización en la lógica. Fotos y reglas de venta se conservan.
- Las pruebas nuevas usan bases temporales y verifican restauración real aislada, pagos mixtos/devoluciones por centavos, rollback, caja, roles, reportes superiores a 1,000 operaciones y fallo de copia externa. Revisión interactiva aislada de venta/cobro/ticket/reportes y renders administrativos. No afirmar una revisión manual de todas las combinaciones ni impresión física verificada.
- Continúan pendientes impresión/lector físicos, instalación en segundo equipo limpio desconectado, inventario avanzado, domicilio y configuración fiscal. No cambiar seguridad del equipo.
- Decisión posterior del usuario: actualizar el envío de Store a la versión más reciente. Se sustituyó 1.0.7.0 por **1.1.1.0** y se reenvió a certificación, con publicación MANUAL retenida. Confirmado In certification / preprocesamiento en curso; no hay aprobación ni paquete firmado instalable todavía. No pulsar Publish now ni continuar cambios de publicación sin nueva indicación. Consultar el último apartado de DISTRIBUCION-STORE.md.

### Estado anterior de ProductosCobroRapido (histórico)

Esta sección y las decisiones posteriores del usuario prevalecen sobre las notas históricas y los requisitos iniciales en conflicto.

- Repositorio independiente: https://github.com/Reingardo/pos-helados-pinwi-pinwi, rama main. No trabajar en el repositorio padre ni modificar la web de referencia.
- Última entrega local: dist/ProductosCobroRapido/PinwiPOS/Pinwi.Pos.exe. Los accesos del escritorio apuntan allí. Cerrar el POS anterior antes de abrir otra versión. No sobrescribir una entrega en ejecución.
- Esquema SQLite **v7**, con respaldo previo a migraciones. No abrir ejecutables antiguos sobre la base migrada. Nunca borrar ni revertir bases para corregir errores.
- Catálogo: 76 productos individuales, cinco paquetes y siete entradas de cobro rápido por gama. Las altas posteriores del administrador son adicionales.
- Inicio de venta: botón **Productos**, al lado de **Cobro rápido**. Las tarjetas rápidas llevan imagen a la izquierda, nombre, precio por pieza y mayoreo. Esquimales se venden desde Productos y no aparecen en cobro rápido.
- **Mayoreo actualizado:** desde 10 piezas de la misma gama, sumando sabores diferentes, tanto en venta normal como rápida. Bolis de agua/leche comparten gama; bollos/heladitos de agua/leche también. Esquimales conservan conteo por presentación. Desde $400 valorados a menudeo se activa mayoreo global, sin perderlo porque el total ajustado baje de $400. No recalcular ventas históricas.
- Las ventas rápidas registran la gama, sin inventar sabores ni descontar existencias de un sabor concreto. Comparten carrito, cobro, borrador y reportes con venta normal. Sus precios son independientes y editables en Productos y precios; si los sabores tenían precios distintos al migrar, el administrador debe confirmar los precios de la gama rápida antes de usarla.
- Paquetes: Pinwi $500; Bollos $480; Fiesta $695; Esquimales $445; Surtido $1,090. Una unidad del carrito representa un paquete completo. No hay selección ni descuento de componentes por sabor.
- Productos y precios permite **+ Añadir producto** y **Editar producto y foto**: nombre, categoría existente/nueva, menudeo, mayoreo, foto opcional, motivo y autorización. Nuevos productos aparecen en Venta con existencia inicial cero, informativa. No cambiar existencias al editar nombre, precio o foto.
- Fotos personalizadas normalizadas a PNG y guardadas como BLOB en SQLite. La carpeta de imágenes de datos es caché reconstruible; el respaldo de base incluye las fotos. Se conserva la imagen local de categoría como alternativa.
- Pruebas completas de lógica aprobadas con datos temporales aislados: precios y agrupación por gama, umbrales, pagos/cambio, idempotencia, rollback, caja/cortes, gastos, borradores, migraciones, altas/ediciones, autorización, fotos y recuperación desde respaldo independiente. La vista final de cobro rápido con imágenes fue renderizada y revisada. La última compilación y publicación terminaron correctamente. No afirmar que imprimir en una impresora física esté verificado.
- Smart App Control bloqueó antes las compilaciones sin firma. El usuario autorizó expresamente desactivarlo y realizó personalmente el cambio después de que la herramienta no pudiera abrir Seguridad de Windows. Tras su aviso, pasaron las pruebas y abrió la actualización de CobroRapido. No se modificó la seguridad con scripts. Ese inicio no equivale a revisar todo el flujo interactivo de la última entrega ProductosCobroRapido. No sugerir desactivar otras protecciones.
- **Prioridad actual: entrega local.** El usuario indicó que no desea publicar por ahora porque el POS no está terminado. Antes había autorizado el envío público gratuito a Store: v1.0.7.0 quedó en certificación y con publicación manual retenida. No pulsar Publish now ni continuar publicación sin nueva indicación. El paquete de Store es anterior al último ajuste de imágenes y texto Productos. Consultar DISTRIBUCION-STORE.md; no confundir Validated/In certification con aprobación o disponibilidad.
- No subir cuentas, contraseñas, bases, respaldos, ventas, cortes ni gastos operativos o de prueba. dist, .tools, artifacts y binarios siguen ignorados. El catálogo comercial, imágenes y procedencia sí se versionan. Una instalación limpia crea al administrador sin credenciales predeterminadas.
- WhatsApp continúa siendo preparación de nota y envío manual con internet. No afirmar envío desatendido ni entrega confirmada.
- Pendiente: impresión física 58/80 mm, revisión interactiva completa de la última entrega, restauración desde UI, roles/cajeros, otros descuentos, devoluciones/cancelaciones, inventario avanzado y domicilio.

## Notas históricas de sesiones anteriores

Las rutas, versiones y reglas antiguas de las siguientes notas conservan el historial; aplicar primero el estado vigente anterior.


## Leer primero al retomar otra sesión

**Actualización posterior importante:** el enlace anterior a `dist/Actualizacion` dejó de abrir porque la base local ya requiere la versión nueva. El registro de .NET confirmó «Esta base requiere una versión más reciente del POS». Se inició correctamente el ejecutable existente de `dist/Gastos/PinwiPOS/Pinwi.Pos.exe`, sin cambiar seguridad ni borrar/revertir la base. Los accesos directos del escritorio ahora apuntan a `dist/Gastos`. Esto verifica el inicio del proceso, no todo el flujo de gastos o impresión. No volver a proporcionar el enlace al ejecutable anterior para esta base. Las notas históricas de bloqueo de firma que siguen abajo describen los intentos previos.

**Contexto guardado el 16 de noviembre de 2026. El usuario continuará en otra sesión.** Este es el proyecto de escritorio del POS; su repositorio es independiente de la web de la heladería.

- **GitHub público:** https://github.com/Reingardo/pos-helados-pinwi-pinwi
- Rama de continuidad: `main`; remoto local: `origin`. El proyecto ahora tiene su propio `.git`; no trabajar ni commitear sobre el repositorio padre de otras carpetas.
- Leer también `CONTINUAR-MANANA.md` para el mapa del código, instrucciones de reproducción y secuencia de decisiones; `ESTADO-PRUEBAS-GASTOS.md` detalla las verificaciones y el bloqueo de Windows.
- El usuario pidió conservar los datos del negocio: nombre, teléfono, Facebook, catálogo, precios, logo e imágenes. **No subir cuentas, contraseñas, bases, respaldos ni ventas/cortes/gastos de prueba u operación.** Una instalación nueva crea al administrador sin credenciales predeterminadas y comienza sin ventas. Las pruebas crean sus propios datos ficticios en carpetas temporales; no son datos precargados del negocio.
- Los binarios, dependencias descargadas, capturas y datos locales no están versionados. El clon contiene todo el código y los recursos necesarios para reconstruir el POS. No borrar bases locales del usuario como parte de limpiar Git.

### Estado real del código al cerrar esta sesión

El código actual incluye catálogo de 76 productos, imágenes locales e individuales, inicio por tipos de producto, carrito con imagen a la izquierda y cantidades editables, mayoreo automático, cobro con sugerencias y cambio, tickets con logo y contactos, ventas sin bloqueo de existencias, nombre del usuario en ventas, recuperación del carrito, respaldos automáticos, cortes históricos y registro de gastos.

**Los últimos cambios son:** navegación «Inicio» / «Volver a inicio»; botón «Registrar gastos» junto a abrir/cerrar caja; formulario de gasto con concepto, importe, proveedor/persona y origen; reporte diario con ventas menos gastos y detalle de conceptos; CSV separado de gastos. Un gasto de caja reduce el efectivo esperado; uno externo no lo reduce. Ambos se restan del resultado de ventas menos gastos. La base está en **versión 4**, con respaldo antes de migrar.

La compilación y las pruebas de lógica llegaron a pasar con datos aislados, incluido **$5,000 − $1,300 = $3,700**, gastos externos, idempotencia, CSV y conservación de cortes. **La apertura completa y la revisión visual de la última actualización siguen pendientes**: Smart App Control bloqueó los binarios nuevos por firma (`0x800711C7`). Una repetición normal de pruebas pudo ejecutarse, pero eso no significó autorización permanente; después se volvieron a bloquear la herramienta de vistas y el ejecutable con gastos. No se cambiaron las protecciones del equipo. No hay certificado disponible y el usuario confirmó que no tiene certificado ni cuenta de firma.

La versión local anterior permitida por Windows está en `dist/Actualizacion/PinwiPOS`, sin registrar gastos. La nueva está aparte en `dist/Gastos/PinwiPOS` y en un ZIP identificado como pendiente de firma. Estas carpetas no están en GitHub. El acceso directo del escritorio «Helados Pinwi Pinwi - version verificada» apunta a la anterior. No abrir ambas a la vez ni volver a una versión antigua tras migrar la base.

WhatsApp **no tiene envío desatendido configurado**. Se prepara una nota para el número del negocio; requiere internet y pulsar Enviar. El estado de enviado es confirmación manual, no comprobación remota. Las notas extensas se copian completas para pegarlas. No afirmar que el reporte diario ya se envía automáticamente.

Se agregó `.github/workflows/windows.yml` para compilar, ejecutar pruebas de lógica y generar un paquete Windows x64 sin firma. **Comprobar el resultado más reciente de GitHub Actions al retomar**; no confundir una ejecución iniciada con una aprobada. El paquete de CI tampoco resuelve la firma ni valida la impresora física.

### Próximos pasos

1. Revisar el estado de GitHub Actions y reproducir las pruebas en Windows con SDK .NET 10.
2. Resolver una vía de firma/distribución admitida por Windows y comprobar la apertura de la actualización sin desactivar seguridad.
3. Verificar visualmente gastos, cortes, navegación y carrito, y probar la impresión real de 58/80 mm.
4. Después seguir con roles/cajeros, otros descuentos, devoluciones/cancelaciones, alta de productos, domicilio, reportes avanzados y restauración desde la interfaz. El alcance completo de este archivo todavía no está terminado.

Las decisiones posteriores del usuario y la sección «Ajustes solicitados posteriormente por el usuario» prevalecen sobre los requisitos iniciales que entren en conflicto, especialmente en existencias informativas y envío por WhatsApp.

## Objetivo y alcance

Construir una aplicación de escritorio moderna, rápida y estable para una heladería y paletería en Windows. Debe funcionar completamente sin internet, con persistencia real en una base de datos local. Entregar una aplicación funcional, no únicamente pantallas o una maqueta.

El alcance inicial es una sucursal con una computadora principal. No compartir el archivo SQLite mediante carpetas de red. No incluir sincronización entre sucursales ni servicios en la nube dentro de este alcance.

## Tecnología y arquitectura

- C# con .NET 10 LTS, WPF y XAML.
- Patrón MVVM; separar interfaz, lógica de negocio y acceso a datos sin introducir capas innecesarias.
- SQLite local mediante Microsoft.Data.Sqlite, consultas parametrizadas y migraciones versionadas.
- No utilizar Electron, interfaces basadas en navegador, microservicios ni servidores externos.
- Mantener las reglas de precios, cobro e inventario independientes de la interfaz y verificables mediante pruebas.
- Publicar para Windows x64 en modalidad self-contained, incluyendo runtime y dependencias nativas de SQLite.
- Utilizar versiones compatibles y con soporte de las dependencias. Evitar paquetes innecesarios.

## Operación completamente local

- No depender de cuentas externas, autenticación en línea, telemetría, licencias en línea, CDN ni APIs remotas.
- Empaquetar tipografías, iconos, imágenes y demás recursos.
- Guardar la base de datos, configuración, registros e imágenes personalizadas en una carpeta de datos con permisos de escritura, separada de la instalación.
- No codificar rutas absolutas de la computadora de desarrollo.
- El instalador debe funcionar en el equipo del negocio sin descargar componentes.
- Distinguir las herramientas necesarias para desarrollar de las dependencias necesarias para operar.
- Las actualizaciones deben poder distribuirse mediante un paquete local y preservar los datos.

## Identidad del negocio

- Nombre comercial: **Helados Pinwi Pinwi**.
- Teléfono / WhatsApp: **833 393 2616**.
- Moneda: MXN.
- Idioma y formatos: español de México.
- Dirección: pendiente de captura; no inventarla.
- Permitir editar estos datos desde administración.

## Interfaz

- Diseño limpio y profesional, con colores suaves inspirados en una heladería y buen contraste.
- Pantalla de venta como vista principal: categorías y productos al centro, carrito siempre visible a la derecha.
- Botones grandes, compatibles con pantalla táctil, mouse y teclado.
- Búsqueda por nombre y código de barras; atajos para operaciones frecuentes.
- Acceso claro a ventas, productos, inventario, caja, reportes y configuración.
- Adaptar la interfaz a resoluciones desde 1366 × 768.
- Evitar animaciones que retrasen la operación y mensajes con detalles técnicos innecesarios.
- Mostrar errores accionables sin bloquear ni perder el trabajo del cajero.

## Catálogo precargado

Crear cada combinación de categoría, sabor y presentación como producto vendible con código interno único e inventario independiente. Todos los precios siguientes son por unidad y en MXN.

| Categoría | Menudeo | Mayoreo | Sabores |
| --- | ---: | ---: | --- |
| Pinwis de agua | 6.00 | 5.00 | Limón; Piña; Uva; Chamoy; Tamarindo; Mango con chile; Fresa agua; Piña con chile |
| Pinwis de leche | 6.00 | 5.00 | Pistache; Coco; Chicle; Fresa leche; Chocolate; Rompope; Plátano; Nuez; Mango |
| Heladitos de leche | 18.00 | 16.00 | Pingüino; Gansito; Fresas con crema; Oreo; Chocorrol; Bubulubu; Mazapán; Coco; Nutella; Pay de limón; Duvalín; Arroz con leche |
| Heladitos de agua | 18.00 | 16.00 | Mango chamoy; Picafresa; Tamarindo con chile |
| Paletas de agua | 18.00 | 16.00 | Chamoy; Melón; Fresa; Limón; Tamarindo; Piña; Uva; Sandía; Mango |
| Paletas de leche | 22.00 | 20.00 | Fresa; Nuez; Galleta Oreo; Mango; Chicle; Carlos V; Coco; Mazapán; Rompope |
| Paletas artesanales | 24.00 | 22.00 | Pingüinos; Gansito; Fresa kiwi; Chocorrol; Guayaba kiwi; Piña Skwinkles; Pay de limón; Cóctel de frutas; Fresas con crema; Mango manzana chile; Mango chamoy Panditas |
| Nieve | 23.00 | 21.00 | Fresa; Nuez; Vainilla; Oreo; Mango chamoy; Pica fresa; Limón |
| Sándwich | 18.00 | 16.00 | Fresa; Nuez; Oreo; Vainilla |

Esquimales con precios por presentación:

| Producto | Menudeo | Mayoreo |
| --- | ---: | ---: |
| Esquimal grande | 22.00 | 20.00 |
| Esquimal grande con coco | 24.00 | 22.00 |
| Esquimal grande con chispas | 24.00 | 22.00 |
| Esquimal grande con nuez | 27.00 | 25.00 |

- El catálogo inicial contiene **76 productos vendibles** en 10 categorías, contando los cuatro esquimales como productos independientes.
- Para nieve, usar una presentación editable llamada «Presentación del menú». No inventar volumen ni peso.
- No precargar productos genéricos de ejemplo en lugar del menú real.
- No inventar códigos de barras comerciales, costos, existencias, recetas ni medidas.
- Iniciar existencias en cero y ofrecer captura de inventario inicial antes de vender.
- Mantener costos pendientes como valores desconocidos, no como cero para reportar utilidades.
- Mostrar nombres completos en tickets y reportes, por ejemplo «Paleta de leche — Nuez».
- Permitir crear, editar, activar y desactivar productos, categorías, sabores y presentaciones.
- Permitir futuras presentaciones por bola, vaso, cono, litro y medio litro, bebidas, toppings y complementos, sin agregarlos como productos reales precargados.
- Admitir modificadores, sabores incluidos y cargos adicionales configurables.
- Admitir opcionalmente venta por peso con captura manual; no asumir integración con báscula.

## Precios de menudeo y mayoreo

Cada producto debe tener dos precios editables: menudeo y mayoreo. Validar que sean no negativos y que mayoreo no supere menudeo.

### Activación automática

1. **Por cantidad:** desde 10 piezas del mismo producto, cobrar todas sus unidades a mayoreo. Sumar renglones con el mismo código de producto y presentación. No sumar sabores o productos diferentes para este umbral. Aplicar esta regla únicamente a productos vendidos por pieza.
2. **Por consumo:** desde $400.00 de productos valorados a menudeo, antes de descuentos y sin cargos adicionales, cobrar todos los productos de la venta a mayoreo.
3. Calcular el umbral por consumo a partir de los precios de menudeo, independientemente de los precios ya mostrados en el carrito. Alcanzado el umbral, el total a mayoreo puede quedar por debajo de $400 sin perder la elegibilidad.
4. Recalcular al agregar, quitar o modificar cantidades. Si el carrito deja de cumplir $400 a menudeo, mantener mayoreo únicamente en productos con al menos 10 piezas. Si tampoco cumplen cantidad, regresar a menudeo.
5. Permitir configurar los umbrales en administración; valores iniciales: 10 piezas y $400.00.

Mostrar por partida «Menudeo» o «Mayoreo». Mostrar ahorro por mayoreo y descuentos adicionales por separado. Conservar en la venta los precios y reglas efectivamente aplicados aunque después cambie la configuración.

### Descuentos

- Admitir descuentos por producto y sobre el total, tanto por porcentaje como por importe fijo en pesos.
- Requerir autorización del administrador y registrar usuario, motivo, fecha e importe.
- Determinar primero menudeo o mayoreo y después aplicar descuentos.
- Por defecto, no acumular descuentos con mayoreo. Permitir habilitarlo desde administración.
- En carritos mixtos, excluir las partidas a mayoreo de descuentos generales cuando la acumulación esté deshabilitada. Distribuir el descuento entre las partidas elegibles.
- Impedir porcentajes fuera de 0–100 e importes que produzcan totales negativos.
- Los descuentos no modifican la base que determina el umbral de $400.
- Definir un orden consistente para descuentos por partida y globales y conservar su distribución para reportes y devoluciones.

## Ventas y cobro

- Agregar, modificar y quitar productos, cantidades, modificadores y observaciones.
- Mostrar subtotal, ahorro por mayoreo, descuentos, impuestos configurables y total sin duplicar conceptos.
- Los precios del menú son los importes finales anunciados; no agregar impuestos por defecto. Permitir configurar su tratamiento sin inventar tasas fiscales.
- Admitir efectivo, registro de pago con terminal externa y pagos mixtos.
- Calcular cambio y validar importes recibidos. El cambio corresponde al componente de efectivo.
- Registrar tarjeta no implica procesarla: la autorización ocurre en una terminal externa. No almacenar datos de tarjetas.
- Guardar y recuperar ventas pendientes sin descontar inventario hasta confirmarlas.
- Consultar ventas, reimprimir y tramitar cancelaciones y devoluciones con motivo y autorización.
- Conservar ventas confirmadas e historial; no borrarlas ni reescribirlas para ocultar operaciones.
- Devolver el importe efectivamente pagado por las unidades, respetando descuentos y devoluciones anteriores; no recalcular una venta histórica con precios actuales.

## Inventario

- Existencias por producto y presentación, con unidades explícitas: piezas, litros, kilogramos y gramos.
- Entradas, ajustes, mermas, mínimos y movimientos con usuario, fecha y motivo.
- Descuento automático al confirmar ventas.
- Recetas opcionales para descontar insumos, con conversiones explícitas y sin descontar dos veces la misma existencia.
- Reintegrar inventario en devoluciones únicamente cuando corresponda según el estado del producto.
- Configurar si se permite vender sin existencias; valor inicial: bloquear.

## Caja

- Apertura de turno con fondo inicial.
- Entradas y salidas de efectivo con motivo.
- Totales separados por método de pago.
- Corte con efectivo esperado, contado y diferencia.
- Historial de turnos y movimientos.
- Reflejar correctamente descuentos, devoluciones, cancelaciones y pagos mixtos.

## Usuarios y seguridad

- Roles iniciales: administrador y cajero.
- Crear al primer administrador en la configuración inicial, sin credenciales predeterminadas.
- Almacenar contraseñas mediante hash seguro con salt y factor de trabajo adecuado; no texto plano ni cifrado reversible.
- Comprobar permisos en la lógica de aplicación, además de la interfaz.
- Permitir bloqueo de sesión y cambio de usuario.
- Registrar descuentos, cancelaciones, devoluciones, ajustes y cortes en una bitácora.

## Tickets y periféricos

- Imprimir mediante impresoras instaladas en Windows, con formatos de 58 y 80 mm.
- Incluir negocio, dirección si está configurada, teléfono, folio, fecha, cajero, partidas, precios aplicados, descuentos, total, pagos y cambio.
- Ofrecer vista previa y reimpresión.
- Admitir lectores de código de barras que actúen como teclado.
- Un error de impresión no debe revertir ni duplicar una venta confirmada.
- El ticket es comprobante de venta. No incluir facturación fiscal en línea en el alcance offline.

## Pedidos a domicilio

- El menú anuncia envío gratis con mínimo de $400 de consumo.
- Incluir modalidad a domicilio con nombre, teléfono, dirección, referencias y estado del pedido.
- Valor inicial para envío gratis: total final de productos después de mayoreo y descuentos, excluyendo envío.
- Mantener este criterio visible y editable: el cartel no especifica cómo se combina con mayoreo. La elegibilidad de envío es independiente del umbral de mayoreo.
- Si no alcanza el mínimo, indicar «No cumple mínimo de envío gratis». No inventar tarifa; permitir configurar tarifa y zona después.
- No depender de mapas, WhatsApp ni otros servicios externos para registrar y gestionar pedidos.

## Reportes

- Ventas por día, rango de fechas, cajero y método de pago.
- Productos y sabores más vendidos.
- Ahorro por mayoreo, descuentos, cancelaciones y devoluciones.
- Existencias, movimientos y mermas.
- Utilidad estimada solo con costos conocidos; señalar cobertura y datos faltantes.
- Exportación CSV compatible con Excel.
- Filtros y totales consistentes con ventas y cortes.

## Imágenes locales y referencias del repositorio

Usar primero las imágenes de `imagenes producto/`:

- `sandwich healado.jpg`
- `paletas de leche.jpg`
- `paletas de agua.jpg`
- `paletas artesanales.jpg`
- `nieves.jpg`
- `esquimales.jpg`
- `bollos.jpg`
- `bolis-pinwis.jpg`

Inspeccionar visualmente cada imagen antes de asignarla. Verificar la relación de bollos con heladitos y bolis con pinwis. Si una imagen muestra varios productos, utilizarla como imagen de categoría, sin atribuirla como fotografía exacta de un sabor.

Fuente complementaria autorizada:

https://github.com/Reingardo/helados-pinwi-pinwi

Rutas relevantes en la rama `main`:

- `public/assets/`: imágenes y recursos de marca.
- `public/assets/logo-pinwi-transparente.png`: logotipo transparente.
- `public/assets/logo-pinwi.webp`: logotipo alternativo.
- `lib/catalog.mjs`: catálogo y asociaciones iniciales de imágenes.
- `lib/photo-map.mjs`: sustituciones por producto y campo `generated`.

Reglas de integración:

- Consultar catálogo y mapa conjuntamente, verificando la existencia del archivo y su extensión final; numerosas referencias terminan realmente en `.webp`.
- El repositorio llama «Bolis» a pinwis y «Bollos» a heladitos. Relacionarlos sin duplicar productos y conservar las denominaciones del menú de este proyecto.
- Priorizar imágenes locales adecuadas y completar las faltantes con recursos del repositorio.
- Distinguir imágenes generadas de fotografías mediante los metadatos disponibles. No describir imágenes generadas como fotos reales.
- No eliminar sabores del menú porque falten en el repositorio ni incorporar automáticamente sus paquetes y promociones.
- No copiar su lógica de cobro: aplicar las reglas de este documento.
- Descargar los recursos durante el desarrollo y empaquetarlos localmente. Convertir WebP a PNG o JPEG cuando sea necesario para WPF.
- Conservar originales, proporciones y un registro del origen y asociación de cada imagen; generar miniaturas optimizadas.
- Ante ausencia de imagen específica, utilizar categoría o icono local. No generar nuevas fotografías ficticias de sabores.
- Permitir cambiar imágenes desde administración, guardarlas en datos e incluirlas en respaldos.
- No consultar GitHub, el sitio web ni un CDN durante la operación.
- El sitio `https://sitio-pinwi.vercel.app/#menu` es una referencia secundaria; no es necesario acceder a él si el repositorio aporta los recursos.

## Integridad, dinero y rendimiento

- Guardar venta, partidas, pagos y movimientos de inventario en una sola transacción.
- Evitar duplicados por doble clic, reintentos y reimpresión.
- Usar `decimal` en cálculos monetarios y centavos enteros para almacenar importes finales; nunca `float` o `double` para dinero.
- Definir redondeo a centavos, distribución de descuentos y conciliación de totales.
- Conservar precio, descuento, impuesto y regla aplicados por partida.
- Activar claves foráneas, configurar SQLite para uso local y manejar bloqueos sin congelar la interfaz.
- Crear índices para búsquedas y reportes; paginar o virtualizar listados grandes.
- No bloquear la interfaz durante consultas largas, reportes o respaldos.
- Registrar errores técnicos localmente sin exponer contraseñas ni datos de pago.
- Asegurar consistencia ante cierres inesperados; no prometer protección contra fallos físicos sin respaldo.

## Respaldos y migraciones

- Respaldos manuales y automáticos a carpeta local o USB elegida por el usuario.
- Utilizar la API de respaldo de SQLite o un procedimiento consistente; no copiar de forma insegura la base abierta.
- Incluir imágenes personalizadas y configuración necesarias para restaurar el negocio.
- Mantener copias fechadas con política de retención.
- Validar antes de restaurar, pedir confirmación y respaldar el estado previo.
- Realizar respaldo antes de migraciones; no borrar datos para resolver errores de esquema.
- Precargar el catálogo una sola vez, transaccionalmente y sin duplicados.
- Las actualizaciones no deben sobrescribir nombres, precios, existencias ni ajustes del administrador.

## Implementación y entregables

Inspeccionar el proyecto antes de modificarlo. Explicar brevemente las decisiones relevantes y avanzar por etapas funcionales. Tomar decisiones razonables y preguntar solo cuando falte un dato indispensable. No publicar, desplegar ni modificar el repositorio remoto de referencia como parte de consultar sus recursos.

Primera etapa: configuración inicial → apertura de caja → catálogo e inventario inicial → venta con mayoreo y descuentos → cobro → ticket → cierre de caja. Después completar inventario avanzado, domicilio y reportes. Las etapas no reducen el alcance final.

Entregar:

1. Código fuente completo y organizado.
2. Base de datos con migraciones y catálogo real precargado; sin ventas de prueba en operación.
3. Instrucciones para compilar, publicar e instalar.
4. Paquete de instalación Windows x64 con todas las dependencias.
5. Manual breve para cajero y administrador.
6. Pruebas de reglas críticas y evidencia de compilación y validación.

No presentar botones decorativos ni funciones incompletas como terminadas. Informar explícitamente las limitaciones y verificaciones pendientes.

## Validación obligatoria

- Catálogo inicial: 76 productos, precios correctos y ausencia de duplicados después de reiniciar o migrar.
- 9 pinwis de agua de limón: $54; 10 del mismo producto: $50.
- 5 pinwis de limón más 5 de piña: $60; no alcanzar mayoreo por cantidad.
- Consolidar correctamente un mismo producto dividido en varios renglones.
- Verificar umbrales de $399.99 y $400 con casos de prueba; $400 exactos activan mayoreo global.
- Al bajar de $400 a menudeo, conservar únicamente las partidas que cumplan 10 piezas.
- No perder mayoreo solo porque su aplicación reduzca el total final por debajo de $400.
- Validar descuentos fijos, porcentuales, por partida y globales, con acumulación de mayoreo habilitada y deshabilitada.
- Verificar cambio, pagos mixtos, permisos, devoluciones parciales, inventario y conciliación del corte.
- Verificar rollback ante fallos de transacción y que doble clic o fallo de impresión no dupliquen ventas.
- Verificar persistencia de ventas, precios e imágenes tras cerrar y abrir.
- Probar respaldo y restauración con base e imágenes personalizadas.
- Ejecutar y revisar el flujo de escritorio; no considerar una compilación exitosa como prueba suficiente de la interfaz o la impresión.
- Prueba final con internet desconectado: instalar, crear administrador, abrir caja, registrar inventario, vender, cobrar, cerrar y abrir, consultar venta, reimprimir, cortar caja y generar respaldo. Todas las imágenes deben seguir visibles.
- Si falta una impresora física o un entorno Windows de prueba, indicar exactamente qué parte no pudo verificarse.


## Ajustes solicitados posteriormente por el usuario

Estas decisiones posteriores prevalecen sobre los requisitos anteriores que entren en conflicto:

- El inventario es informativo y no bloquea cobros. Permitir ventas sin existencias; mostrar aviso rojo en producto, carrito y cobro. Conservar registros históricos y los precios. El saldo de existencias se limita a cero, sin inventar entradas. La edición habitual de precios no debe exigir capturar existencias.
- Guardar el nombre real del usuario de sesión en cada venta, ticket, historial y CSV. No atribuir retroactivamente ventas antiguas a un usuario; mostrar «No registrado» si faltaba ese dato.
- Usar una única ventana de cobro moderna con efectivo, tarjeta o mixto, importes sugeridos, captura manual y cambio calculado antes de confirmar. Ejemplo: $470 pagados con $500 → $30 de cambio.
- El ticket del cliente incluye el logo proporcionado en Assets/Brand/logo-pinwi.png, «¡Muchas gracias por su compra!», «Cualquier sugerencia, estamos a la orden.», Facebook facebook.com/heladospinwi y WhatsApp 833 393 2616 con sus iconos al lado. Incluir estos recursos localmente tanto en vista previa como en impresión.

- Mostrar una imagen del producto a la izquierda de cada partida del carrito.
- Conservar los cortes para consulta y generar automáticamente nota y CSV de las ventas diarias al cerrar caja. Diferenciar venta del turno y acumulado del día al momento del corte.
- Preparar el resumen de WhatsApp para el número de la empresa al cerrar caja y con un botón. No afirmar envío ni entrega mientras no exista integración remota; mantener pendiente y permitir confirmación manual. El POS conserva sus funciones locales sin internet.
- Guardar el carrito con cada modificación válida; eliminar su borrador en la misma transacción que confirma la venta para evitar duplicados al recuperar. Nunca persistir contraseñas de autorización en borradores.
- Respaldar consistentemente después de modificaciones y cada cinco minutos sin actividad, agrupando ráfagas para no bloquear la interfaz. Mantener hasta 288 copias y mostrar errores de guardado o respaldo.

- En navegación de ventas usar «Inicio» y «Volver a inicio» en lugar de «Gamas».
- Agregar «Registrar gastos» junto a los controles de caja. Registrar importe, concepto, proveedor/persona, usuario y origen del pago durante un turno abierto. Descontar los pagos de caja del efectivo esperado; los pagos externos solo del resultado de ventas menos gastos.
- El reporte diario y la nota de WhatsApp deben incluir ventas, total de gastos, resultado y detalle por concepto. Guardar CSV de gastos junto al CSV de ventas, sin modificar cortes anteriores. «Resultado» no equivale a utilidad contable.

- Decisión más reciente: mayoreo por cantidad suma todos los sabores de una misma gama en venta normal y rápida, desde 10 piezas; umbral global desde $400 a menudeo. Esta decisión reemplaza las reglas históricas que exigían el mismo sabor. Esquimales se mantienen por presentación.
