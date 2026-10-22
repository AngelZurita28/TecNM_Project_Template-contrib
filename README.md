# TecNM Campus Monclova — Plantilla Base de Proyectos

Este repositorio sirve como la **plantilla oficial de referencia (template)** para el desarrollo de todas las aplicaciones web y servicios del **TecNM Campus Monclova**.

Su propósito es estandarizar la arquitectura, calidad de código y patrones de diseño en el desarrollo de software institucional, asegurando mantenibilidad, escalabilidad e interoperabilidad entre proyectos.


## Padrón institucional listo para desarrollo

Base mínima para alumnos, docentes y administrativos. Seis tablas: `people`, `users`, `students`, `employees`, `programs`, `departments`. Una persona conserva su UUID aunque cambie de matrícula, cuenta o vínculo. No incluye horarios, materias, calificaciones ni trámites.

- [Auditoría del SQL histórico: 85 tablas](docs/legacy-database-review.md).
- [Modelo y contrato de integración](docs/identity-model.md).
- [Especificación del padrón](specs/people.spec.md).
- [DDL PostgreSQL](database.sql), [semilla ficticia](seed-development.sql) y [visor relacional](visor_er.html).

### Arranque completo con Docker

Requisitos en el host: Docker con Compose, Python 3 y OpenSSL. Backend y frontend se compilan dentro de Docker.

```bash
bash scripts/setup-dev.sh
```

El script inicia **tres contenedores separados** en el mismo Compose:

| Contenedor | Acceso predeterminado |
| --- | --- |
| `tecnm-identity-frontend` | **https://localhost:5173** |
| `tecnm-identity-backend` | https://localhost:7001/api/health |
| `tecnm-identity-postgres` | `db:5432` en red interna, sin puerto publicado. |

Frontend y backend publican solo en `127.0.0.1`. Frontend consulta `/api` mediante su proxy HTTPS al backend. Solo backend y PostgreSQL comparten la red privada de datos. Administración de la base: `docker compose exec db psql -U tecnm_app -d tecnm_identity`.

Elegir puertos y comprobar disponibilidad:

```bash
bash scripts/setup-dev.sh --frontend-port 5180 --backend-port 7010
bash scripts/setup-dev.sh --frontend-port 5180 --backend-port 7010 --check-ports
```

Si un puerto está ocupado, avisa y propone uno libre antes de construir o levantar contenedores. En terminal permite introducir otro; sin terminal termina con mensaje y código 2. No mata procesos ni cambia puertos silenciosamente. Permite reutilizar los puertos de sus propios contenedores al reiniciar.

Los puertos y secretos se guardan en `.env` (ver [.env.example](.env.example)). Nombre alternativo: `--project-name tecnm-campus`. Otro nombre crea un proyecto y volumen independientes; no traslada los datos del proyecto anterior.

El volumen `tecnm-identity_identity_data` conserva la base ya existente. DDL y semillas se cargan al crear una base vacía. Repetir el script conserva perfiles y contraseñas. No usar `docker compose down -v` si se quieren conservar datos.

```bash
docker compose ps
docker compose logs -f
docker compose stop
# Reiniciar con configuración y certificados ya generados:
docker compose up -d --wait
# Tras cambiar código, reconstruir e iniciar:
bash scripts/setup-dev.sh
```

El frontend sirve la compilación Vue con Nginx; no tiene recarga automática. HTTPS usa un certificado local autofirmado generado en `.docker/certs/`, ignorado por Git. El navegador puede pedir aceptar el certificado de desarrollo. En producción se requieren certificados y credenciales reales; esta configuración incluye usuarios demo.

| Usuario | Perfil de prueba |
| --- | --- |
| `alumno` | Alumno inscrito; matrícula `M20260001`. |
| `alumno.baja` | Alumno con baja temporal y sin inscripción. |
| `docente` | Docente; empleado `D0003`. |
| `administrativo` | Administrativo con permiso común. |
| `admin` | Administrativo con permiso de administrador del sistema. |
| `mixto` | Alumno, docente y administrativo en una sola persona. |

Contraseña común: **`TecNM-Demo-2026!`**, exclusiva de desarrollo. Al entrar se muestran identidad, contacto, domicilio, emergencia y fichas aplicables. «Actualizar información» vuelve a consultar la base.

### Verificación

Con los tres contenedores activos:

```bash
python3 scripts/check-dev-ports.py
python3 scripts/check-identity.py
# Por defecto la API usa 7001; si se cambia, indicar URL para este smoke:
API_URL=https://localhost:7001 bash scripts/smoke-auth.sh
```

`check-identity.py` verifica cuentas demo y modifica temporalmente su nombre, permiso y habilitación; restaura valores en `finally`. Usarlo solo con la base aislada de desarrollo. Verifica restricciones dentro de transacciones revertidas.

