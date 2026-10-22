#!/usr/bin/env python3
"""Integration check against the isolated development DB and running HTTPS API."""
import base64
import http.cookiejar
import json
import os
from pathlib import Path
import ssl
import subprocess
import urllib.error
import urllib.request
import uuid

os.chdir(Path(__file__).resolve().parent.parent)
def configured_backend_port():
    path = Path('.env')
    for line in path.read_text().splitlines() if path.exists() else []:
        if line.startswith('TECNM_BACKEND_PORT='):
            return line.split('=', 1)[1].strip().strip("\"'")
    return '7001'


BASE = os.environ.get('API_URL', 'https://localhost:' + configured_backend_port()).rstrip('/')
PASSWORD = 'TecNM-Demo-2026!'
PERSON = '20000000-0000-4000-8000-000000000001'
ACCOUNT = '10000000-0000-4000-8000-000000000001'


def sql(statement):
    result = subprocess.run(['docker', 'compose', 'exec', '-T', 'db', 'psql',
                             '-X', '-v', 'ON_ERROR_STOP=1', '-At', '-U', 'tecnm_app', '-d', 'tecnm_identity'],
                            input=statement, text=True, capture_output=True, check=True)
    return result.stdout.strip()


def quote(value):
    return "'" + str(value).replace("'", "''") + "'"


def client():
    jar = http.cookiejar.CookieJar()
    opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar),
        urllib.request.HTTPSHandler(context=ssl._create_unverified_context()))
    return jar, opener


def request(opener, path, expected=200, body=None, method=None):
    payload = None if body is None else json.dumps(body).encode()
    req = urllib.request.Request(BASE + path, data=payload,
        headers={'Content-Type': 'application/json'}, method=method)
    try:
        response = opener.open(req)
    except urllib.error.HTTPError as error:
        response = error
    raw = response.read()
    assert response.status == expected, (path, expected, response.status, raw.decode())
    if path.startswith('/api/auth/') and expected == 200:
        assert 'no-store' in response.headers.get('Cache-Control', '')
    return json.loads(raw) if raw else None


def login(opener, username, expected=200, password=PASSWORD):
    return request(opener, '/api/auth/login', expected, {'username': username, 'password': password})


jar, api = client()
request(api, '/api/auth/me', 401)
request(api, '/api/people/' + PERSON, 401)
request(api, '/api/people?studentNumber=M20260001', 401)
request(api, '/api/auth/login', 400, {})
request(api, '/api/auth/login', 400, {'username': 'x' * 65, 'password': PASSWORD})
login(api, 'alumno', 401, 'incorrecta')
login(api, 'no-existe', 401)
for username in ['alumno', 'alumno.baja', 'docente', 'administrativo', 'admin', 'mixto']:
    account = login(api, username)
    profile = request(api, '/api/auth/me')
    assert profile == account
    assert profile['role'] == ('Admin' if username == 'admin' else 'User')
    assert 'password' not in json.dumps(profile).lower()
    p = profile['person']
    assert profile['email'] == p['institutionalEmail']
    assert bool(p['student']) == (username in ['alumno', 'alumno.baja', 'mixto'])
    assert bool(p['employee']) == (username in ['docente', 'administrativo', 'admin', 'mixto'])
    if p['student']:
        assert p['student']['personId'] == p['id']
        assert p['student']['isEnrolled'] == (username != 'alumno.baja')
    if username == 'mixto':
        assert p['employee']['isTeacher'] and p['employee']['isAdministrative']
    if username == 'alumno.baja':
        assert p['personalEmail'] is None and p['curp'] is None
    token = next(cookie for cookie in jar if cookie.name == 'auth_token')
    marker = next(cookie for cookie in jar if cookie.name == 'session_exp')
    token_attributes = {key.lower(): value for key, value in token._rest.items()}
    marker_attributes = {key.lower(): value for key, value in marker._rest.items()}
    assert token.secure and 'httponly' in token_attributes and token_attributes['samesite'].lower() == 'lax'
    assert marker.secure and 'httponly' not in marker_attributes
    claims = json.loads(base64.urlsafe_b64decode(token.value.split('.')[1] + '=='))
    assert claims['person_id'] == p['id'] and claims['sub'] == profile['id']
    assert claims['exp'] - claims['nbf'] == 8 * 60 * 60
    assert 'curp' not in claims and 'person' not in claims

# Exact institutional lookup for consumers, without private profile fields.
for path in ['/api/people/' + PERSON, '/api/people?studentNumber=m20260001', '/api/people?username=ALUMNO']:
    person = request(api, path)
    assert person['personId'] == PERSON and person['student']['studentNumber'] == 'M20260001'
    assert set(person) == {'personId', 'name', 'institutionalEmail', 'student', 'employee'}
    for forbidden in ['curp', 'rfc', 'birthDate', 'street', 'personalEmail', 'phone', 'password']:
        assert forbidden not in json.dumps(person)
employee = request(api, '/api/people?employeeNumber=d0003')['employee']
assert employee['isTeacher'] and 'professionalLicense' not in employee
for path in ['/api/people', '/api/people?username=', '/api/people?username=alumno&studentNumber=M20260001',
             '/api/people?username=alumno&username=docente', '/api/people?email=alumno']:
    request(api, path, 400)
