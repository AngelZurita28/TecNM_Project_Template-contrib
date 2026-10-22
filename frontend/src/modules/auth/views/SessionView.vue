<script setup lang="ts">
import { isAxiosError } from 'axios'
import { computed, onMounted, ref } from 'vue'
import { UiAlert, UiBadge, UiBasePage, UiButton, UiCard, UiLayout } from '../../../shared/components'
import { me } from '../api/authApi'
import type { UserResponse } from '../types/auth'

const user = ref<UserResponse | null>(null)
const errorMessage = ref('')
const isLoading = ref(false)
const statusLabels: Record<string, string> = {
  Active: 'Vigente', TemporaryLeave: 'Baja temporal', Withdrawn: 'Baja definitiva', Graduated: 'Egresado',
}
type Field = [string, string | number | boolean | null | undefined]
type Section = { id: string; title: string; fields: Field[] }
const display = (value: Field[1]): string =>
  value == null || value === '' ? 'No registrado' : typeof value === 'boolean' ? (value ? 'Sí' : 'No') : String(value)
function civilDate(value: string | null): string | null {
  if (!value) return null
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}
const sections = computed<Section[]>(() => {
  if (!user.value) return []
  const account = user.value
  const p = account.person
  const result: Section[] = [
    { id: 'cuenta', title: 'Cuenta institucional', fields: [
      ['ID de persona', p.id], ['ID de cuenta', account.id], ['Usuario', account.username],
      ['Correo institucional', account.email], ['Permiso de cuenta', account.role === 'Admin' ? 'Administrador del sistema' : 'Usuario'],
      ['Acceso habilitado', account.isActive],
      ['Cuenta creada', new Intl.DateTimeFormat('es-MX', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(account.createdAt))],
    ] },
    { id: 'identidad', title: 'Datos personales', fields: [
      ['Nombre(s)', p.firstName], ['Primer apellido', p.lastName], ['Segundo apellido', p.secondLastName],
      ['Fecha de nacimiento', civilDate(p.birthDate)], ['Lugar de nacimiento', p.birthPlace],
      ['Sexo', p.sex], ['Nacionalidad', p.nationality], ['CURP', p.curp], ['RFC', p.rfc],
    ] },
    { id: 'contacto', title: 'Contacto', fields: [
      ['Correo personal', p.personalEmail], ['Teléfono', p.phone], ['Celular', p.mobilePhone],
    ] },
    { id: 'domicilio', title: 'Domicilio', fields: [
      ['Calle', p.street], ['Número', p.addressNumber], ['Colonia', p.neighborhood], ['Código postal', p.postalCode],
      ['Ciudad o municipio', p.city], ['Estado', p.state], ['País', p.country],
    ] },
    { id: 'emergencia', title: 'Contacto de emergencia', fields: [
      ['Nombre de contacto', p.emergencyContactName], ['Teléfono de emergencia', p.emergencyContactPhone],
      ['Parentesco', p.emergencyContactRelationship],
    ] },
  ]
  if (p.student) {
    const s = p.student
    result.push({ id: 'alumno', title: 'Información del alumno', fields: [
      ['Matrícula', s.studentNumber], ['Carrera', s.program.name], ['Clave de carrera', s.program.code],
      ['ID de carrera', s.program.id], ['Carrera vigente', s.program.isActive],
      ['Año de ingreso', s.admissionYear], ['Periodo de ingreso', s.admissionTerm], ['Generación', s.generation],
      ['Modalidad', s.modality], ['Plan de estudios', s.curriculumCode], ['Especialidad', s.specialty],
      ['Situación escolar', statusLabels[s.status] ?? s.status], ['Inscrito', s.isEnrolled],
    ] })
  }
  if (p.employee) {
    const e = p.employee
    result.push({ id: 'personal', title: 'Información del personal', fields: [
      ['Número de empleado', e.employeeNumber], ['Departamento', e.department.name], ['Clave de departamento', e.department.code],
      ['ID de departamento', e.department.id], ['Departamento vigente', e.department.isActive],
      ['Puesto', e.jobTitle], ['Fecha de ingreso laboral', civilDate(e.hireDate)], ['Personal vigente', e.isActive],
      ['Docente', e.isTeacher], ['Administrativo', e.isAdministrative], ['Título profesional', e.degree], ['Cédula profesional', e.professionalLicense],
    ] })
  }
  return result
})

async function loadSession(): Promise<void> {
  if (isLoading.value) return
  isLoading.value = true
  errorMessage.value = ''
  try {
    user.value = await me()
  } catch (error) {
    user.value = null
    if (!isAxiosError(error) || error.response?.status !== 401)
      errorMessage.value = 'No fue posible consultar tu información. Verifica tu conexión e intenta nuevamente.'
  } finally {
    isLoading.value = false
  }
}
onMounted(loadSession)
</script>

<template>
  <UiBasePage fluid :title="user ? user.name : 'Mi información'"
    description="Consulta tu información personal y tu vínculo con el Campus Monclova.">
    <template #actions>
      <UiButton variant="secondary" :loading="isLoading" :disabled="isLoading" @click="loadSession">Actualizar información</UiButton>
    </template>
    <div class="profile" :aria-busy="isLoading">
      <div role="status" aria-live="polite">
        <UiAlert v-if="isLoading" tone="info" title="Consultando tu información…" />
        <UiAlert v-else-if="errorMessage" tone="danger" :title="errorMessage" />
      </div>
      <template v-if="user && !isLoading">
        <UiLayout kind="row" aria-label="Vínculos institucionales">
          <UiBadge v-if="user.person.student" tone="info">Alumno</UiBadge>
          <UiBadge v-if="user.person.employee?.isTeacher" tone="info">Docente</UiBadge>
          <UiBadge v-if="user.person.employee?.isAdministrative" tone="info">Administrativo</UiBadge>
          <UiBadge v-if="user.person.student" :tone="user.person.student.isEnrolled ? 'success' : 'warning'">
            {{ user.person.student.isEnrolled ? 'Inscrito' : 'Sin inscripción' }}
          </UiBadge>
        </UiLayout>
        <UiCard v-for="section in sections" :id="section.id" :key="section.id" :title="section.title">
          <dl class="identity-list">
            <div v-for="[label, value] in section.fields" :key="label">
              <dt>{{ label }}</dt><dd>{{ display(value) }}</dd>
            </div>
          </dl>
        </UiCard>
      </template>
    </div>
  </UiBasePage>
</template>

<style scoped>
.profile { display: grid; gap: 24px; }
.profile > [role='status']:empty { display: none; }
.profile section { scroll-margin-top: 24px; }
.identity-list { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); margin: 0; }
.identity-list > div { min-width: 0; padding: 12px 16px; border-top: 1px solid var(--color-border-light); }
.identity-list dt { color: var(--color-gray-600); font-size: 0.875rem; }
.identity-list dd { margin: 4px 0 0; overflow-wrap: anywhere; }
@media (max-width: 640px) {
  .identity-list { grid-template-columns: 1fr; }
  .identity-list > div { padding-inline: 0; }
}
</style>
