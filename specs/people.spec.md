# Padrón institucional

## Alcance

Fuente única de identidad para alumnos, docentes y administrativos. PostgreSQL es la base canónica; `scriptAlumnosTEC.sql` permanece como evidencia histórica SQL Server. No ejecutarlo sobre el sistema nuevo.

## Modelo

- `people`: UUID permanente, nombres separados, apellidos opcionales, correo institucional único, correo personal, nacimiento, sexo, nacionalidad, CURP/RFC opcionales únicos, teléfonos, domicilio y contacto de emergencia. Campos desconocidos usan NULL.
- `users`: cuenta opcional por persona, usuario único, hash, permiso `Admin`/`User`, cuenta habilitada y creación. Sin duplicar nombre/correo.
- `students`: una ficha por persona; PK/FK `person_id`, matrícula única, carrera, ingreso (año/periodo), generación, modalidad, plan y especialidad opcionales, situación e inscripción.
- `employees`: una ficha por persona; PK/FK `person_id`, número de empleado único, departamento, puesto, fecha de ingreso, vigencia, indicadores independientes docente/administrativo, título y cédula opcionales.
- `programs`, `departments`: catálogos mínimos, clave única, nombre y vigencia.
- Una persona puede tener ambas fichas. El tipo institucional no concede permisos de administrador del sistema.
- UUID de persona identifica relaciones externas. `students.person_id` también es el identificador del alumno. `users.id` identifica la cuenta.
- Nuevas cuentas, catálogos y empleados tienen vigencia predeterminada true; un false explícito se conserva al guardar con EF.
- No borrar en cascada identidades ni relaciones. Usar vigencia; no reciclar UUID, matrícula ni número de empleado.
- Inscripción solo puede ser true con situación `Active`. Situaciones: `Active`, `TemporaryLeave`, `Withdrawn`, `Graduated`.
- Inscrito y cuenta habilitada son independientes: una baja puede consultar su perfil.

## Responsabilidad externa

Horarios, grupos, materias, calificaciones, kárdex, créditos, asistencia, pagos, becas, documentos, salud, tutorías, formación detallada y trámites pertenecen a servicios externos. Este padrón conserva solo la situación vigente, sin historial de operaciones. El servicio propietario deberá actualizar la ficha mediante un flujo autorizado futuro; no se implementa captura ni sincronización en esta entrega.

## Consulta

`GET /api/people/{personId}`: sesión activa requerida. Devuelve únicamente identidad institucional: personId, nombre, correo institucional, matrícula/carrera/ingreso/situación, empleo/departamento/puesto/vigencia. No incluye CURP, RFC, nacimiento, domicilio, teléfonos, correo personal ni credenciales.

`GET /api/people?studentNumber=...`, `?employeeNumber=...` o `?username=...`: exactamente un filtro no vacío; búsqueda exacta normalizada. Misma respuesta. Sin listados generales.

200 ficha; 400 filtro inválido; 401 sin sesión/cuenta deshabilitada; 404 sin persona. Personas sin cuenta también son consultables por UUID o identificador institucional.

Esta versión usa cookies de sesión existentes. No es todavía un protocolo SSO ni autenticación máquina a máquina entre dominios.

## Aceptación

- Login y `/me` muestran todos los campos propios disponibles, fichas alumno/personal y catálogos relacionados.
- `/me` consulta PostgreSQL en cada petición; cambios de datos/vigencia se reflejan con la misma sesión.
- Las consultas institucionales permiten relacionar servicios sin copiar datos personales.
- Semillas ficticias: alumno inscrito, alumno sin inscripción, docente, administrativo, administrador del sistema y persona alumno/docente/administrativo.
- Contraseñas solo hash. Semillas separadas del DDL y exclusivas de desarrollo; repetición no sobrescribe registros existentes.
- Verificar claves únicas, FK, perfiles simultáneos, baja consultable, sesión deshabilitada, privacidad y logout.
