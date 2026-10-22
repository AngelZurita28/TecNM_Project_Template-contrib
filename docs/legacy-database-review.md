# Revisión de la base histórica

## Resultado

Se sustituyen 85 tablas por seis tablas de padrón. El SQL Server original permanece intacto. `database.sql` define PostgreSQL nuevo; no es una migración automática ni importa expedientes reales.

Revisión estática del archivo completo, decodificado UTF-16 LE. Se inventariaron tablas, columnas, PK/FK, vistas y procedimientos. El archivo no contiene un volcado de filas de personas. No permite comprobar duplicados reales, catálogos ni calidad de datos productivos.

| Objeto | Histórico | Padrón nuevo |
| --- | ---: | ---: |
| Tablas | 85 | 6 |
| Vistas | 22 | 0 |
| Procedimientos | 30 | 0 |
| Llaves foráneas declaradas | 3 | 5 |
| Tablas sin PK declarada | 30 | 0 |

## Hallazgos y decisiones

1. `Alumnos` mezcla matrícula, identidad, situación, créditos, horarios indirectos y certificación. Se conservan identidad, carrera, ingreso, situación e inscripción; se separan servicios transaccionales.
2. `Alumnos.Apellidos`, `ApellidoP`, `ApellidoM` compiten. `UpdateAlumnos` actualiza el nombre combinado, pero no los dos apellidos separados. No es seguro dividir por espacios: apellidos compuestos requieren revisión.
3. `Alumnos.correo` admite 50 caracteres, `AlumnoDetalle.Email` 80 y `Docentes.Correo` 100. No hay correo institucional único. El nuevo modelo separa institucional/personal, permite 254 caracteres y exige unicidad institucional normalizada.
4. Matrícula tiene 12 caracteres en `Alumnos`, 15 en `AlumnoDetalle`; no existe FK entre ellas. Otras relaciones de alumnos y cuentas también dependen de convenciones. Se adoptan UUID y FK reales; matrícula y empleado siguen únicos.
5. Años son `varchar(4)` y varios campos desconocidos son obligatorios. Se usa año entero, fechas civiles y NULL para desconocidos. `Inscrito` se mantiene separado de situación y habilitación de cuenta.
6. `Docentes.Pass`, `Alumnos.Clave`, `Usuarios.Clave` y `CorreosClaves.Clave` son cadenas sin contrato de hash acreditado. El esquema no demuestra texto plano ni hash seguro. No se reutilizan contraseñas antiguas. Cuentas nuevas usan formato Identity V3 y verificación `PasswordHasher<User>`.
7. `Usuarios.Tipo`, `Docentes.Tipo` y `MenuPermisos` no definen una identidad común. No inferir administrativo desde permiso `Admin`. Fichas laborales y permisos de cuenta son independientes; una persona puede estudiar y trabajar.
8. `sp_capturaCalificacion` concatena identificadores y valores directamente en SQL dinámico, sin parámetros de valores ni lista permitida visible para columnas. `sp_reporteTutor` también arma SQL dinámico. No se portan esos procedimientos.
9. Varios CATCH solo imprimen errores y tienen rollback comentado. `UpdateAlumnos`, `UpdateFotoA` y `UpdateFotoD` devuelven `@@Identity` después de UPDATE. No son contratos fiables para nuevas APIs.
10. `vTodosDatos` exige `EstatusID='VI'` e `Inscrito=1`; `vTodosDatosNO` exige `BT` y no inscrito. Otras vistas usan INNER JOIN con detalles/catálogos y pueden omitir personas incompletas. El padrón consulta identidad sin exigir inscripción ni una ficha de cada tipo.
11. `DocentesDP`/`AlumnoDetalle` mezclan información de salud, NSS, discapacidad y familia extensa. Se retienen contacto personal y emergencia; datos clínicos, familiares extensos y socioeconómicos quedan fuera de esta base mínima.
12. Fotos `image`, documentos binarios y campos genéricos de notas no se copian al padrón. Servicios de expediente deben controlar archivos y permisos.

## Mapeo de campos conservados

