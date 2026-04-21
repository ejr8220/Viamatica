# Skills / agentes utilizados

## 1. Enfoque general de asistencia

Para el desarrollo de la solución se utilizó una combinación de:

- razonamiento y edición directa sobre el repositorio
- inspección técnica de código existente
- ejecución de builds y validaciones
- delegación controlada a agentes especializados por dominio

## 2. Agentes especializados utilizados

### 2.1 Agente `backend`

**Propósito**

Resolver tareas de implementación y refactorización del backend con foco en:

- .NET 9
- JWT y claims
- Entity Framework Core
- DDD / Onion Architecture
- Swagger
- eliminaciones lógicas
- integración con SQL Server

**Cuándo se utilizó**

- al estructurar o refactorizar capas de backend
- al alinear el proyecto con Onion Architecture
- al implementar autenticación JWT
- al ajustar consumo de stored procedure
- al completar módulos funcionales del backend

**Tipo de entregables generados**

- servicios de aplicación
- repositorios y queries
- controladores
- integración EF Core
- seguridad JWT
- configuración técnica del API

### 2.2 Agente `frontend`

**Propósito**

Resolver tareas de implementación del frontend con foco en:

- Angular 20
- PrimeNG
- autenticación y flujo de sesión
- guardas de ruta
- diseño modular
- dashboard y módulos administrativos
- componentes reutilizables

**Cuándo se utilizó**

- al montar la arquitectura base Angular
- al estructurar módulos lazy loaded
- al construir layout principal
- al implementar páginas funcionales y componentes shared
- al preparar el frontend para build y dockerización

**Tipo de entregables generados**

- módulos Angular
- servicios de dominio frontend
- guards e interceptor
- layout topbar/sidebar/footer
- páginas de login, welcome, dashboard y mantenimiento
- componentes reutilizables tipo grid y búsqueda

## 3. Skills y capacidades técnicas aplicadas

Aunque la solución se desarrolló principalmente con agentes especializados por dominio, las capacidades efectivamente utilizadas fueron:

### 3.1 Exploración técnica del código

- identificación de controladores y endpoints reales
- lectura de DTOs y contratos backend
- trazabilidad entre requisitos y módulos implementados
- auditoría de cumplimiento respecto a la especificación del usuario

### 3.2 Generación de estructura y refactor técnico

- generación de módulos, componentes y servicios
- normalización de arquitectura por capas
- separación de responsabilidades en backend y frontend
- acoplamiento controlado entre API y cliente

### 3.3 Integración y validación

- compilación del backend
- build del frontend
- build de imagen Docker del frontend
- validación de `docker-compose`
- revisión de wiring de Serilog, JWT y navegación por rol

## 4. Criterios profesionales de uso

Los agentes no se utilizaron como reemplazo ciego de implementación, sino bajo estos criterios:

1. **especialización por dominio**
   - backend para decisiones .NET / EF / JWT / arquitectura
   - frontend para Angular / PrimeNG / layout / UX funcional

2. **validación posterior**
   - todo resultado fue revisado sobre el código real
   - se inspeccionaron archivos clave antes de dar por cerrado un cambio

3. **alineación a requerimientos**
   - cada fase se contrastó con lo solicitado por el usuario
   - cuando se detectó incumplimiento, se corrigió o se reportó como parcial

4. **trazabilidad**
   - cada cambio importante quedó asociado a un objetivo funcional o técnico concreto

## 5. Áreas donde el uso de agentes aportó valor

### Backend

- aceleración de estructura inicial por capas
- integración de JWT con claims protegidas
- refactor hacia cumplimiento Onion
- incorporación de Serilog
- consumo real de SP
- consolidación de controladores y servicios

### Frontend

- montaje rápido de base Angular 20
- creación masiva de módulos y páginas
- armado del layout administrativo
- implementación de servicios, guards e interceptor
- construcción de pantallas reutilizables y grids
- preparación para build y Docker

## 6. Limitaciones observadas y correcciones aplicadas

Durante el uso de agentes se detectaron y corrigieron manualmente varios puntos:

- estructuras preliminares que no cumplían al 100% con el requerimiento funcional
- validaciones incompletas o parciales en algunos formularios
- fallback local del menú, posteriormente eliminado para obligar consumo desde base de datos
- nombres temporales generados por scaffolding (`viamatica-frontend-temp`), luego normalizados
- rutas o guards que requirieron revisión adicional

Esto refuerza que el uso de agentes fue **asistido y supervisado**, no automático ni descontrolado.
