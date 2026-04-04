<template>
  <div class="flex border-b">
    <button
      type="button"
      class="px-5 py-3 text-sm font-medium transition"
      :class="
        activeTab === 'scheduled'
          ? 'bg-gray-900 text-white'
          : 'bg-white text-gray-700 hover:bg-gray-50'
      "
      @click="emit('change', 'scheduled')"
    >
      Scheduled training
    </button>

    <button
      type="button"
      class="px-5 py-3 text-sm font-medium transition border-l"
      :class="
        activeTab === 'custom'
          ? 'bg-gray-900 text-white'
          : 'bg-white text-gray-700 hover:bg-gray-50'
      "
      @click="emit('change', 'custom')"
    >
      Choose your own training
    </button>
  </div>
</template>

<script setup lang="ts">
type TrainingTab = 'scheduled' | 'custom'

defineProps<{
  activeTab: TrainingTab
}>()

const emit = defineEmits<{
  (e: 'change', value: TrainingTab): void
}>()
</script>