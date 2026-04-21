# Prompts técnicos utilizados

## 1. Principio de diseño de prompts

Los prompts utilizados siguieron un patrón profesional de especificación:

- contexto del repositorio
- objetivo funcional
- restricciones técnicas
- contratos conocidos del backend
- estructura objetivo
- criterios de aceptación

## 2. Prompt técnico consolidado para backend

### Objetivo

Implementar un backend en **.NET 9** alineado con los requerimientos de la prueba técnica:

- arquitectura Onion / DDD
- Swagger
- Entity Framework Core
- JWT con claims protegidas
- soft delete
- Serilog
- stored procedure y consumo real
- Postman
- Docker

### Prompt consolidado

> Implementar el backend del proyecto Viamatica en .NET 9 respetando Onion Architecture. Separar API, Application, Domain e Infrastructure. Modelar entidades según el esquema relacional entregado. Configurar Entity Framework Core con Fluent API, migraciones y SQL Server. Implementar autenticación JWT con claims sensibles protegidas, Swagger con Bearer, Serilog para errores, alertas e inicios de sesión, eliminación lógica y consumo real de al menos un stored procedure o función. Exponer controladores REST para usuarios, clientes, cajas, turnos, servicios, contratos y atenciones. Garantizar compatibilidad con Docker y generar una colección Postman para pruebas funcionales.

### Variantes técnicas derivadas

#### Variante para arquitectura

> Refactorizar el backend para eliminar acoplamientos directos de Application a EF Core. Introducir puertos, repositorios, queries y unidad de trabajo en Application, dejando las implementaciones concretas en Infrastructure.

#### Variante para seguridad

> Ajustar la autenticación JWT para que los claims de negocio e identidad no queden visibles en texto plano dentro del token. Mantener el funcionamiento de autorización reconstruyendo claims operativas del lado servidor.

#### Variante para navegación por rol

> Implementar un endpoint de navegación que devuelva el árbol de menú según el rol autenticado, leyendo la estructura desde base de datos y no desde configuración estática o código hardcodeado.

## 3. Prompt técnico consolidado para frontend

### Objetivo

Construir un frontend en **Angular 20 + PrimeNG** con enfoque modular y administrativo.

### Prompt consolidado

> Implementar el frontend del proyecto Viamatica en Angular 20 usando PrimeNG. Organizar la solución con módulos lazy loaded para auth, welcome, dashboard, users, clients, cashes, turns, services, contracts y attentions. Implementar layout principal con topbar, sidebar, footer y content. Cargar el menú desde backend según rol, consumiendo un endpoint dedicado y representándolo como árbol en el sidebar. Crear guards de autenticación y rol, interceptor HTTP para JWT y cabeceras, mensajes de error/éxito, loading global, componentes reutilizables tipo grid-shared e input-search-shared, paginación y filtros por columna. Integrar formularios validados, login con recuperación de contraseña y soporte para carga masiva de usuarios y clientes desde xlsx/csv. Preparar el frontend para build y dockerización.

### Variantes técnicas derivadas

#### Variante para layout

> Basarse funcionalmente en una distribución tipo ERP administrativo, implementada con PrimeNG, con sidebar jerárquico, topbar informativa, footer y área central para módulos.

#### Variante para grid reutilizable

> Crear un grid reutilizable en PrimeNG que soporte paginación, ordenamiento, filtros por columna, filtros combinados, limpieza de filtros y acciones por fila.

#### Variante para importación masiva

> Implementar un flujo de carga masiva para usuarios y clientes desde archivos xlsx/csv, con parsing en cliente, validación estructural, procesamiento por fila y reporte de resultados.

## 4. Prompt técnico de auditoría de cumplimiento

Se utilizó un enfoque explícito de auditoría para comparar el estado real de la solución con el requerimiento del usuario.

### Prompt consolidado

> Revisar el proyecto implementado y determinar, punto por punto, qué requisitos cumplen, cuáles cumplen parcialmente y cuáles faltan. Basar la evaluación en el código real, contratos API existentes, wiring de rutas, guards, componentes, layout, dockerización y entregables documentales.

## 5. Prompt técnico de endurecimiento final

Durante el cierre se utilizaron prompts orientados a consolidar cumplimiento:

### Menú desde base de datos

> El menú debe cargarse desde base de datos de acuerdo al rol. Eliminar cualquier fallback local y garantizar que el frontend dependa del endpoint real del backend para construir el árbol de navegación.

### Consistencia de validación de contraseñas

> Alinear la validación de contraseñas del frontend con el requerimiento mínimo de 8 caracteres y con la validación del backend, tanto en login como en mantenimiento/importación de usuarios.

### Documentación AI

> Documentar de manera extensa, técnica y profesional los skills, agentes y prompts utilizados durante el desarrollo, organizándolos en una carpeta AI como parte de los entregables.

## 6. Criterios de calidad en redacción de prompts

Los prompts fueron formulados con estos criterios:

1. **orientación a resultados**
   - cada prompt define un entregable verificable

2. **alineación a contratos reales**
   - se trabaja sobre endpoints, DTOs y restricciones existentes

3. **restricciones explícitas**
   - arquitectura, librerías, estilo de layout, seguridad y comportamiento requerido

4. **cierre verificable**
   - build, docker, navegación, guards, documentación y consistencia funcional
