# BancoArboleda — Simulador de Recargas Celulares

BancoArboleda es un prototipo de banca web moderno y premium desarrollado en **ASP.NET Core 8.0 Razor Pages**. Permite realizar recargas de telefonía celular mediante un flujo interactivo multipaso seguro, validación de clave online de prueba, retroalimentación visual de procesamiento y consulta de historial de transacciones en una base de datos local SQLite utilizando SQL puro con Dapper.

---

## 🚀 Cómo Ejecutar el Proyecto

Sigue estos pasos para compilar y ejecutar la aplicación en tu entorno local:

### 1. Requisitos Previos
Asegúrate de tener instalado el **SDK de .NET 8.0**. Puedes verificarlo en tu terminal ejecutando:
```bash
dotnet --version
```

### 2. Compilar la Aplicación
Navega a la carpeta del proyecto `BancoArboleda` desde la terminal y compila el proyecto para descargar las dependencias y verificar que no existan errores:
```bash
cd BancoArboleda
dotnet build
```

### 3. Ejecutar el Servidor de Desarrollo
Inicia el servidor web ejecutando el siguiente comando:
```bash
dotnet run
```

Al iniciar, la terminal te indicará las URLs en las que el servidor está escuchando. Por defecto, puedes ingresar a:
- **[http://localhost:5194](http://localhost:5194)** (HTTP)
- **[https://localhost:7178](https://localhost:7178)** (HTTPS)

> [!NOTE]
> La base de datos SQLite se crea e inicializa (seeder) de manera completamente automática al arrancar la aplicación por primera vez, insertando los operadores móviles de prueba (Claro, Movistar, Entel, Bitel) con sus rangos de recarga.

---

## 🗂️ Estructura y Arquitectura del Proyecto

El proyecto está diseñado bajo un principio de **separación estricta de responsabilidades (Frontend / Backend)** para facilitar el desarrollo, mantenimiento y escalado rápido.

```
BancoArboleda/
│
├── Frontend/                           # Capa de Presentación (UI)
│   ├── Shared/
│   │   └── _Layout.cshtml              # Plantilla y diseño global HTML5
│   ├── Recarga/                        # Vistas y lógica de formularios de recarga
│   │   ├── Paso1.cshtml + .cs          # Entrada de celular y operador móvil
│   │   ├── Paso2.cshtml + .cs          # Selección de monto de recarga
│   │   ├── Paso3.cshtml + .cs          # Ingreso y validación de clave online
│   │   └── Confirmacion.cshtml + .cs   # Constancia de recarga exitosa
│   ├── Index.cshtml + .cs              # Pantalla principal (Home)
│   └── Historial.cshtml + .cs          # Tabla de últimas 10 transacciones
│
├── Backend/                            # Capa de Lógica de Negocio y Datos
│   ├── Models/                         # Entidades de datos del negocio
│   │   ├── Operador.cs                 # Atributos de operador (monto min/max, nombre)
│   │   └── Transaccion.cs              # Atributos de transacción
│   ├── Data/                           # Infraestructura de persistencia
│   │   ├── DatabaseContext.cs          # Conexión pura a SQLite
│   │   └── DbSeeder.cs                 # Inicializador y semillero de datos
│   └── Services/                       # Capa de Servicios (Consultas SQL directas)
│       ├── OperadorService.cs          # SQL para operadores móviles
│       └── RecargaService.cs           # SQL para inserción de recarga e historial
│
├── wwwroot/
│   └── css/
│       └── site.css                    # Diseño visual, variables y animaciones CSS
└── Program.cs                          # Configuración y arranque de la aplicación web
```

### Flujo de Cambios Rápidos
* **¿Deseas cambiar el diseño o UI de una pantalla?** Modifica las vistas en `Frontend/[Pantalla].cshtml`.
* **¿Necesitas cambiar una validación de un formulario?** Edita el PageModel `Frontend/[Pantalla].cshtml.cs`.
* **¿Quieres ajustar una regla de negocio o consulta SQL?** Edita los servicios en `Backend/Services/`.

---

## 🛠️ Tecnologías y Justificación Técnica

* **ASP.NET Core 8.0 (Razor Pages)**: Permite integrar de forma sumamente ágil e integrada las vistas HTML con su lógica de servidor asociada.
* **SQLite (Microsoft.Data.Sqlite)**: Motor de base de datos relacional ultraligero que se guarda en un solo archivo local (`banco.db`), ideal para demostraciones, pruebas y simulaciones rápidas sin requerir servidores externos.
* **Dapper**: Micro-ORM rápido que permite realizar consultas SQL nativas (`SELECT`, `INSERT`) con mapeo automático de objetos C#, garantizando el máximo rendimiento y control sobre las sentencias a la base de datos sin el sobrecosto de Entity Framework.
* **Vanilla CSS (Diseño Premium)**: Estilos modernos construidos de cero, usando HSL, variables CSS globales, esquemas de color corporativos (verde banca), sombras suaves, transiciones fluidas y animaciones interactivas.

---

## ✨ Características Especiales de UX

1. **Flujo de Recarga Guiado**: Un indicador de pasos superior muestra dinámicamente en qué etapa del flujo se encuentra el usuario.
2. **Ingreso Seguro de Clave PIN**: Visualización de 6 casillas individuales interactivas que ocultan los números en forma de puntos (`●`) y habilitan el botón únicamente al completar los 6 dígitos.
3. **Simulador de Procesamiento Seguro**: Al dar clic en *"Confirmar Recarga"*, se bloquea la interfaz y se despliega un overlay a pantalla completa con desenfoque de fondo (`backdrop-filter`) y un spinner animado para simular la validación con la pasarela de pagos.
4. **Animación SVG de Éxito**: En caso de ingresar la clave correcta (`123456`), el spinner transiciona de manera fluida a una palomita animada premium generada mediante SVG vectorial, brindando feedback inmediato al cliente antes de redirigirlo a la pantalla final.
5. **Validación del Servidor**: La clave online de simulación requerida para concretar la transacción es **`123456`**. Cualquier otra clave será validada y denegada en el servidor, mostrando un mensaje de error sin registrar operaciones inválidas en la base de datos.