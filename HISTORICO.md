# Historial del Proyecto - JSONPlaceholder API Consumer

## Informacion del Proyecto
- **Fecha de inicio**: 2026-01-20
- **Tecnologia**: .NET 8 Web API
- **Objetivo**: Crear una API web que exponga endpoints RESTful propios y consuma datos de JSONPlaceholder

---

## Transcripcion de la Conversacion

### Mensaje 1 - Usuario
**Pregunta inicial**: "Que ves aqui?"

**Respuesta IA**: Se analizo el repositorio encontrando:
- Repositorio nuevo/vacio con estructura inicial
- Archivos: README.md, .gitignore (configurado para .NET), LICENSE
- Proposito: Crear una API que consuma JSONPlaceholder
- Sin codigo fuente aun

---

### Mensaje 2 - Usuario
**Solicitud**: Crear estructura del proyecto en capas (Modelos, Controllers, Services, Repositorios) y preparar archivo .md con historico.

**Contexto del ejercicio proporcionado**:
> "Genera una nueva API web en .NET que exponga endpoints RESTFul propios y consuma datos de una API externa (https://jsonplaceholder.typicode.com/)"

**Requisitos adicionales**:
- Commits por cada decision de cambios con justificacion
- Hacer preguntas adecuadas antes de comenzar

---

### Preguntas de Clarificacion - IA
La IA realizo las siguientes preguntas para tomar decisiones de arquitectura:

1. **Version de .NET**: .NET 8 (LTS mas reciente)
2. **Recursos a consumir**: Todos (Posts, Comments, Users, Albums, Photos, Todos)
3. **Persistencia de datos**: Solo consumo API externa (sin base de datos local)
4. **Documentacion**: Swagger/OpenAPI

---

### Mensaje 3 - Usuario
**Feedback**: "He realizado algunas pruebas de los endpoints y parece que funciona bien."

**Solicitud**: Implementar:
1. Manejo de errores global (middleware)
2. Logging

**Requisito adicional**: Anadir comentarios en el codigo explicando el porque de cada nueva funcionalidad.

---

### Mensaje 4 - Usuario
**Pregunta**: "Crees que seria conveniente implementar JWT y/o OAuth 2.0? Aportaria alguna funcionalidad interesante para este proyecto?"

**Respuesta IA**: Se explico que para ESTE proyecto especifico no era estrictamente necesario porque:
- Los datos de JSONPlaceholder son ficticios
- No hay usuarios reales ni operaciones sensibles
- Seria over-engineering sin beneficio real

Sin embargo, se menciono que tiene valor educativo si el objetivo es aprender/demostrar conocimientos.

**Decision Usuario**: Inicialmente "No implementar", luego cambio a "Implementar JWT".

---

### Mensaje 5 - Usuario
**Solicitud**: "Me lo he pensado mejor, implementa JWT y OAuth 2.0"

**Preguntas de Clarificacion - IA**:
1. **Tipo de autenticacion**: JWT simple (login con usuario/contrasena)
2. **Endpoints a proteger**: Solo escritura (POST, PUT, DELETE)
3. **Sistema de roles**: Roles avanzados con permisos granulares

---

### Respuesta IA - Implementacion de JWT

Se implemento un sistema completo de autenticacion y autorizacion:

#### 1. Modelos de Autenticacion
**Carpeta**: `Models/Auth/`
- `AppUser.cs`: Usuario del sistema con roles y permisos
- `Permission.cs`: Permisos granulares (Resource.Operation)
- `Role.cs`: Roles predefinidos (Admin, Editor, Viewer)
- `AuthDtos.cs`: DTOs para login, registro y respuestas
- `JwtSettings.cs`: Configuracion de JWT

#### 2. Sistema de Permisos Granulares
**Carpeta**: `Auth/`
- `PermissionRequirement.cs`: Requisito de autorizacion
- `PermissionHandler.cs`: Handler que evalua permisos
- `RequirePermissionAttribute.cs`: Atributo para proteger endpoints

