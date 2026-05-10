<template>
  <div class="w-full rounded-xl bg-white p-4">
    <div class="mb-3 text-sm font-semibold text-gray-900">
      Evaluation
    </div>

    <select
      class="w-full rounded-lg border border-gray-300 bg-gray-50 px-3 py-1 text-sm text-gray-700 transition focus:bg-white focus:outline-none focus:ring-2 focus:ring-blue-500"
      :value="evaluation ?? ''"
      @change="save(($event.target as HTMLSelectElement).value)"
    >
      <option value="">
        No evaluation
      </option>

      <option
        v-for="item in evaluationOptions"
        :key="item.value"
        :value="item.value"
      >
        {{ item.label }}
      </option>
    </select>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  nodeId: number
  evaluation?: number | null
}>()

const emit = defineEmits<{
  saveEvaluation: [
    payload: {
      nodeId: number
      evaluation: number | null
    }
  ]
}>()

const evaluationOptions = [
  { value: 1, label: 'Book move' },
  { value: 2, label: 'Best move' },
  { value: 3, label: 'Excellent move' },
  { value: 4, label: 'Good move' },
  { value: 5, label: 'Inaccuracy' },
  { value: 6, label: 'Mistake' },
  { value: 7, label: 'Blunder' },
  { value: 8, label: 'Brilliant move' },
  { value: 9, label: 'Great move' },
  { value: 10, label: 'Missed win' }
]

function save(value: string) {
  const evaluation = value ? Number(value) : null

  if ((props.evaluation ?? null) === evaluation) {
    return
  }

  emit('saveEvaluation', {
    nodeId: props.nodeId,
    evaluation
  })
}
</script>