| Origen | Destino | Regla |
| --- | --- | --- |
| Alumnos.Nombre / Docentes.Nombre / Usuarios.Nombre | people.first_name + apellidos | Priorizar campos separados verificados; nombres administrativos requieren revisión manual. |
| Alumnos.ApellidoP / ApellidoM / Apellidos | people.last_name / second_last_name | No separar cadenas compuestas automáticamente. |
| Alumnos.Matricula | students.student_number | Trim/mayúsculas; única; UUID nuevo independiente. |
| Alumnos.CarreraID + Carreras | students.program_id + programs | Resolver catálogo; no cargar FK huérfana. |
| Alumnos.AnoIngreso / PeriodoIngreso / Generacion | admission_year / admission_term / generation | Validar año; resolver descripción del periodo. |
| Alumnos.Modalidad / PlanEstID / EspecialidadID | modality / curriculum_code / specialty | Solo identificación de asignación, sin currículo ni materias. |
| Alumnos.EstatusID / Inscrito | status / is_enrolled | VI y BT aparecen en vistas; confirmar todos los códigos antes de convertir. |
| AlumnoDetalle.Email / Alumnos.correo / Docentes.Correo | institutional_email / personal_email | Determinar tipo de correo y resolver conflictos con fuente autoritativa. No inventar correos. |
| AlumnoDetalle.CURP/RFC, Docentes.CURP/RFC | people.curp / rfc | Vacíos a NULL; mayúsculas; resolver duplicados antes de importar. |
| AlumnoDetalle y DocentesDP nacimiento/contacto/domicilio | people.* | Resolver catálogos a nombres; fechas sin hora; sin fechas centinela. |
| AlumnoDetalle.Emergencia / Tutor / TelefonoTutor | emergency_contact_* | Validar contacto autorizado; no asumir que tutor siempre es contacto de emergencia. |
| Docentes.Clave / FechaIngreso / Vigente | employees.employee_number / hire_date / is_active | No asumir que Clave y UsuarioID identifican personas distintas. |
| Docentes.CveDepto / Usuarios.CveDepto | employees.department_id | Resolver departamento. |
| Docentes.Titulo / Cedula | employees.degree / professional_license | Identificación profesional básica; otras cédulas/formación quedan en expediente. |
| Usuarios.Usuario / Activo / Puesto | users.username / is_active; employees.job_title | Cuenta vinculada a persona verificada; permiso concedido explícitamente. |

## Inventario de las 85 tablas

«Fuera» significa propiedad de otro servicio o utilidad histórica; no se elimina información de la base original.

