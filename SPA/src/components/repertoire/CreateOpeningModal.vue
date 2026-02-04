<template>
  <div class="fixed inset-0 bg-black/40 grid place-items-center z-50">
    <div class="bg-white rounded-lg w-full max-w-sm p-4 space-y-4">
      <h2 class="text-lg font-semibold">Create opening</h2>

      <input
        v-model="name"
        type="text"
        placeholder="Opening name"
        class="w-full border rounded px-3 py-2"
      />

      <select
        v-model="color"
        class="w-full border rounded px-3 py-2"
      >
        <option :value="1">White</option>
        <option :value="2">Black</option>
      </select>

      <div class="flex justify-end gap-2">
        <button class="px-3 py-2 text-sm" @click="$emit('close')">
          Cancel
        </button>

        <button
          class="px-3 py-2 text-sm bg-blue-600 text-white rounded disabled:opacity-50"
          :disabled="!name"
          @click="submit"
        >
          Create
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { createOpening } from '@/services/repertoireService'

const props = defineProps<{
  parentId?: string | null
}>()

const emit = defineEmits<{
  (e: 'created', openingId: string): void
  (e: 'close'): void
}>()

const name = ref('')
const color = ref(1) // 1 = White, 2 = Black

async function submit() {
  const openingId = await createOpening({
    parentId: props.parentId ?? null,
    name: name.value.trim(),
    color: color.value
  })

  emit('created', openingId)
}
</script>
