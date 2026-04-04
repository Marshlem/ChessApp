<template>
  <div class="border rounded-xl bg-white p-5 space-y-4">
    <div class="text-lg font-semibold text-gray-900">
      Set Up
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-700">
          Lines to learn
        </label>

        <input
          :value="modelValue.linesToLearn"
          type="number"
          min="1"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-gray-300"
          @input="updateLinesToLearn"
        />

        <div class="text-xs text-gray-500">
          How many lines from the selected opening should be included in this training batch.
        </div>
      </div>

      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-700">
          Line depth
        </label>

        <select
          :value="modelValue.lineDepth"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-gray-300"
          @change="updateLineDepth"
        >
          <option value="mainline">Only mainline</option>
          <option value="sidelines">Include sidelines</option>
          <option value="all">Other moves</option>
        </select>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
type SetupLineDepth = 'mainline' | 'sidelines' | 'all'

type TrainingSetup = {
  linesToLearn: number
  lineDepth: SetupLineDepth
}

const props = defineProps<{
  modelValue: TrainingSetup
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: TrainingSetup): void
}>()

function updateLinesToLearn(event: Event) {
  const value = Number((event.target as HTMLInputElement).value)

  emit('update:modelValue', {
    ...props.modelValue,
    linesToLearn: Number.isNaN(value) || value < 1 ? 1 : value
  })
}

function updateLineDepth(event: Event) {
  emit('update:modelValue', {
    ...props.modelValue,
    lineDepth: (event.target as HTMLSelectElement).value as SetupLineDepth
  })
}
</script>