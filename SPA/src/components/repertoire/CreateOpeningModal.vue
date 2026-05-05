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
        v-model.number="color"
        class="w-full border rounded px-3 py-2"
      >
        <option :value="1">White</option>
        <option :value="2">Black</option>
      </select>

      <div class="space-y-1">
        <label class="text-sm font-medium">
          Import PGN optional
        </label>

        <input
          type="file"
          accept=".pgn,text/plain"
          class="w-full text-sm"
          @change="onFileChange"
        />

        <p v-if="fileName" class="text-xs text-gray-600">
          Selected: {{ fileName }}
        </p>

        <p v-if="fileError" class="text-xs text-red-600">
          {{ fileError }}
        </p>
      </div>

      <div class="flex justify-end gap-2">
        <button class="px-3 py-2 text-sm" @click="$emit('close')">
          Cancel
        </button>

        <button
          class="px-3 py-2 text-sm bg-blue-600 text-white rounded disabled:opacity-50"
          :disabled="!canSubmit"
          @click="submit"
        >
          Create
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { createOpening } from '@/services/repertoireService'

const props = defineProps<{
  parentId?: string | null
}>()

const emit = defineEmits<{
  (e: 'created', openingId: string): void
  (e: 'close'): void
}>()

const name = ref('')
const color = ref<number>(1)

const pgnText = ref<string | null>(null)
const fileName = ref<string | null>(null)
const fileError = ref<string | null>(null)
const loading = ref(false)

const canSubmit = computed(() =>
  !!name.value.trim() && !fileError.value && !loading.value
)

async function onFileChange(event: Event) {
  fileError.value = null
  pgnText.value = null
  fileName.value = null

  const input = event.target as HTMLInputElement
  const file = input.files?.[0]

  if (!file) {
    return
  }

  const isPgn =
    file.name.toLowerCase().endsWith('.pgn') ||
    file.type === 'text/plain' ||
    file.type === ''

  if (!isPgn) {
    fileError.value = 'Only PGN files are supported'
    input.value = ''
    return
  }

  fileName.value = file.name

  try {
    pgnText.value = await file.text()
  } catch {
    fileError.value = 'Failed to read PGN file'
    input.value = ''
  }
}

async function submit() {
  if (!canSubmit.value) {
    return
  }

  loading.value = true

  try {
    const openingId = await createOpening({
      name: name.value.trim(),
      color: color.value,
      pgnText: pgnText.value
    })

    emit('created', openingId)
  } finally {
    loading.value = false
  }
}
</script>