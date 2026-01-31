<template>
  <form class="grid gap-3" @submit.prevent="onRegister">
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
      autocomplete="new-password"
      class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
             placeholder:text-gray-400
             focus:outline-none focus:ring-2 focus:ring-blue-600/40"
    />

    <input
      v-model="confirm"
      type="password"
      placeholder="Confirm password"
      autocomplete="new-password"
      class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
             placeholder:text-gray-400
             focus:outline-none focus:ring-2 focus:ring-blue-600/40"
    />

    <button
      type="submit"
      class="rounded-md border border-blue-600 bg-blue-600 px-3 py-2 text-sm font-medium text-white
             hover:bg-blue-700 disabled:opacity-60 disabled:cursor-not-allowed"
    >
      Register
    </button>

    <p v-if="error" class="text-sm text-red-600">
      {{ error }}
    </p>

    <p v-if="success" class="text-sm text-green-600">
      Account created. You can now sign in.
    </p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { register } from '@/services/authService'

const email = ref('')
const password = ref('')
const confirm = ref('')
const error = ref('')
const success = ref(false)

const onRegister = async () => {
  error.value = ''
  success.value = false

  if (!email.value || !password.value) {
    error.value = 'Email and password are required'
    return
  }

  if (password.value !== confirm.value) {
    error.value = 'Passwords do not match'
    return
  }

  try {
    await register(email.value, password.value)
    success.value = true

    email.value = ''
    password.value = ''
    confirm.value = ''
  } catch {
    error.value = 'Registration failed'
  }
}
</script>