#### 3. Servicio de Autenticacion
**Archivo**: `Services/AuthService.cs`
- Login con validacion de credenciales
- Registro de nuevos usuarios
- Generacion de tokens JWT con claims de permisos
- Hash de contrasenas con PBKDF2
- Usuarios de prueba: admin, editor, viewer

#### 4. Controlador de Autenticacion
**Archivo**: `Controllers/AuthController.cs`
- POST /api/auth/login
- POST /api/auth/register
- GET /api/auth/me
- GET /api/auth/roles
- GET /api/auth/validate

#### 5. Proteccion de Endpoints
Todos los controladores actualizados con:
- `[AllowAnonymous]` en GET (lectura publica)
- `[RequirePermission(Resource, Operation)]` en POST, PUT, DELETE

---

## Decisiones de Arquitectura

### Decision 1: Estructura en Capas
**Fecha**: 2026-01-20
**Descripcion**: Se adopta arquitectura en capas para separacion de responsabilidades

**Justificacion**:
- Facilita el mantenimiento y testing
- Sigue principios SOLID
- Permite cambiar la fuente de datos sin afectar otras capas

### Decision 2: Uso de HttpClient con IHttpClientFactory
**Fecha**: 2026-01-20
**Descripcion**: Se utiliza IHttpClientFactory para gestionar las instancias de HttpClient

**Justificacion**:
- Evita el agotamiento de sockets (socket exhaustion)
- Gestiona automaticamente el ciclo de vida de los handlers
- Permite configuracion centralizada de la URL base

### Decision 3: Patron Repository con Interfaces
**Fecha**: 2026-01-20
**Descripcion**: Cada repositorio implementa una interfaz para inyeccion de dependencias

**Justificacion**:
- Facilita el testing mediante mocks
- Desacopla la implementacion del contrato
- Permite cambiar facilmente la fuente de datos

### Decision 4: Middleware de Manejo de Errores Global
**Fecha**: 2026-01-20
**Descripcion**: Se implementa un middleware que captura todas las excepciones no manejadas

**Justificacion**:
- Centralizacion del manejo de errores en un solo punto
- Respuestas de error consistentes para todos los endpoints
- Seguridad: no expone detalles internos (stack traces) en produccion

### Decision 5: Logging Estructurado
**Fecha**: 2026-01-20
**Descripcion**: Se implementa logging en multiples capas con parametros estructurados

**Justificacion**:
- Observabilidad: monitoreo del trafico y comportamiento de la API
- Trazabilidad: TraceId permite seguir una peticion a traves de todo el sistema
- Diagnostico: facilita identificar origen de problemas

### Decision 6: Autenticacion JWT con Permisos Granulares
**Fecha**: 2026-01-20
**Descripcion**: Se implementa autenticacion JWT con sistema de roles y permisos por recurso/operacion

**Justificacion**:
- **JWT (JSON Web Tokens)**:
  - Stateless: no requiere almacenar sesiones en servidor
  - Escalable: funciona bien en arquitecturas distribuidas
  - Seguro: firmado digitalmente, no modificable
  - Estandar: ampliamente adoptado en la industria

- **Permisos Granulares (RBAC)**:
  - Control fino: permisos especificos por recurso y operacion
  - Flexibilidad: roles predefinidos + permisos directos
  - Principio de minimo privilegio: usuarios solo tienen lo necesario
  - Auditoria: facil rastrear quien puede hacer que

- **Proteccion solo en escritura**:
  - GET publicos: datos ficticios, sin informacion sensible
  - POST/PUT/DELETE protegidos: operaciones que modifican datos

---

## Registro de Commits

