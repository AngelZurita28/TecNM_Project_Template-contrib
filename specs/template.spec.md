# Template agnóstico TecNM

## Alcance

Base técnica para aplicaciones y microservicios: .NET 10, EF Core, PostgreSQL, Vue 3, TypeScript y componentes institucionales. Cada proyecto define su dominio.

- Backend conserva configuración, DbContext sin entidades y health check público.
- Frontend conserva shell, tema y componentes reutilizables; muestra una página inicial pública y neutra.
- Los módulos y datos de negocio se definen en cada proyecto.
- `database.sql` en raíz es el punto de entrada para inicializar PostgreSQL; inicialmente solo contiene comentarios.
- Setup conserva HTTPS, comprobación de puertos, contenedores separados y persistencia. No depende de tablas concretas.

## Responsabilidad de cada proyecto

Definir módulos, entidades, esquema SQL, migraciones e integraciones. Actualizar specs antes de cambiar comportamiento o contratos. Los `AGENTS.md` contienen patrones de diseño y reglas de desarrollo; los requisitos funcionales pertenecen a las specs.

## Aceptación

1. Frontend compila y abre la página pública `/` con navegación y selector de tema.
2. Backend compila y responde `200 { "status": "ok" }` en `/api/health`.
3. Una ruta de API no registrada devuelve `404`.
4. PostgreSQL vacío inicia con el `database.sql` de comentarios, sin tablas de aplicación.
5. SQL propio se ejecuta al inicializar un volumen nuevo; reiniciar no vuelve a ejecutarlo ni borra datos existentes.
