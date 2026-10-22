-- DEVELOPMENT ONLY. Fictional data; shared password: TecNM-Demo-2026!
-- Re-running never resets existing profiles or passwords.
BEGIN;

INSERT INTO programs (id, code, name, is_active) VALUES
    ('30000000-0000-4000-8000-000000000001', 'ISC', 'Ingeniería en Sistemas Computacionales', true)
ON CONFLICT DO NOTHING;

INSERT INTO departments (id, code, name, is_active) VALUES
    ('40000000-0000-4000-8000-000000000001', 'SISTEMAS', 'Sistemas y Computación', true),
    ('40000000-0000-4000-8000-000000000002', 'ESCOLARES', 'Servicios Escolares', true)
ON CONFLICT DO NOTHING;

INSERT INTO people (id, first_name, last_name, second_last_name, institutional_email, personal_email, birth_date, birth_place, sex, nationality, curp, rfc, phone, mobile_phone, street, address_number, neighborhood, postal_code, city, state, country, emergency_contact_name, emergency_contact_phone, emergency_contact_relationship) VALUES
    ('20000000-0000-4000-8000-000000000001', 'Ana Sofía', 'López', 'García', 'alumno@tecnm.example', 'alumno@correo.example', '2005-03-15', 'Monclova, Coahuila', 'Mujer', 'Mexicana', 'XEXX050315HNEXXX01', 'XEXX050315001', '000-000-0000', '000-000-0001', 'Calle de Prueba', '10', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', 'Contacto Ficticio 1', '000-000-0091', 'Familiar'),
    ('20000000-0000-4000-8000-000000000002', 'Diego', 'Martínez', 'Ruiz', 'alumno.baja@tecnm.example', NULL, '2003-11-02', 'Monclova, Coahuila', 'Hombre', 'Mexicana', NULL, NULL, NULL, '000-000-0002', 'Calle de Prueba', '20', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', NULL, NULL, NULL),
    ('20000000-0000-4000-8000-000000000003', 'Laura', 'Hernández', 'Torres', 'docente@tecnm.example', 'docente@correo.example', '1982-06-20', 'Monclova, Coahuila', 'Mujer', 'Mexicana', 'XEXX820620HNEXXX03', 'XEXX820620003', '000-000-0000', '000-000-0003', 'Calle de Prueba', '30', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', 'Contacto Ficticio 3', '000-000-0093', 'Familiar'),
    ('20000000-0000-4000-8000-000000000004', 'Carlos', 'Sánchez', 'Díaz', 'administrativo@tecnm.example', 'administrativo@correo.example', '1988-02-12', 'Monclova, Coahuila', 'Hombre', 'Mexicana', 'XEXX880212HNEXXX04', 'XEXX880212004', '000-000-0000', '000-000-0004', 'Calle de Prueba', '40', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', 'Contacto Ficticio 4', '000-000-0094', 'Familiar'),
    ('20000000-0000-4000-8000-000000000005', 'Elena', 'Ramírez', 'Flores', 'admin@tecnm.example', 'admin@correo.example', '1985-08-09', 'Monclova, Coahuila', 'Mujer', 'Mexicana', 'XEXX850809HNEXXX05', 'XEXX850809005', '000-000-0000', '000-000-0005', 'Calle de Prueba', '50', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', 'Contacto Ficticio 5', '000-000-0095', 'Familiar'),
    ('20000000-0000-4000-8000-000000000006', 'Alex', 'Moreno', 'Vega', 'mixto@tecnm.example', 'mixto@correo.example', '1996-01-25', 'Monclova, Coahuila', 'No especificado', 'Mexicana', 'XEXX960125HNEXXX06', 'XEXX960125006', '000-000-0000', '000-000-0006', 'Calle de Prueba', '60', 'Colonia Demostración', '25700', 'Monclova', 'Coahuila', 'México', 'Contacto Ficticio 6', '000-000-0096', 'Familiar')
ON CONFLICT DO NOTHING;

INSERT INTO students (person_id, student_number, program_id, admission_year, admission_term, generation, modality, curriculum_code, specialty, status, is_enrolled) VALUES
    ('20000000-0000-4000-8000-000000000001', 'M20260001', '30000000-0000-4000-8000-000000000001', 2026, 'Agosto–diciembre', '2026', 'Escolarizada', 'ISIC-2010-224', NULL, 'Active', true),
    ('20000000-0000-4000-8000-000000000002', 'M20220002', '30000000-0000-4000-8000-000000000001', 2022, 'Agosto–diciembre', '2022', 'Escolarizada', 'ISIC-2010-224', NULL, 'TemporaryLeave', false),
    ('20000000-0000-4000-8000-000000000006', 'M20240006', '30000000-0000-4000-8000-000000000001', 2024, 'Enero–junio', '2024', 'Mixta', 'ISIC-2010-224', 'Desarrollo de software', 'Active', true)
ON CONFLICT DO NOTHING;

INSERT INTO employees (person_id, employee_number, department_id, job_title, hire_date, is_active, is_teacher, is_administrative, degree, professional_license) VALUES
    ('20000000-0000-4000-8000-000000000003', 'D0003', '40000000-0000-4000-8000-000000000001', 'Docente de tiempo completo', '2010-08-16', true, true, false, 'Maestría en Ciencias de la Computación', 'DEMO-0003'),
    ('20000000-0000-4000-8000-000000000004', 'A0004', '40000000-0000-4000-8000-000000000002', 'Auxiliar de servicios escolares', '2018-02-01', true, false, true, NULL, NULL),
    ('20000000-0000-4000-8000-000000000005', 'A0005', '40000000-0000-4000-8000-000000000002', 'Responsable del padrón institucional', '2015-01-12', true, false, true, NULL, NULL),
    ('20000000-0000-4000-8000-000000000006', 'D0006', '40000000-0000-4000-8000-000000000001', 'Docente y apoyo administrativo', '2021-08-16', true, true, true, 'Ingeniería en Sistemas Computacionales', 'DEMO-0006')
ON CONFLICT DO NOTHING;

INSERT INTO users (id, person_id, username, password, role, is_active) VALUES
    ('10000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000001', 'alumno', 'AQAAAAIAAYagAAAAEID6KV+P5WVNvBw5/kaCp8H+q09TUJkrSrDvQ7UWVSjbu7wflhDkA26HJ1aZhQeCfA==', 'User', true),
    ('10000000-0000-4000-8000-000000000002', '20000000-0000-4000-8000-000000000002', 'alumno.baja', 'AQAAAAIAAYagAAAAECP+JNFT8ZN301RTiVE6XqZ1VQagaau0C9tdjuNYpvryraRjOTb5wjI+UxS1+xRj5w==', 'User', true),
    ('10000000-0000-4000-8000-000000000003', '20000000-0000-4000-8000-000000000003', 'docente', 'AQAAAAIAAYagAAAAEFpEn0QyTEs/Aqh2VIweMaDXiG/RJgDAWMurahuTSrRC774QNht478pbOsadRvJxTQ==', 'User', true),
    ('10000000-0000-4000-8000-000000000004', '20000000-0000-4000-8000-000000000004', 'administrativo', 'AQAAAAIAAYagAAAAEFpVMnjeaSBXncsiA6Ymg3/9cmkQBkEokvDKMgjx1+XayL4+7PLHLMyeJyYFf3OgKQ==', 'User', true),
    ('10000000-0000-4000-8000-000000000005', '20000000-0000-4000-8000-000000000005', 'admin', 'AQAAAAIAAYagAAAAENt7vunkBSkIiczP+JnIM0uzj+gzVo9+nqTszZO3cXjzB30wUj5ksjT6YUB4675G7Q==', 'Admin', true),
    ('10000000-0000-4000-8000-000000000006', '20000000-0000-4000-8000-000000000006', 'mixto', 'AQAAAAIAAYagAAAAEDkpjYANBJRyh55THsi9tIFJuGApq/73LXjJDUcmgQmmuljSEtu93bgIZAVeFYUBnw==', 'User', true)
ON CONFLICT DO NOTHING;

COMMIT;
