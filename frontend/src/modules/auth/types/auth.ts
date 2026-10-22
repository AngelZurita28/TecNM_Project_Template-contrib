export type UserRole = 'Admin' | 'User'

export interface LoginRequest {
  username: string
  password: string
}

export interface CatalogResponse {
  id: string
  code: string
  name: string
  isActive: boolean
}

export interface StudentResponse {
  personId: string
  studentNumber: string
  programId: string
  admissionYear: number
  admissionTerm: string | null
  generation: string | null
  modality: string | null
  curriculumCode: string | null
  specialty: string | null
  status: string
  isEnrolled: boolean
  program: CatalogResponse
}

export interface EmployeeResponse {
  personId: string
  employeeNumber: string
  departmentId: string
  jobTitle: string
  hireDate: string | null
  isActive: boolean
  isTeacher: boolean
  isAdministrative: boolean
  degree: string | null
  professionalLicense: string | null
  department: CatalogResponse
}

export interface PersonResponse {
  id: string
  firstName: string
  lastName: string | null
  secondLastName: string | null
  institutionalEmail: string
  personalEmail: string | null
  birthDate: string | null
  birthPlace: string | null
  sex: string | null
  nationality: string | null
  curp: string | null
  rfc: string | null
  phone: string | null
  mobilePhone: string | null
  street: string | null
  addressNumber: string | null
  neighborhood: string | null
  postalCode: string | null
  city: string | null
  state: string | null
  country: string | null
  emergencyContactName: string | null
  emergencyContactPhone: string | null
  emergencyContactRelationship: string | null
  student: StudentResponse | null
  employee: EmployeeResponse | null
}

export interface UserResponse {
  id: string
  username: string
  email: string
  name: string
  role: UserRole
  isActive: boolean
  createdAt: string
  person: PersonResponse
}