| Tabla histórica | Decisión |
| --- | --- |
| `CreacionGrupos` | Fuera: Oferta, horarios y asistencia. |
| `PlanEstudios` | Solo clave vigente en `students.curriculum_code`; estructura curricular fuera. |
| `Carreras` | `programs`: clave, nombre, vigencia; sin contadores ni carga. |
| `ListaGrupos` | Fuera: Calificaciones y trayectoria académica. |
| `Alumnos` | `students` y nombres en `people`; separar identidad y situación. |
| `Asignaturas` | Fuera: Oferta, horarios y asistencia. |
| `Docentes` | `people` + `employees`; cuenta nueva en `users`, sin importar Pass. |
| `Fichas` | Fuera: Admisión. |
| `EvaluacionDocente` | Fuera: Trámites, vinculación y documentos. |
| `HorariosDocentes` | Fuera: Oferta, horarios y asistencia. |
| `InscritoHistorial` | Fuera: Calificaciones y trayectoria académica. |
| `Municipios` | Resolver nombre hacia domicilio; no conservar catálogo completo. |
| `Periodos` | Solo descripción del ingreso en `students.admission_term`; periodo vigente fuera. |
| `ColoniasCP` | Resolver colonia/CP; no conservar catálogo postal completo. |
| `Turnos` | Fuera: Oferta, horarios y asistencia. |
| `Escuelas` | Fuera: Admisión. |
| `Especialidades` | Solo nombre asignado en `students.specialty`; oferta curricular fuera. |
| `Estados` | Resolver nombre hacia domicilio/nacimiento; no conservar catálogo completo. |
| `AlumnoDetalle` | `people`: contacto, nacimiento, CURP/RFC, domicilio y emergencia; excluir salud y estudio socioeconómico. |
| `Kardex` | Fuera: Calificaciones y trayectoria académica. |
| `Categorias` | Fuera: Expediente y gestión del personal. |
| `AccesosLog` | Fuera: Configuración, accesos o auxiliares heredados. |
| `ActividadesComplementarias` | Fuera: Trámites, vinculación y documentos. |
| `ActividadesDocentes` | Fuera: Expediente y gestión del personal. |
| `Adeudos` | Fuera: Pagos, becas y adeudos. |
| `AsignaturaDetalle` | Fuera: Oferta, horarios y asistencia. |
| `AsignaturaEspecialidad` | Fuera: Oferta, horarios y asistencia. |
| `AsignaturasD` | Fuera: Oferta, horarios y asistencia. |
| `AsistenciaDocentes` | Fuera: Expediente y gestión del personal. |
| `Aulas` | Fuera: Oferta, horarios y asistencia. |
| `Bajas` | Fuera: Trámites, vinculación y documentos. |
| `BajasRep` | Fuera: Trámites, vinculación y documentos. |
| `BecasInscripcion` | Fuera: Pagos, becas y adeudos. |
| `BecasInscripcionNI` | Fuera: Pagos, becas y adeudos. |
| `CalendarioEvaluacion` | Fuera: Oferta, horarios y asistencia. |
| `Colonias` | Resolver nombre hacia domicilio; no conservar catálogo completo. |
| `Competencias` | Fuera: Oferta, horarios y asistencia. |
| `Configuracion` | Fuera: Configuración, accesos o auxiliares heredados. |
| `CorreosClaves` | Fuera: Configuración, accesos o auxiliares heredados. |
| `CursoEspecial` | Fuera: Calificaciones y trayectoria académica. |
| `Departamentos` | `departments`: clave, nombre, vigencia; puesto en ficha del empleado. |
| `DisponibilidadMaterias` | Fuera: Oferta, horarios y asistencia. |
| `dkar` | Fuera: Calificaciones y trayectoria académica. |
| `Docente` | Fuera: Expediente y gestión del personal. |
| `DocentesActividades` | Fuera: Expediente y gestión del personal. |
| `DocentesDP` | `people`: nacimiento, domicilio; excluir NSS, hijos y estado civil. |
| `DocentesExtra` | Fuera: Expediente y gestión del personal. |
| `DocentesFormacion` | Fuera: Expediente y gestión del personal. |
| `DocentesNotas` | Fuera: Expediente y gestión del personal. |
| `DocentesOtros` | Fuera: Expediente y gestión del personal. |
| `DocentesTitulos` | Fuera: Expediente y gestión del personal. |
| `Documentacion` | Fuera: Trámites, vinculación y documentos. |
| `Edificios` | Fuera: Oferta, horarios y asistencia. |
| `Empresas` | Fuera: Trámites, vinculación y documentos. |
| `enec` | Fuera: Oferta, horarios y asistencia. |
| `Estatus` | Normalizar a `students.status` con mapeo verificado; no copiar códigos sin catálogo. |
| `FichaCanalizacion` | Fuera: Tutoría y canalizaciones. |
| `FichaContraReferencia` | Fuera: Tutoría y canalizaciones. |
| `Files` | Fuera: Trámites, vinculación y documentos. |
| `FilesInscripciones` | Fuera: Pagos, becas y adeudos. |
| `GruposExamenAdmision` | Fuera: Admisión. |
| `GruposRegulares` | Fuera: Oferta, horarios y asistencia. |
| `Ingles` | Fuera: Calificaciones y trayectoria académica. |
| `Instrumentacion` | Fuera: Oferta, horarios y asistencia. |
| `IntrumentacionAna` | Fuera: Oferta, horarios y asistencia. |
| `ITSMLG` | Fuera: Calificaciones y trayectoria académica. |
| `Menu` | Fuera: Configuración, accesos o auxiliares heredados. |
| `MenuPermisos` | Fuera: Configuración, accesos o auxiliares heredados. |
| `NivelCarreras` | Fuera: Configuración, accesos o auxiliares heredados. |
| `Oportunidades` | Fuera: Calificaciones y trayectoria académica. |
| `OportunidadesC` | Fuera: Calificaciones y trayectoria académica. |
| `Paso` | Fuera: Configuración, accesos o auxiliares heredados. |
| `Paso2` | Fuera: Configuración, accesos o auxiliares heredados. |
| `Pasos` | Fuera: Configuración, accesos o auxiliares heredados. |
| `PeriodoVigente` | Fuera: Oferta, horarios y asistencia. |
| `Posiciones` | Fuera: Oferta, horarios y asistencia. |
| `RegistroCertificados` | Fuera: Trámites, vinculación y documentos. |
| `Residencias` | Fuera: Trámites, vinculación y documentos. |
| `Scaneados` | Fuera: Trámites, vinculación y documentos. |
| `ServicioSocial` | Fuera: Trámites, vinculación y documentos. |
| `ServicioSocialDoc` | Fuera: Trámites, vinculación y documentos. |
| `Titulacion` | Fuera: Trámites, vinculación y documentos. |
| `TurnoH` | Fuera: Oferta, horarios y asistencia. |
| `Usuarios` | `users` + `people` + `employees` cuando exista adscripción administrativa verificada. |
| `verano` | Fuera: Configuración, accesos o auxiliares heredados. |