| Commit | Descripcion | Justificacion |
|--------|-------------|---------------|
| #1 | Estructura base del proyecto en capas | Arquitectura limpia con separacion de responsabilidades siguiendo SOLID |
| #2 | Manejo de errores global y logging | Mejora observabilidad, seguridad y mantenibilidad de la API |
| #3 | Autenticacion JWT con permisos granulares | Seguridad en endpoints de escritura, sistema RBAC flexible |

---

## Sistema de Roles y Permisos

### Roles Predefinidos

| Rol | Descripcion | Permisos |
|-----|-------------|----------|
| **Admin** | Acceso total | Todos los recursos: Create, Read, Update, Delete |
| **Editor** | Crear y modificar | Todos los recursos: Create, Read, Update (sin Delete) |
| **Viewer** | Solo lectura | Todos los recursos: Read |

### Usuarios de Prueba

| Usuario | Contrasena | Rol |
|---------|------------|-----|
| admin | admin123 | Admin |
| editor | editor123 | Editor |
| viewer | viewer123 | Viewer |

### Formato de Permisos

```
{Recurso}.{Operacion}

Recursos: Posts, Comments, Users, Albums, Photos, Todos
Operaciones: Create, Read, Update, Delete

Ejemplos: Posts.Create, Users.Delete, Albums.Update
```

---

## Endpoints de Autenticacion

| Metodo | Endpoint | Descripcion | Autenticacion |
|--------|----------|-------------|---------------|
| POST | /api/auth/login | Iniciar sesion | No |
| POST | /api/auth/register | Registrar usuario | No |
| GET | /api/auth/me | Obtener usuario actual | Si |
| GET | /api/auth/roles | Listar roles disponibles | No |
| GET | /api/auth/validate | Validar token | Si |

---

## Estructura del Proyecto Actualizada

```
src/JsonPlaceholderApi/
├── Auth/                                   <-- NUEVO
│   ├── PermissionRequirement.cs
│   └── RequirePermissionAttribute.cs
├── Controllers/
│   ├── AuthController.cs                   <-- NUEVO
│   ├── PostsController.cs                  <-- Actualizado con [RequirePermission]
│   ├── CommentsController.cs               <-- Actualizado
│   ├── UsersController.cs                  <-- Actualizado
│   ├── AlbumsController.cs                 <-- Actualizado
│   ├── PhotosController.cs                 <-- Actualizado
│   └── TodosController.cs                  <-- Actualizado
├── Middleware/
│   ├── GlobalExceptionHandlerMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Models/
│   ├── Auth/                               <-- NUEVO
│   │   ├── AppUser.cs
│   │   ├── AuthDtos.cs
│   │   ├── JwtSettings.cs
│   │   ├── Permission.cs
│   │   └── Role.cs
│   ├── Post.cs
│   ├── Comment.cs
│   ├── User.cs
│   ├── Album.cs
│   ├── Photo.cs
│   └── TodoItem.cs
├── Repositories/
│   └── ...
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs                 <-- NUEVO
│   │   └── ...
│   ├── AuthService.cs                      <-- NUEVO
│   └── ...
├── Program.cs                              <-- Actualizado con JWT
├── appsettings.json                        <-- Actualizado con JwtSettings
└── JsonPlaceholderApi.csproj               <-- Actualizado con paquete JWT
```

---

## Proximos Pasos
1. [x] Crear archivo HISTORICO.md
2. [x] Crear proyecto .NET 8 Web API
3. [x] Implementar capa de Modelos
4. [x] Implementar capa de Repositorios
5. [x] Implementar capa de Servicios
6. [x] Implementar capa de Controladores
7. [x] Configurar Swagger
8. [x] Probar endpoints
9. [x] Agregar manejo de errores global
10. [x] Agregar logging
11. [x] Implementar autenticacion JWT
12. [x] Implementar sistema de roles y permisos
13. [ ] Agregar refresh tokens (opcional)
14. [ ] Agregar tests unitarios (opcional)
15. [ ] Agregar health checks (opcional)