request(api, '/api/people?username=missing', 404)
request(api, '/api/people/00000000-0000-0000-0000-000000000000', 404)

# A person can exist without login credentials or any institutional profile.
fixture_id = str(uuid.uuid4())
try:
    sql("INSERT INTO people (id,first_name,institutional_email) VALUES (" + quote(fixture_id)
        + ",'Persona sin cuenta'," + quote(fixture_id + '@test.example') + ");")
    entry = request(api, '/api/people/' + fixture_id)
    assert entry['personId'] == fixture_id and entry['student'] is None and entry['employee'] is None
finally:
    sql("DELETE FROM people WHERE id=" + quote(fixture_id) + ';')

# Detect schema/EF drift through every read above, then exercise constraints with rollback.
assert sql("SELECT count(*) FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';") == '6'
checks = [
    ("UPDATE people SET institutional_email='ALUMNO@tecnm.example' WHERE id=" + quote(PERSON), 'check_violation'),
    ("UPDATE people SET institutional_email='docente@tecnm.example' WHERE id=" + quote(PERSON), 'unique_violation'),
    ("UPDATE users SET username='docente' WHERE id=" + quote(ACCOUNT), 'unique_violation'),
    ("UPDATE users SET role='Teacher' WHERE id=" + quote(ACCOUNT), 'check_violation'),
    ("UPDATE students SET student_number='M20220002' WHERE person_id=" + quote(PERSON), 'unique_violation'),
    ("UPDATE employees SET employee_number='A0004' WHERE employee_number='D0003'", 'unique_violation'),
    ("UPDATE people SET curp=(SELECT curp FROM people WHERE institutional_email='docente@tecnm.example') WHERE id=" + quote(PERSON), 'unique_violation'),
    ("UPDATE students SET program_id='00000000-0000-0000-0000-000000000000' WHERE person_id=" + quote(PERSON), 'foreign_key_violation'),
    ("UPDATE students SET admission_year=0 WHERE person_id=" + quote(PERSON), 'check_violation'),
    ("UPDATE students SET status='Unknown' WHERE person_id=" + quote(PERSON), 'check_violation'),
    ("UPDATE students SET status='Withdrawn' WHERE person_id=" + quote(PERSON), 'check_violation'),
    ("UPDATE employees SET is_teacher=false,is_administrative=false WHERE employee_number='D0003'", 'check_violation'),
    ("DELETE FROM people WHERE id=" + quote(PERSON), 'foreign_key_violation'),
]
for statement, expected in checks:
    sql("BEGIN; DO $$ BEGIN BEGIN " + statement + "; RAISE EXCEPTION 'Missing constraint'; EXCEPTION WHEN " + expected + " THEN NULL; END; END $$; ROLLBACK;")

# Prove fresh reads, immediate role changes and account revocation with an existing cookie.
original = json.loads(sql("SELECT row_to_json(p) FROM people p WHERE id=" + quote(PERSON) + ';'))
try:
    login(api, 'alumno')
    sql("UPDATE people SET first_name='Nombre actualizado' WHERE id=" + quote(PERSON) + ';')
    assert request(api, '/api/auth/me')['person']['firstName'] == 'Nombre actualizado'
    sql("UPDATE users SET role='Admin' WHERE id=" + quote(ACCOUNT) + ';')
    assert request(api, '/api/auth/me')['role'] == 'Admin'
    sql("UPDATE users SET is_active=false WHERE id=" + quote(ACCOUNT) + ';')
    request(api, '/api/auth/me', 401)
    request(api, '/api/people/' + PERSON, 401)
    login(api, 'alumno', 401)
finally:
    sql("UPDATE people SET first_name=" + quote(original['first_name']) + " WHERE id=" + quote(PERSON)
        + "; UPDATE users SET role='User',is_active=true WHERE id=" + quote(ACCOUNT) + ';')

# Seed is repeatable and never changes existing records or hashes.
before = sql("SELECT md5(string_agg(row_to_json(u)::text,',' ORDER BY id)) FROM users u;")
subprocess.run(['docker', 'compose', 'exec', '-T', 'db', 'psql', '-X', '-v', 'ON_ERROR_STOP=1',
                '-U', 'tecnm_app', '-d', 'tecnm_identity'], input=Path('seed-development.sql').read_text(),
               text=True, capture_output=True, check=True)
assert sql("SELECT md5(string_agg(row_to_json(u)::text,',' ORDER BY id)) FROM users u;") == before
login(api, 'alumno')
request(api, '/api/auth/logout', 204, method='POST')
request(api, '/api/auth/me', 401)
request(api, '/api/auth/logout', 204, method='POST')
# Forged JWT must not authenticate.
for cookie in list(jar): jar.clear(cookie.domain, cookie.path, cookie.name)
jar.set_cookie(http.cookiejar.Cookie(0, 'auth_token', 'invalid.jwt.signature', None, False,
                                    'localhost.local', False, False, '/', True, True, None, True, None, None, {}))
request(api, '/api/auth/me', 401)
print('OK: seis perfiles, cookies, consulta, privacidad, 13 restricciones, datos vigentes, revocación, semilla y logout.')
