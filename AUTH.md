# Integración con el servicio de login TecNM

El template no autentica. `backend/Modules/Auth/` reserva la estructura para conectar en el futuro la API entregada por el servicio de login del TecNM.

## Estado incluido

- Especificación compartida y guía de integración en backend.
- Sin login local, usuarios, hashes, roles, JWT, cookies ni endpoints de sesión.
- Frontend público, sin formularios, guards ni redirecciones de autenticación.
- El arranque solo requiere la conexión PostgreSQL y configuración técnica del servicio.

## Trabajo de cada microservicio

1. Obtener la documentación oficial de la API TecNM: ambientes, protocolo, endpoints, solicitudes, respuestas, errores y requisitos de acceso.
2. Actualizar [spec compartida](specs/auth.spec.md) y [spec backend](backend/Modules/Auth/specs/auth.spec.md) con el contrato real.
3. Implementar el adaptador en `backend/Modules/Auth/Integration/`. Mantener credenciales y configuración sensible fuera de Git.
4. Registrar cliente y middleware en `Program.cs` según ese contrato. Validar identidad antes de autorizar operaciones del microservicio.
5. Definir permisos del dominio y probar identidad inválida, expiración, indisponibilidad del proveedor y acceso denegado.
6. Incorporar el flujo frontend únicamente cuando estén definidos sus requisitos.

El mecanismo de sesión y transporte depende del contrato oficial. No se establece por anticipado el uso de JWT, cookies, OIDC, claims ni duración de sesión. No implementar una autenticación local de respaldo ni simular sesiones válidas.

Guía de estructura: [integración backend](backend/Modules/Auth/Integration/README.md).
