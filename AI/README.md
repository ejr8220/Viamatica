# AI - Skills y prompts utilizados

Esta carpeta documenta de forma **técnica, profesional y trazable** el uso de agentes, skills y prompts aplicados durante el desarrollo de la solución **Viamatica**.

## Objetivo de esta carpeta

Cumplir el entregable solicitado para adjuntar:

- skills utilizados
- prompts utilizados
- contexto técnico de uso
- alcance funcional cubierto
- criterio de aplicación por fase

## Estructura

- `AI\skills-used.md`: agentes, especialidades y criterios de uso
- `AI\prompts-used.md`: prompts técnicos consolidados usados para backend y frontend
- `AI\requirements-traceability.md`: traducción profesional de los requerimientos del usuario a objetivos técnicos de implementación

## Resumen ejecutivo

Durante el desarrollo se utilizaron agentes especializados para:

1. **Backend .NET 9**
   - arquitectura Onion / DDD
   - JWT con claims protegidas
   - Entity Framework Core
   - Swagger
   - Serilog
   - stored procedure y consumo real desde aplicación

2. **Frontend Angular 20**
   - arquitectura modular
   - layout administrativo con PrimeNG
   - guards, interceptor y sesión JWT
   - componentes reutilizables
   - paginación y filtros por columna
   - carga del menú desde backend según rol

## Consideraciones

- La documentación aquí contenida resume y formaliza los prompts técnicos y decisiones operativas utilizadas durante la construcción del proyecto.
- Se documentan los agentes realmente empleados y el tipo de instrucciones dadas para resolver cada fase.
- Los prompts están redactados de manera profesional para facilitar auditoría técnica, trazabilidad y mantenimiento futuro.
