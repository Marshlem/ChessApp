<template>
  <div class="border rounded-xl overflow-hidden">
    <div class="bg-gray-100 px-4 py-3 border-b">
      <div class="font-semibold text-gray-900">
        {{ day.monthLabel }}
      </div>
      <div class="text-sm text-gray-600">
        {{ day.weekLabel }}
      </div>
    </div>

    <div class="p-4 space-y-5">
      <div>
        <div class="text-sm font-semibold text-gray-900 mb-2">
          White
        </div>

        <div class="space-y-2">
          <button
            v-for="item in day.white"
            :key="item.id"
            type="button"
            class="w-full text-left rounded-lg border border-gray-200 px-4 py-3 hover:bg-gray-50 transition"
            @click="emit('start-opening', item)"
          >
            <div class="font-medium text-gray-900">
              {{ item.name }}
            </div>
            <div class="text-sm text-gray-500">
              {{ item.lines }} lines available
            </div>
          </button>

          <div
            v-if="day.white.length === 0"
            class="text-sm text-gray-400"
          >
            No white openings scheduled
          </div>
        </div>
      </div>

      <div>
        <div class="text-sm font-semibold text-gray-900 mb-2">
          Black
        </div>

        <div class="space-y-2">
          <button
            v-for="item in day.black"
            :key="item.id"
            type="button"
            class="w-full text-left rounded-lg border border-gray-200 px-4 py-3 hover:bg-gray-50 transition"
            @click="emit('start-opening', item)"
          >
            <div class="font-medium text-gray-900">
              {{ item.name }}
            </div>
            <div class="text-sm text-gray-500">
              {{ item.lines }} lines available
            </div>
          </button>

          <div
            v-if="day.black.length === 0"
            class="text-sm text-gray-400"
          >
            No black openings scheduled
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
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
  day: ScheduledDay
}>()

const emit = defineEmits<{
  (e: 'start-opening', value: MockOpening): void
}>()
</script>