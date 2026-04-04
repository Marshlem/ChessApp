<template>
  <div class="p-5 space-y-5">
    <div class="space-y-2 max-w-xl">
      <label class="block text-sm font-medium text-gray-700">
        Choose opening
      </label>

      <select
        :value="selectedOpeningId"
        class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-gray-300"
        @change="onSelectChange"
      >
        <option disabled value="">
          Select opening
        </option>

        <optgroup
          v-for="group in groupedOpenings"
          :key="group.label"
          :label="group.label"
        >
          <option
            v-for="opening in group.items"
            :key="opening.id"
            :value="opening.id"
          >
            {{ opening.name }}
          </option>
        </optgroup>
      </select>

      <div
        v-if="selectedOpening"
        class="rounded-lg border border-gray-200 px-4 py-3 bg-gray-50"
      >
        <div class="font-medium text-gray-900">
          {{ selectedOpening.name }}
        </div>
        <div class="text-sm text-gray-500">
          {{ selectedOpening.lines }} lines available
        </div>
      </div>
    </div>

    <div class="flex justify-end">
      <button
        type="button"
        class="rounded-lg bg-gray-900 text-white px-4 py-2 text-sm font-medium hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
        :disabled="!selectedOpening"
        @click="emit('start')"
      >
        Start
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

type MockOpening = {
  id: number
  name: string
  color: 'White' | 'Black'
  lines: number
}

type GroupedOpenings = {
  label: string
  items: MockOpening[]
}

const props = defineProps<{
  groupedOpenings: GroupedOpenings[]
  selectedOpeningId: number | ''
}>()

const emit = defineEmits<{
  (e: 'update:selected-opening-id', value: number | ''): void
  (e: 'start'): void
}>()

const selectedOpening = computed(() => {
  if (props.selectedOpeningId === '') return null

  for (const group of props.groupedOpenings) {
    const found = group.items.find(x => x.id === props.selectedOpeningId)
    if (found) return found
  }

  return null
})

function onSelectChange(event: Event) {
  const rawValue = (event.target as HTMLSelectElement).value

  if (rawValue === '') {
    emit('update:selected-opening-id', '')
    return
  }

  emit('update:selected-opening-id', Number(rawValue))
}
</script>