## Objetos restantes

Vistas: `vAlumnosPorDocente`, `vAnoFichas`, `vAnosCG`, `vCarrerasEvaluacion`, `vConHorarios`, `vFichasAno`, `vGruposCal`, `vGruposCal2`, `vLara`, `vMateriasFrenteGrupos`, `vMateriasGrupos`, `vPorAnos`, `vTodosDatosPA`, `vPorCarreraPeriodo`, `vPorEdadInscritos`, `vTodosDatos`, `vTodosDatosNO`, `vSUBES`, `vCategorias`, `vGruposBases`, `vPorEstadoPrepa`, `vInscritos`.

Procedimientos: `ActualizaAlumnoDetalla`, `ActualizaCalificaciones`, `ActualizaLG`, `ActualizaMateriaKardex`, `EliminaHorario`, `InsertaAlumnoDetalle`, `InsertaAsistencia`, `InsertaHorario`, `InsertaInscritos`, `InsertAlumnos`, `InsertaMateriaKardex`, `InsertCreacionGrupos`, `InsertDocentes`, `InsertDocentesDP`, `InsertFicha`, `InsertFichaWeb`, `sp_capturaCalificacion`, `sp_reporteTutor`, `spGetColonias`, `spGetColoniasN`, `spGetEstados`, `spGetMunicipios`, `UpdateAlumnos`, `UpdateCreacionGrupos`, `UpdateDocentes`, `UpdateDocentesDP`, `UpdateDocumentacionNI`, `UpdateFicha`, `UpdateFotoA`, `UpdateFotoD`.

No se portan vistas ni procedimientos: combinan dominios externos o implementan CRUD sustituido por el nuevo modelo.

Tablas sin PK declarada: `EvaluacionDocente`, `HorariosDocentes`, `Municipios`, `AccesosLog`, `ActividadesComplementarias`, `ActividadesDocentes`, `AsignaturasD`, `AsistenciaDocentes`, `Aulas`, `BajasRep`, `Colonias`, `Configuracion`, `DisponibilidadMaterias`, `dkar`, `DocentesActividades`, `DocentesExtra`, `DocentesNotas`, `DocentesTitulos`, `Documentacion`, `enec`, `Files`, `Ingles`, `IntrumentacionAna`, `ITSMLG`, `MenuPermisos`, `Oportunidades`, `OportunidadesC`, `Pasos`, `Posiciones`, `Scaneados`.

## Importación real pendiente de datos

1. Extraer personas y catálogos del sistema autorizado, sin ejecutar DDL antiguo en PostgreSQL.
2. Resolver homónimos y cruces alumno/docente/usuario mediante evidencia; CURP sirve como candidato, no como deduplicación ciega.
3. Confirmar apellidos, tipo de correo, códigos de estatus, periodo de ingreso, matrícula y número laboral. Emitir reporte de conflictos.
4. Asignar UUID permanentes y conservar correspondencia con llaves antiguas en un archivo de migración controlado.
5. Importar solo campos acordados en una transacción, validar totales, duplicados y huérfanos. Aprovisionar credenciales nuevas.

No se inventó una migración de registros que no vienen en el archivo. La entrega deja modelo, datos ficticios y consulta funcional.

SHA-256 del archivo histórico: `8b9f62ec74002c329ad625d15117303a01b336302fcd84fbf3de0b54334772bc`.
