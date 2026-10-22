# Auth: estructura de integración

Contrato compartido: [spec raíz](../../../../specs/auth.spec.md).

## Alcance

`Modules/Auth` reserva la integración con la API del servicio de login TecNM. Actualmente solo contiene especificación y [guía de integración](../Integration/README.md).

- No registrar servicios de autenticación ni middleware de validación provisional.
- No crear cuentas, hashes, JWT, cookies, endpoints ni identidades de prueba.
- No depender del esquema de base de datos de otro servicio.
- Implementar cliente HTTP, DTOs y validación cuando exista el contrato oficial; mantener controllers delgados y secretos fuera de Git.
- Cada microservicio define sus permisos después de validar la identidad mediante el mecanismo oficial.

## Aceptación

Backend compila y sirve `/api/health` sin autenticarse, sin secreto JWT y sin tablas de identidad. Las antiguas rutas Auth y People no están disponibles.
