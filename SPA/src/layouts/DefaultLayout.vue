<script setup lang="ts">
import { isAuthenticated, logoutAndRedirect } from '@/services/authService'
import AppFooter from '@/layouts/AppFooter.vue'
</script>

<template>
  <div class="min-h-screen bg-gray-100 flex flex-col">
    <!-- HEADER -->
    <header class="h-14 flex items-center gap-6 px-4 border-b border-black/10">
      <!-- LOGO + BRAND -->
      <router-link to="/" class="flex items-center gap-2 font-semibold">
        <img class="h-7 w-auto" src="/logo40px.svg" alt="Marshlem Development" />
        <span>Chess Learning</span>
      </router-link>

      <!-- MENU (tik prisijungus) -->
      <nav v-if="isAuthenticated()" class="flex items-center gap-2">
        <router-link
          to="/openings"
          class="px-3 py-2 text-sm rounded-md hover:bg-black/5"
        >
          Openings
        </router-link>
        <router-link
          to="/practicing"
          class="px-3 py-2 text-sm rounded-md hover:bg-black/5"
        >
          Practicing
        </router-link>
      </nav>

      <!-- LOGOUT -->
      <div class="ml-auto">
        <button @click="logoutAndRedirect" class="px-3 py-2 text-sm rounded-md hover:bg-black/5">
          Logout
        </button>
      </div>
    </header>

    <!-- PAGE CONTENT -->
    <main class="p-4 flex-1">
      <router-view />
    </main>

    <!-- FOOTER (visada apačioje) -->
    <AppFooter />
  </div>
</template>