`scriptAlumnosTEC.sql` se conserva intacto como fuente histórica SQL Server. No contiene las filas reales del padrón y no se ejecuta para instalar esta versión. Producción debe aplicar únicamente el DDL, cargar registros verificados y aprovisionar cuentas reales; no cargar la semilla demo.

---

## 🤖 Desarrollo Guiado por Especificaciones (Spec-Driven Development - SDD)

El trabajo y colaboración con **Agentes de Inteligencia Artificial** en este ecosistema se rige estrictamente bajo **SDD (Spec-Driven Development)**:

1. **Especificación Primero:** Antes de generar o modificar código en cualquier módulo, se deben redactar o actualizar los documentos en su carpeta local `specs/`.
2. **Contexto Aislado:** Las especificaciones funcionales, casos de uso, contratos y criterios de aceptación residen dentro del propio módulo/slice para dar contexto preciso a los agentes.
3. **Validación Continua:** Los agentes implementan código y pruebas contrastando directamente contra las especificaciones del slice correspondiente.
4. **Regla de Redacción:** Toda especificación y documentación generada por IA debe redactarse utilizando la skill **caveman** para garantizar máxima concisión técnica y eliminar contenido redundante.

### Centralización y Contexto Maestro (`AGENTS.md`)

Para inicializar y mantener la sincronización del contexto global, cada repositorio/proyecto (Backend y Frontend por separado) debe contar con un archivo **`AGENTS.md`** en su raíz:

- **Punto de Entrada del Agente:** Es el primer documento que la IA debe consultar para comprender el sistema.
- **Contenido Requerido:**
  - Resumen y objetivos del proyecto.
  - Reglas operativas, convenciones de código y estándares del campus.
  - Índice / mapa centralizado que referencia a todos los módulos (`Modules/` o `src/modules/`), sus componentes clave y sus carpetas `specs/`.
- **Mantenimiento:** Al inicializar un proyecto o crear un nuevo slice/módulo, debe registrarse y enlazarse inmediatamente en `AGENTS.md`.

### Skills Recomendadas para Agentes

Se recomienda el uso de las siguientes skills para optimizar el flujo de desarrollo, diseño e implementación:

- **caveman:** Reducción de ruido y documentación ultra concisa. *(Obligatoria para generar specs)*.
- **frontend-design:** Dirección estética y diseño visual intencional.
- **ui-ux-pro-max:** Sistema de diseño UI/UX, patrones de interacción, accesibilidad y paletas.
- **ponytail:** Simplicidad pragmática y código minimalista (evita sobreingeniería).

#### Comandos de Instalación de Skills

```bash
npx skills add https://github.com/juliusbrussee/caveman --skill caveman
npx skills add https://github.com/anthropics/skills --skill frontend-design
npx skills add https://github.com/nextlevelbuilder/ui-ux-pro-max-skill --skill ui-ux-pro-max
npx skills add https://github.com/dietrichgebert/ponytail --skill ponytail
```

---

## 🏛️ Especificaciones y Arquitecturas

El proyecto se rige por los siguientes estándares arquitectónicos:

- **[Estándar de Autenticación y Sesiones](./AUTH.md):** Especificación de seguridad, cookies `HttpOnly`, ciclo de vida del JWT y soporte para login local y federado (Microsoft Entra / Google).
- **[Arquitectura Backend (.NET 10)](./BACKEND_ARCHITECTURE.md):** Estructura y lineamientos para la API REST, persistencia con EF Core y PostgreSQL, y módulos de negocio con especificaciones locales.
- **[Arquitectura Frontend (Vue 3 + TypeScript)](./FRONTEND_ARCHITECTURE.md):** Estructura y lineamientos para la aplicación SPA con Vite, rutas modulares descentralizadas, componentes reutilizables y especificaciones por slice.

---

## 🌐 Convenciones Transversales

- **Nomenclatura de Base de Datos:** `snake_case` en tablas, columnas, índices, constraints y vistas de PostgreSQL.
- **Idioma del Código:** inglés para todo identificador, archivo, ruta y API.
- **Idioma de la Interfaz:** español de México (es-MX) para todo texto visible al usuario, mensajes de error y de estado.

---

## 📋 Planes de Implementación (`plans/`)

Las hojas de ruta, fases de ejecución técnica y planes de trabajo generados para agentes o desarrolladores se centralizan en el directorio `plans/`:

- **[01. Plan de Autenticación Local](./plans/01-auth-implementation.md):** Plan inicial de construcción del esqueleto Backend, Frontend y flujo de autenticación local con PostgreSQL y JWT en cookies.
