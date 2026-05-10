<template>
  <div class="w-full rounded-xl bg-white p-4">
    <div class="mb-3 text-sm font-semibold text-gray-900">
      Comment
    </div>

    <textarea
      class="w-full min-h-24 resize-y rounded-lg border border-gray-300 bg-gray-50 px-3 py-2 text-sm text-gray-700 transition focus:bg-white focus:outline-none focus:ring-2 focus:ring-blue-500"
      :value="comment ?? ''"
      placeholder="Add comment..."
      @blur="save(($event.target as HTMLTextAreaElement).value)"
    />
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  nodeId: number
  comment?: string | null
}>()

const emit = defineEmits<{
  saveComment: [
    payload: {
      nodeId: number
      comment: string | null
    }
  ]
}>()

function save(value: string) {
  const comment = value.trim() ? value : null

  if ((props.comment ?? null) === comment) {
    return
  }

  emit('saveComment', {
    nodeId: props.nodeId,
    comment
  })
}
</script>