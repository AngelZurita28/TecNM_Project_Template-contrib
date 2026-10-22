# Arranque Docker TecNM

## Contrato

- Un Compose; servicios `frontend`, `backend` y `db`. Contenedores `${COMPOSE_PROJECT_NAME}-frontend`, `-backend`, `-postgres`; nombre predeterminado `tecnm-identity`.
- Frontend: Vue compilado con Node dentro de Docker, servido con Nginx HTTPS. `/api` se reenvía al backend. Reconstruir al cambiar fuentes; no requiere Node ni .NET en el host.
- Backend: compilación .NET 10 en Docker y runtime separado, HTTPS. Conserva cookies Secure.
- PostgreSQL: sin `ports`; solo comparte red `internal: true` con backend. Frontend comparte otra red con backend y no accede a la red de datos.
- Volumen existente `identity_data` conservado. DDL/semilla al crear volumen vacío; repetición no borra datos.
- Puertos publicados solo en 127.0.0.1: frontend 5173 y backend 7001 predeterminados; configurables por flags y `.env`. PostgreSQL usa 5432 interno y no necesita puerto de host.
- Certificado local autofirmado y secretos generados fuera de Git. Contextos de build excluyen secretos, artefactos y dependencias locales.

## Script

`bash scripts/setup-dev.sh [--frontend-port N] [--backend-port N] [--project-name tecnm-nombre] [--check-ports]`.

- Requiere Docker Compose, Python 3 y OpenSSL. Valida herramientas/daemon, nombres, puertos 1–65535 y puertos distintos.
- Verifica listeners IPv4/IPv6 antes de construir o cambiar contenedores. Permite reutilizar el puerto de un contenedor propio del mismo servicio.
- Puerto ocupado: identificar servicio/puerto y proponer otro. En terminal interactiva permite introducir alternativa; sin terminal termina con aviso controlado y código 2. No mata procesos ni cambia puertos silenciosamente.
- `--check-ports` verifica sin crear configuración, certificados ni contenedores.
- Guarda configuración elegida en `.env`, preservando secretos existentes y ajustes ajenos.
- Construye imágenes, repite comprobación de puertos y levanta en orden por healthchecks. Inicializa esquema/semilla desde contenedor, sin conexión PostgreSQL publicada.
- Error Docker: mensaje controlado y comando para consultar logs. Carrera de puerto tras la comprobación: diagnosticar nuevamente y avisar.
- Resumen final: nombres y URLs reales. Reinicio idempotente; no duplica servicios ni sustituye datos.

## Aceptación

1. Arrancar todo con un comando; tres contenedores saludables.
2. Iniciar sesión y consultar perfil mediante frontend/proxy y directamente en API.
3. Elegir otros puertos y comprobarlos. Repetir el arranque con contenedores propios activos.
4. Un socket ajeno ocupado genera aviso antes de invocar build/up. Cubrir IPv4, IPv6, puertos inválidos y duplicados.
5. PostgreSQL no publica puerto; red interna solo tiene backend y db. Persistencia tras recrear contenedores.
