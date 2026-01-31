import axios from 'axios'
import router from '@/router'
import { logout } from '@/services/authService'

const baseURL = import.meta.env.VITE_API_URL || '/api'

const api = axios.create({
  baseURL
})


api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers = config.headers ?? {}
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let isRefreshing = false
let pendingRequests: Array<() => Promise<any>> = []

api.interceptors.response.use(
  response => response,
  async error => {
    if (error?.response?.status !== 401) {
      return Promise.reject(error)
    }

    const refreshToken = localStorage.getItem('refresh_token')
    if (!refreshToken) {
      logout()
      router.replace({ name: 'login' })
      return Promise.reject(error)
    }

    if (isRefreshing) {
        return new Promise<void>(resolve => {
        pendingRequests.push(() => api(error.config))
        resolve()
        })
    }

    isRefreshing = true

    try {
      const res = await axios.post(`${baseURL}/auth/refresh`, { refreshToken })

      localStorage.setItem('token', res.data.token)
      localStorage.setItem('refresh_token', res.data.refreshToken)

      pendingRequests.forEach(cb => cb())
      pendingRequests = []

      return api(error.config)
    } catch {
      logout()
      router.replace({ name: 'login' })
      return Promise.reject(error)
    } finally {
      isRefreshing = false
    }
  }
)

export default api
