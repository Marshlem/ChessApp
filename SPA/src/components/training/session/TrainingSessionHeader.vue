<template>
  <div class="flex items-center justify-between border rounded-xl bg-white p-4">
    <div>
      <div class="text-sm text-gray-500">Training session</div>
      <div class="font-medium text-gray-900">
        {{ openingName || 'Practice' }}
      </div>
    </div>

    <button
      type="button"
      class="rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition"
      @click="emit('back')"
    >
      Back to setup
    </button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  openingName: string
}>()

const emit = defineEmits<{
  (e: 'back'): void
}>()
</script>