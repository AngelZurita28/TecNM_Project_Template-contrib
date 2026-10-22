# Inicio del template

## Alcance

Página pública de bienvenida sin datos ni operaciones de negocio.

- `/` usa `MainLayout` persistente y `UiBasePage` con `fluid=true`.
- Mostrar título del proyecto y mensaje neutro; reutilizar componentes institucionales.
- Conservar navegación de inicio y selector de tema claro/oscuro.
- Sin login, logout, perfil, cookies de sesión, guards ni peticiones automáticas a APIs de dominio.
- Cliente HTTP compartido conserva base URL configurable, sin credenciales ni redirecciones de autenticación.
- Rutas de módulos se exportan por `index.ts` y se cargan de forma diferida.
- Textos en español de México; identidad TecNM y diseño responsive existentes.

## Aceptación

Compilación TypeScript/Vue correcta. Inicio accesible sin sesión. Layout funciona con header, sidebar, ambos o ninguno. La interfaz no ofrece acciones de autenticación ni datos del padrón.
