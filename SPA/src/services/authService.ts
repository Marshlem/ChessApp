import api from './api'
import router from '@/router'

const TOKEN_KEY = 'token'
const REFRESH_KEY = 'refresh_token'
const PROVIDER_KEY = 'auth_provider'

export function register(email: string, password: string) {
  email = (email ?? '').trim().toLowerCase()
  return api.post('/auth/register', { email, password })
}

export async function login(email: string, password: string) {
  email = (email ?? '').trim().toLowerCase()

  const res = await api.post('/auth/login', { email, password })

  const token = res?.data?.token
  const refreshToken = res?.data?.refreshToken

  if (token) localStorage.setItem(TOKEN_KEY, token)
  if (refreshToken) localStorage.setItem(REFRESH_KEY, refreshToken)

  localStorage.setItem(PROVIDER_KEY, 'local')

  return res.data
}

export function logout() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(REFRESH_KEY)
  localStorage.removeItem(PROVIDER_KEY)
}

export function logoutAndRedirect() {
  logout()
  router.push({ name: 'login' })
}

export function isAuthenticated() {
  return !!localStorage.getItem(TOKEN_KEY)
}

export function getAuthProvider() {
  return localStorage.getItem(PROVIDER_KEY) 
}

export function isLocalAuth() {
  return getAuthProvider() === 'local'
}
