# Contrato de autenticación local

Fuente canónica para backend y frontend. Cambiar este archivo antes de cambiar código o specs locales.

## Alcance

- Login local contra PostgreSQL.
- Sesión JWT institucional en cookies.
- Consulta de usuario actual.
- Logout idempotente.
- Sin OAuth/OIDC, refresh token, registro ni recuperación de contraseña.

## DTO

```text
LoginRequest
  username: string, requerido, sin espacios externos
  password: string, requerido

UserResponse
  id: UUID
  username: string
  email: string
  name: string
  role: "Admin" | "User" (permiso de cuenta, no tipo de persona)
  isActive: boolean
  createdAt: ISO-8601
  person: PersonResponse (specs/people.spec.md; datos propios completos)
```

Payload nulo, campo ausente o valor vacío: inválido. Backend recorta espacios externos de `username`. Backend no devuelve entidad de persistencia.

## API

| Método y ruta | Acceso | Entrada | Éxito | Error |
| --- | --- | --- | --- | --- |
| `POST /api/auth/login` | Público | `LoginRequest` JSON | `200 UserResponse` y cookies | `400` payload inválido; `401` credenciales incorrectas o usuario inactivo |
| `GET /api/auth/me` | Protegido | Ninguna | `200 UserResponse` desde PostgreSQL | `401` token ausente, vencido, alterado, inválido o cuenta deshabilitada |
| `POST /api/auth/logout` | Público | Ninguna | Siempre `204`; expira ambas cookies | Ninguno por sesión ausente |

Respuesta `401` de login usa mensaje genérico. No confirma si cuenta existe, está inactiva o contraseña falla.

## Sesión

- Duración fija: 8 horas. Sin renovación automática.
- `auth_token`: JWT, `HttpOnly`, `Secure`, `SameSite=Lax`, `Path=/`.
- `session_exp`: timestamp Unix, no `HttpOnly`, `Secure`, `SameSite=Lax`, `Path=/`.
- Ambas cookies comparten expiración.
- Logout elimina ambas cookies con misma ruta y opciones compatibles.
- Respuestas de Auth no se almacenan en caché (`Cache-Control: no-store`).
- Frontend envía credenciales en cada petición. Nunca lee ni guarda `auth_token`.
- `session_exp` solo mejora UX. Backend conserva autoridad.
- Desarrollo usa HTTPS para conservar `Secure=true`.

## JWT

- Firma: HMAC-SHA256.
- Claims requeridos: `sub`, `username`, `email`, `name`, `role`, `exp`, `iss`, `aud`.
- `sub` contiene `UserResponse.id`.
- `role` solo acepta `Admin` o `User`.
- `person_id` contiene el UUID estable de la persona. Datos personales completos no se incluyen en JWT.
- Cada petición protegida verifica que cuenta y persona sigan vinculadas y cuenta esté activa.
- Backend valida firma, emisor, audiencia y expiración en cada ruta protegida.
- `Jwt__SecretKey` vive fuera de Git. API falla al iniciar si falta o tiene menos de 32 bytes UTF-8.

## Datos demo

- Cuentas de desarrollo: `alumno`, `alumno.baja`, `docente`, `administrativo`, `admin`, `mixto`. Solo `admin` tiene permiso `Admin`.
- Contraseña común exclusiva de desarrollo: `TecNM-Demo-2026!`.
- PostgreSQL guarda solo hashes Identity V3 compatibles con `PasswordHasher<User>`, verificados en las pruebas de login.
- Producción no incluye credenciales demo. DDL y semilla se ejecutan por separado.

## Casos de aceptación

1. Login con payload inválido devuelve `400` y no crea sesión.
2. Login con credenciales incorrectas o usuario inactivo devuelve `401` genérico.
3. Login válido devuelve `200`, `UserResponse` y ambas cookies.
4. `/me` con `auth_token` válido devuelve mismo usuario.
5. `/me` sin token válido devuelve `401`.
6. Logout sin sesión o con sesión devuelve `204` y cookies expiradas.
7. `/me` después de logout devuelve `401`.
8. Frontend redirige a `/login` cuando falta, es inválido o vence `session_exp`.
9. Respuesta `401` protegida limpia estado local y redirige a `/login`.
10. Acciones en cliente y servidor previenen dobles peticiones: interfaz bloquea reentrancia mientras procesa y operaciones mutativas aseguran unicidad de ejecución.
