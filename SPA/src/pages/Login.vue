<template>
  <section class="min-h-[60vh] grid place-items-center p-6">
    <div class="w-full max-w-sm rounded-xl border border-gray-200 bg-white p-6 space-y-4 shadow-sm">

      <!-- Tabs -->
      <div class="flex gap-2">
        <button
          class="flex-1 rounded-md border px-3 py-2 text-sm font-medium transition hover:bg-gray-50"
          :class="tab === 'login'
            ? 'border-blue-600 bg-blue-50 text-gray-900'
            : 'border-gray-300 bg-white text-gray-600'"
          type="button"
          @click="tab = 'login'"
        >
          Sign in
        </button>

        <button
          class="flex-1 rounded-md border px-3 py-2 text-sm font-medium transition hover:bg-gray-50"
          :class="tab === 'register'
            ? 'border-blue-600 bg-blue-50 text-gray-900'
            : 'border-gray-300 bg-white text-gray-600'"
          type="button"
          @click="tab = 'register'"
        >
          Register
        </button>
      </div>

      <!-- Content -->
      <LoginForm v-if="tab === 'login'" />
      <RegisterForm v-else />
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import LoginForm from '@/components/LoginForm.vue'
import RegisterForm from '@/components/RegisterForm.vue'

const tab = ref('login')
</script>
