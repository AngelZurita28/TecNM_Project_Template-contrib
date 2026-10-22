-- Canonical PostgreSQL identity schema. Apply to an empty database. No legacy import.

BEGIN;

CREATE TABLE people (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    first_name varchar(100) NOT NULL,
    last_name varchar(100),
    second_last_name varchar(100),
    institutional_email varchar(254) NOT NULL,
    personal_email varchar(254),
    birth_date date,
    birth_place varchar(160),
    sex varchar(30),
    nationality varchar(80),
    curp varchar(18),
    rfc varchar(13),
    phone varchar(30),
    mobile_phone varchar(30),
    street varchar(120),
    address_number varchar(30),
    neighborhood varchar(100),
    postal_code varchar(10),
    city varchar(100),
    state varchar(100),
    country varchar(80),
    emergency_contact_name varchar(160),
    emergency_contact_phone varchar(30),
    emergency_contact_relationship varchar(60),
    CONSTRAINT uq_people_institutional_email UNIQUE (institutional_email),
    CONSTRAINT uq_people_curp UNIQUE (curp),
    CONSTRAINT uq_people_rfc UNIQUE (rfc),
    CONSTRAINT pk_people PRIMARY KEY (id),
    CONSTRAINT ck_people_email CHECK (institutional_email = lower(btrim(institutional_email)) AND institutional_email ~ '^[^[:space:]@]+@[^[:space:]@]+[.][^[:space:]@]+$'),
    CONSTRAINT ck_people_name CHECK (length(btrim(first_name)) > 0),
    CONSTRAINT ck_people_curp CHECK (curp IS NULL OR curp ~ '^[A-Z0-9]{18}$'),
    CONSTRAINT ck_people_rfc CHECK (rfc IS NULL OR rfc ~ '^[A-ZÑ&]{3,4}[0-9]{6}[A-Z0-9]{3}$')
);

CREATE TABLE programs (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    code varchar(30) NOT NULL,
    name varchar(160) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT uq_programs_code UNIQUE (code),
    CONSTRAINT pk_programs PRIMARY KEY (id),
    CONSTRAINT ck_programs_code CHECK (length(btrim(code)) > 0)
);

CREATE TABLE departments (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    code varchar(30) NOT NULL,
    name varchar(160) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT uq_departments_code UNIQUE (code),
    CONSTRAINT pk_departments PRIMARY KEY (id),
    CONSTRAINT ck_departments_code CHECK (length(btrim(code)) > 0)
);

CREATE TABLE students (
    person_id uuid NOT NULL,
    student_number varchar(20) NOT NULL,
    program_id uuid NOT NULL,
    admission_year integer NOT NULL,
    admission_term varchar(40),
    generation varchar(30),
    modality varchar(40),
    curriculum_code varchar(50),
    specialty varchar(160),
    status varchar(20) NOT NULL,
    is_enrolled boolean NOT NULL,
    CONSTRAINT uq_students_student_number UNIQUE (student_number),
    CONSTRAINT pk_students PRIMARY KEY (person_id),
    CONSTRAINT ck_students_student_number CHECK (student_number = upper(btrim(student_number)) AND length(student_number) > 0),
    CONSTRAINT ck_students_admission_year CHECK (admission_year BETWEEN 1900 AND 2200),
    CONSTRAINT ck_students_status CHECK (status IN ('Active', 'TemporaryLeave', 'Withdrawn', 'Graduated')),
    CONSTRAINT ck_students_enrollment CHECK (NOT is_enrolled OR status = 'Active'),
    CONSTRAINT fk_students_people FOREIGN KEY (person_id) REFERENCES people(id) ON DELETE RESTRICT,
    CONSTRAINT fk_students_programs FOREIGN KEY (program_id) REFERENCES programs(id) ON DELETE RESTRICT
);

CREATE INDEX ix_students_program_id ON students (program_id);

CREATE TABLE employees (
    person_id uuid NOT NULL,
    employee_number varchar(20) NOT NULL,
    department_id uuid NOT NULL,
    job_title varchar(160) NOT NULL,
    hire_date date,
    is_active boolean NOT NULL DEFAULT true,
    is_teacher boolean NOT NULL,
    is_administrative boolean NOT NULL,
    degree varchar(160),
    professional_license varchar(30),
    CONSTRAINT uq_employees_employee_number UNIQUE (employee_number),
    CONSTRAINT pk_employees PRIMARY KEY (person_id),
    CONSTRAINT ck_employees_employee_number CHECK (employee_number = upper(btrim(employee_number)) AND length(employee_number) > 0),
    CONSTRAINT ck_employees_type CHECK (is_teacher OR is_administrative),
    CONSTRAINT fk_employees_people FOREIGN KEY (person_id) REFERENCES people(id) ON DELETE RESTRICT,
    CONSTRAINT fk_employees_departments FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE RESTRICT
);

CREATE INDEX ix_employees_department_id ON employees (department_id);

CREATE TABLE users (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    person_id uuid NOT NULL,
    username varchar(64) NOT NULL,
    password varchar(512) NOT NULL,
    role varchar(16) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_users_username UNIQUE (username),
    CONSTRAINT uq_users_person_id UNIQUE (person_id),
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT ck_users_role CHECK (role IN ('Admin', 'User')),
    CONSTRAINT ck_users_username CHECK (username = lower(btrim(username)) AND length(username) > 0),
    CONSTRAINT fk_users_people FOREIGN KEY (person_id) REFERENCES people(id) ON DELETE RESTRICT
);

COMMIT;
