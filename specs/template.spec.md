# Template agnóstico TecNM

## Alcance

Base técnica para aplicaciones y microservicios: .NET 10, EF Core, PostgreSQL, Vue 3, TypeScript y componentes institucionales. Cada proyecto define su dominio.

- Backend conserva configuración, DbContext sin entidades y health check público.
- `Modules/Auth` reserva la integración futura con el servicio de login TecNM, sin autenticación activa.
- Frontend conserva shell, tema y componentes reutilizables; muestra una página inicial pública y neutra.
- Sin padrón, cuentas, fichas, roles, semillas ni endpoints de negocio.
- `database.sql` en raíz es el punto de entrada para inicializar PostgreSQL; inicialmente solo contiene comentarios.
- Setup conserva HTTPS, comprobación de puertos, contenedores separados y persistencia. No depende de tablas concretas.

## Responsabilidad de cada proyecto

Definir módulos, entidades, esquema SQL, migraciones, permisos e integración de identidad. Actualizar specs antes de código y mantener índices de módulos en ambos `AGENTS.md`.

## Aceptación

1. Frontend compila y abre `/` sin sesión; no ofrece login, logout ni perfiles.
2. Backend compila y responde `200 { "status": "ok" }` en `/api/health`.
3. Rutas antiguas de Auth y People devuelven `404`.
4. PostgreSQL vacío inicia con el `database.sql` de comentarios, sin tablas de aplicación.
5. SQL propio se ejecuta al inicializar un volumen nuevo; reiniciar no vuelve a ejecutarlo ni borra datos existentes.
6. No existen claves JWT, contraseñas demo ni validaciones del esquema institucional.
