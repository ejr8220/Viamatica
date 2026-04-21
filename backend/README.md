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
    "DefaultConnection": "Server=localhost,1433;Database=ViamaticaDB;User Id=sa;Password=Viamatica2026!;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

Valores definidos para Docker:

- Usuario: `sa`
- Password: `Viamatica2026!`
- Base de datos: `ViamaticaDB`

## Migraciones

```bash
cd backend\Viamatica.Infrastructure
dotnet ef database update --startup-project ..\Viamatica.API
```

## Credenciales seed

Si la base está vacía, al iniciar la API se crean credenciales mínimas:

- Administrador: `admin2026` / `Admin2026`
- Gestor: `gestor01` / `Gestor2026`
- Cajero: `cajero01` / `Cajero2026`

## Módulos disponibles

- Login JWT con roles y bloqueo para usuarios no aprobados.
- Usuarios: CRUD + aprobación.
- Clientes: CRUD + búsqueda por identificación.
- Cajas: CRUD + asignación de cajeros + sesión activa para bloqueo lógico.
- Turnos: CRUD por caja.
- Servicios: CRUD.
- Contratos: alta, actualización, pagos, cambio de servicio, cambio de forma de pago y cancelación.
- Atenciones: inicio/cierre/cancelación ligadas a turno, cliente y contrato opcional.

## Postman

Colección versionada:

- `backend/postman/Viamatica.postman_collection.json`
