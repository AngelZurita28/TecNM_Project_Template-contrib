# Contexto frontend

## Skills obligatorias

Antes de proceder con cualquier indicación del usuario, intentar cargar estas skills:

1. `caveman`: respuestas y documentación concisas.
2. `ponytail`: implementación mínima, mantenible y sin sobreingeniería.
3. `frontend-design`: diseño visual intencional y coherente con identidad TecNM.
4. `ui-ux-pro-max`: consistencia UI/UX, accesibilidad y responsive.

Usar las cuatro durante trabajo frontend. Si alguna no existe o no puede cargarse, informar al usuario y preguntar si desea proceder sin ella. No iniciar cambios hasta recibir respuesta.

## Objetivo

Mantener base SPA Vue 3 con TypeScript, inicio público, layout persistente y componentes reutilizables. Cada proyecto define sus módulos; el template no autentica ni incluye funcionalidad de negocio.

## Lectura obligatoria

1. [Arquitectura frontend](../FRONTEND_ARCHITECTURE.md)
2. [Estándar de autenticación](../AUTH.md)
3. [Contrato canónico de autenticación](../specs/auth.spec.md)
4. [Spec de inicio](./src/modules/home/specs/home.spec.md) y [alcance del template](../specs/template.spec.md)

## Reglas

- Aplicar SDD. Actualizar spec antes de código.
- Priorizar siempre componentes existentes en `src/shared/components/`. Buscar, reutilizar y componer antes de crear componente o CSS local.
- Extender componente compartido cuando necesidad sea genérica para varias vistas. Mantener lógica exclusiva dentro del módulo correspondiente.
- Usar `UiBasePage` como raíz de contenido en todas las vistas de página, públicas o autenticadas.
- Usar `UiBasePage` con `fluid=true` por defecto para ocupar ancho disponible de lado a lado.
- Usar modo centrado solo cuando usuario o spec lo indique, o cuando contenido requiera lectura estrecha tipo artículo o formulario compacto.
- Usar Vue 3, Composition API, `<script setup lang="ts">`, TypeScript y Vite.
- Organizar por vertical slice. Exponer API pública mediante `index.ts`.
- Mantener rutas del módulo en `routes.ts` y cargar vistas de forma diferida.
- Centralizar Axios e interceptores en `src/shared/api/`.
- No agregar cookies, credenciales, login, guards ni redirecciones de autenticación. La integración futura con el servicio TecNM se define en backend según su contrato oficial.
- Prevenir dobles peticiones: toda acción debe ser única. Bloquear reentrancia en manejadores/composables (`if (isSubmitting.value) return;`) y deshabilitar controles durante el procesamiento (`loading`, `disabled`).
- Usar CSS nativo y tokens semánticos. Mantener formulario accesible y responsive.
- No agregar Pinia ni dependencias sin uso directo.
- Actualizar specs antes de cambiar contratos de API o comportamiento de módulos.
- Código en inglés: componentes, composables, tipos, variables, archivos y rutas (`HomeView.vue`, `useTheme`, `ItemResponse`).
- Textos visibles al usuario en español de México (es-MX): etiquetas, títulos, botones, placeholders, mensajes de error y de estado (`"Inicio"`, `"Guardar cambios"`).
- No mezclar idiomas en la misma cadena. No dejar texto placeholder en inglés en la UI.

## Comandos previstos

```bash
pnpm --prefix frontend install
pnpm --prefix frontend run dev -- --host
pnpm --prefix frontend run build
```

Verificación: inicio público sin sesión, tema claro/oscuro, navegación y responsive. Ejecutar build y `node scripts/check-layout.mjs` desde frontend.

## Mapa

- [`src/app/`](./src/app/): arranque SPA y router agregador.
- [`src/shared/api/`](./src/shared/api/): Axios base sin credenciales ni redirecciones de autenticación.
- [`src/shared/styles/`](./src/shared/styles/): tokens y estilos globales.
- [`src/shared/composables/`](./src/shared/composables/): estado reactivo transversal; [spec de tema](./src/shared/composables/useTheme-spec.md).
- [`src/modules/home/`](./src/modules/home/): inicio público, rutas y API del módulo.
- [`src/modules/home/specs/`](./src/modules/home/specs/home.spec.md): contexto de inicio.
- [Contrato raíz](../specs/auth.spec.md): única fuente de verdad compartida.

- [`src/shared/components/`](./src/shared/components/): biblioteca visual reutilizable; [spec](./src/shared/components/specs/components.spec.md).
- [`src/shared/views/`](./src/shared/views/): vistas genéricas compartidas (404); [spec](./src/shared/views/specs/notFound.spec.md).

- [`src/app/layouts/`](./src/app/layouts/mainLayout-spec.md): MainLayout persistente; configuración independiente de header, sidebar, marca y navegación.

- [Integración futura de identidad](../AUTH.md): estructura exclusiva de backend, sin autenticación activa.

- [Arranque Docker](../specs/docker.spec.md): un Compose, contenedores separados, PostgreSQL privado y puertos comprobados. `bash scripts/setup-dev.sh` inicia todo.
