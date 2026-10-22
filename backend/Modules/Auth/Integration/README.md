# Integración pendiente: login TecNM

Esta carpeta es el punto de extensión para conectar la API del servicio de login TecNM. No contiene cliente ejecutable, contratos inventados ni autenticación provisional.

Cuando se entregue el contrato oficial:

- Crear aquí el cliente HTTP del servicio y sus DTOs de transporte.
- Separar la validación/adaptación de identidad de los permisos propios del microservicio.
- Documentar URL por ambiente, configuración, errores, tiempos de espera y ciclo de sesión.
- Registrar dependencias y middleware en `Program.cs` según el mecanismo oficial.
- Mantener secretos fuera del repositorio y cubrir los casos de aceptación del contrato real.

No crear tablas de usuarios ni validar contraseñas del proveedor en la base del microservicio. Los nombres de métodos, DTOs, endpoints y formato de credenciales se definirán cuando exista la API oficial.

Referencias: [guía raíz](../../../../AUTH.md), [spec compartida](../../../../specs/auth.spec.md) y [spec local](../specs/auth.spec.md).
