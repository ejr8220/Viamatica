# Backend Viamatica

Solución .NET 9 con tres proyectos:

- `Viamatica.API`: punto de entrada.
- `Viamatica.Domain`: entidades y reglas de dominio.
- `Viamatica.Infrastructure`: Entity Framework Core, DbContext, configuraciones y migraciones.

## Base de datos

La conexión se configura con `DefaultConnection` en `Viamatica.API/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=ViamaticaDb;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True"
  }
}
```

## Migraciones

```bash
cd backend\Viamatica.Infrastructure
dotnet ef database update --startup-project ..\Viamatica.API
```
