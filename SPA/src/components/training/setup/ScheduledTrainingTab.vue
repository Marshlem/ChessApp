<template>
  <div class="p-5 space-y-6">
    <div class="text-sm italic text-gray-600">
      Every opening pressable to start learning
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <ScheduledTrainingDayCard
        v-for="day in days"
        :key="day.key"
        :day="day"
        @start-opening="emit('start-opening', $event)"
      />
    </div>

    <div class="flex justify-end pt-2">
      <button
        type="button"
        class="rounded-lg bg-gray-900 text-white px-4 py-2 text-sm font-medium hover:bg-gray-800 transition"
        @click="emit('start-all')"
      >
        Start All
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import ScheduledTrainingDayCard from './ScheduledTrainingDayCard.vue'

type MockOpening = {
  id: number
  name: string
  color: 'White' | 'Black'
  lines: number
}

type ScheduledDay = {
  key: string
  monthLabel: string
  weekLabel: string
  white: MockOpening[]
  black: MockOpening[]
}

defineProps<{
  days: ScheduledDay[]
}>()

const emit = defineEmits<{
  (e: 'start-opening', value: MockOpening): void
  (e: 'start-all'): void
}>()
</script>