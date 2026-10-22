#!/usr/bin/env bash
set -euo pipefail

API_URL="${API_URL:-https://localhost:7001}"
API_URL="${API_URL%/}"
smoke_directory=$(mktemp -d)
trap 'rm -rf "$smoke_directory"' EXIT

request() {
  curl --insecure --silent --show-error --connect-timeout 5 --max-time 15 \
    --request "$1" --dump-header "$smoke_directory/headers" \
    --output "$smoke_directory/body" --write-out '%{http_code}' "$API_URL$2"
}

status=$(request GET /api/health)
test "$status" = 200 || { printf 'Health check devolvió %s.\n' "$status" >&2; exit 1; }
python3 - "$smoke_directory/body" <<'PY'
import json
from pathlib import Path
import sys
assert json.loads(Path(sys.argv[1]).read_text()) == {'status': 'ok'}
PY

assert_no_cookies() {
  python3 - "$smoke_directory/headers" <<'PY'
from pathlib import Path
import sys
headers = Path(sys.argv[1]).read_text().lower().splitlines()
assert not any(line.startswith('set-cookie:') for line in headers), 'El template no debe emitir cookies.'
PY
}
assert_no_cookies

while read -r method path; do
  status=$(request "$method" "$path")
  test "$status" = 404 || { printf '%s %s devolvió %s; se esperaba 404.\n' "$method" "$path" "$status" >&2; exit 1; }
  assert_no_cookies
done <<'ROUTES'
POST /api/auth/login
GET /api/auth/me
POST /api/auth/logout
GET /api/people/me
GET /api/people/00000000-0000-0000-0000-000000000001
ROUTES

printf 'OK: health check público, sin cookies y sin endpoints de autenticación ni padrón.\n'
