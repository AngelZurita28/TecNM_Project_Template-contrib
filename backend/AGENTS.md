# Contexto backend

## Objetivo

Base agnóstica para microservicios con .NET 10, EF Core y PostgreSQL. Sin dominio de negocio ni autenticación activa. `Modules/Auth` reserva la integración futura con el servicio de login TecNM.

## Lectura obligatoria

1. [Arquitectura backend](../BACKEND_ARCHITECTURE.md).
2. [Alcance del template](../specs/template.spec.md).
3. [Integración de autenticación](../AUTH.md) y [spec compartida](../specs/auth.spec.md).
4. [Setup y base de datos](../docs/setup.md).

## Reglas

- Aplicar SDD: actualizar spec antes de código.
- Usar C# y .NET 10, organización por vertical slice y controllers delgados.
- Poner reglas y consultas en servicios; exponer DTOs, nunca entidades EF.
- Usar EF Core async y PostgreSQL. No agregar Repository sin necesidad real.
- Cero SQL en código (`raw SQL` / `FromSqlRaw`). Usar LINQ asíncrono; consultas complejas mediante vistas SQL mapeadas en EF Core.
- Acciones mutativas deben prevenir duplicados y aplicar idempotencia cuando corresponda.
- Conservar `ConnectionStrings__DefaultConnection` fuera de Git.
- No implementar login local, generar tokens ni crear cuentas demo. Integrar Auth solo a partir del contrato oficial TecNM.
- `database.sql` en raíz pertenece al microservicio; mantenerlo alineado con entidades y estrategia de migraciones.
- Base de datos en `snake_case`; identificadores, archivos y rutas en inglés.
- Textos dirigidos al usuario en español de México. No devolver excepciones crudas.
- Mantener el mapa de módulos y sus specs al agregar funcionalidad.

## Comandos

```bash
dotnet user-secrets set --project backend/TecNM.Api.csproj "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=5432;Database=tecnm_template;Username=tecnm_app;Password=<local>"
dotnet restore backend/TecNM.Api.csproj
dotnet build backend/TecNM.Api.csproj --no-restore
dotnet run --project backend/TecNM.Api.csproj
```

La ejecución manual requiere PostgreSQL propio; Compose no publica su puerto. Verificación: `/api/health` público y rutas antiguas Auth/People ausentes.

## Mapa

- [`Core/`](./Core/README.md): ubicación para futuras entidades compartidas, sin modelo predefinido.
- [`Data/TecNMDbContext.cs`](./Data/TecNMDbContext.cs): contexto vacío y descubrimiento de configuraciones EF del proyecto.
- [`Modules/Auth/specs/`](./Modules/Auth/specs/auth.spec.md): alcance de la integración pendiente.
- [`Modules/Auth/Integration/`](./Modules/Auth/Integration/README.md): guía para conectar la API TecNM.
- [Health check](../specs/health.spec.md): único endpoint incluido.
- [Arranque Docker](../specs/docker.spec.md): servicios separados, PostgreSQL privado y comprobación de puertos.
