# BancoArboleda — Simulador de Recargas Celulares (Arquitectura Desacoplada)

BancoArboleda es un prototipo de banca web moderno y premium desarrollado bajo una **arquitectura desacoplada en .NET 8.0**:
- **Backend (Web API)**: Construido en **ASP.NET Core Web API** utilizando Minimal APIs. Administra el acceso a una base de datos SQLite y realiza consultas de datos mediante SQL puro usando **Dapper**.
- **Frontend (SPA)**: Desarrollado en **Blazor WebAssembly**. Se ejecuta completamente en el navegador del cliente, consume los endpoints JSON del Backend vía `HttpClient` y gestiona el flujo multipaso de recargas usando un contenedor de estado en memoria del cliente.

---

## 🚀 Cómo Ejecutar el Proyecto

Sigue estos pasos para compilar y ejecutar tanto el Backend como el Frontend en tu entorno local:

### 1. Requisitos Previos
Asegúrate de tener instalado el **SDK de .NET 8.0**. Puedes verificarlo en tu terminal ejecutando:
```bash
dotnet --version
```

### 2. Ejecutar los Servidores de Desarrollo

Para ver el sistema en funcionamiento, debes iniciar **ambos** proyectos en terminales separadas:

#### Paso A: Iniciar el Backend (Web API)
Abre una terminal en la raíz del repositorio, navega a la carpeta `Backend` e inicia el servidor de la API:
```bash
cd Backend
dotnet run
```
El Backend escuchará por defecto en:
- **[http://localhost:5000](http://localhost:5000)** (HTTP)
- **[https://localhost:5001](https://localhost:5001)** (HTTPS - Swagger / OpenAPI disponible en [https://localhost:5001/swagger](https://localhost:5001/swagger))

> [!NOTE]
> La base de datos SQLite `banco.db` se crea e inicializa automáticamente con datos de prueba (operadores y transacciones semilla) en la carpeta `Backend/` la primera vez que se ejecuta el servidor.

#### Paso B: Iniciar el Frontend (Blazor WebAssembly)
Abre otra terminal en la raíz del repositorio, navega a la carpeta `Frontend` e inicia el cliente web:
```bash
cd Frontend
dotnet run
```
El Frontend de Blazor escuchará en:
- **[http://localhost:5194](http://localhost:5194)** (HTTP)
- **[https://localhost:7178](https://localhost:7178)** (HTTPS)

Abre tu navegador e ingresa a **[http://localhost:5194](http://localhost:5194)** para interactuar con la aplicación.

---

## 🗂️ Estructura y Arquitectura del Proyecto

El repositorio está dividido en dos proyectos independientes:

```
/ (Raíz del Repositorio)
├── BancoArboleda.sln                # Solución unificada de .NET
│
├── Backend/                         # Capa de API y Persistencia (BancoArboleda.API)
│   ├── Data/                        # Contexto de Base de Datos SQLite y Seeder
│   │   ├── DatabaseContext.cs
│   │   └── DbSeeder.cs
│   ├── Models/                      # Modelos del negocio (Operador, Transaccion)
│   ├── Services/                    # Servicios de negocio con consultas SQL puras por Dapper
│   │   ├── OperadorService.cs
│   │   └── RecargaService.cs
│   ├── Program.cs                   # Registro de servicios, configuración CORS y Endpoints Minimal API
│   └── appsettings.json             # Cadena de conexión a banco.db
│
└── Frontend/                        # Capa de Presentación (BancoArboleda.Web - Blazor WASM)
    ├── Pages/                       # Vistas y componentes interactivos
    │   ├── Index.razor              # Pantalla principal (Inicio)
    │   ├── Historial.razor          # Tabla de últimas 10 transacciones (llamados API GET)
    │   └── Recarga/                 # Flujo de recarga guiada de 4 pasos
    │       ├── Paso1.razor          # Número de celular y Operador
    │       ├── Paso2.razor          # Selección de monto con límites del operador
    │       ├── Paso3.razor          # Clave online con spinner y feedback SVG de éxito
    │       └── Confirmacion.razor   # Recibo final (limpia el estado del cliente)
    ├── Services/                    # Inyección del cliente HTTP y Estado del flujo
    │   ├── RecargaFlowState.cs      # Contenedor de estado para datos del flujo multipaso
    │   ├── OperadorService.cs       # Cliente para obtener operadores desde la API
    │   └── RecargaService.cs        # Cliente para registrar transacciones y leer historial
    ├── Shared/                      # Contenedores comunes (MainLayout con navbar y footer premium)
    └── wwwroot/                     # Archivos estáticos
        └── css/
            └── site.css             # Estilos corporativos y animaciones CSS (Vanilla CSS)
```

---

## 🛠️ Tecnologías y Justificación Técnica

- **Blazor WebAssembly (SPA)**: Permite ejecutar toda la interfaz de usuario en C# directamente en el navegador del cliente mediante WebAssembly, optimizando el rendimiento y permitiendo una experiencia interactiva sin recargas de página (SPA).
- **ASP.NET Core Web API**: Expone endpoints de forma ligera usando Minimal APIs con serialización JSON nativa y configuración de políticas CORS para consumo cruzado seguro.
- **SQLite & Dapper**: Motor de base de datos local e independiente almacenada en archivo físico (`Backend/banco.db`), consumido con SQL puro a través del micro-ORM Dapper para mantener velocidad óptima y control absoluto de consultas relacionales.
- **Contenedor de Estado en Cliente (`RecargaFlowState`)**: Reemplaza las variables `TempData` y `Session` de servidor por un servicio en memoria del navegador del cliente, logrando la persistencia del flujo multipaso de manera limpia y tipada bajo buenas prácticas de SPA.
- **Vanilla CSS (Diseño Premium)**: Estilos corporativos en tonalidades verdes y grises oscuros, variables de diseño HSL, sombras fluidas, transiciones de botones e indicadores, y pantallas de carga con desenfoque de fondo y animaciones SVG vectoriales.