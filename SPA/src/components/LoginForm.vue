<template>
  <form class="grid gap-3" @submit.prevent="onLogin">
    <input
      v-model="email"
      type="email"
      placeholder="Email"
      autocomplete="email"
      class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
             placeholder:text-gray-400
             focus:outline-none focus:ring-2 focus:ring-blue-600/40"
    />

    <input
      v-model="password"
      type="password"
      placeholder="Password"
      autocomplete="current-password"
      class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
             placeholder:text-gray-400
             focus:outline-none focus:ring-2 focus:ring-blue-600/40"
    />

    <button
      :disabled="loading"
      type="submit"
      class="rounded-md border border-blue-600 bg-blue-600 px-3 py-2 text-sm font-medium text-white
             hover:bg-blue-700 disabled:opacity-60 disabled:cursor-not-allowed"
    >
      {{ loading ? 'Signing in…' : 'Sign in' }}
    </button>

    <p v-if="error" class="text-sm text-red-600">
      {{ error }}
    </p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { login } from '@/services/authService'
import { useRouter } from 'vue-router'

const router = useRouter()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const onLogin = async () => {
  error.value = ''
  loading.value = true

  try {
    await login(email.value, password.value)
    router.push({ name: 'home' })
  } catch {
    error.value = 'Invalid email or password'
  } finally {
    loading.value = false
  }
}
</script>
