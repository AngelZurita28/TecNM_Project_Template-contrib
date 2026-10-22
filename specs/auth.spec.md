# Integración futura de autenticación

Fuente canónica: [AUTH.md](../AUTH.md). El template reserva la integración únicamente en backend.

## Estado actual

- No autentica usuarios ni expone endpoints de login, sesión o logout.
- No valida contraseñas, emite tokens, crea cookies ni mantiene cuentas locales.
- No define roles, claims, tablas de identidad ni DTOs del proveedor.
- El frontend es público y no contiene formularios ni guards de autenticación.
- `backend/Modules/Auth/Integration/` contiene la guía para conectar la API del servicio de login TecNM.

## Implementación futura

1. Obtener el contrato oficial de la API del servicio de login TecNM.
2. Documentar protocolo, endpoints, configuración, validación de identidad, errores y ciclo de sesión.
3. Implementar el cliente y la validación en backend; configurar secretos fuera de Git.
4. Definir autorización por microservicio y probar acceso permitido y rechazado antes de publicar rutas protegidas.
5. Incorporar flujo frontend solo si el contrato oficial lo requiere.

No asumir JWT, cookies, duración de sesión, proveedores federados ni rutas de API sin ese contrato. No simular una identidad autenticada mientras la integración esté pendiente.

## Aceptación

- `/api/auth/login`, `/api/auth/me` y `/api/auth/logout` no existen.
- El arranque no requiere secretos JWT ni consulta tablas de usuarios.
- La página inicial abre sin sesión ni redirección al login.
- Conectar el servicio TecNM es responsabilidad de cada proyecto.
