<template>
  <div class="w-72 rounded-xl border border-gray-200 bg-white p-4">
    <div class="mb-3 text-sm font-semibold text-gray-900">
      Candidate moves
    </div>

    <div v-if="loading" class="text-sm text-gray-500">Loading…</div>

    <div v-else-if="!moves.length" class="text-sm text-gray-500">
      No candidate moves from this position
    </div>

    <div v-else>
      <div
        class="grid grid-cols-[80px_minmax(0,1fr)_32px] items-center px-2 py-1 text-[11px] font-semibold text-gray-500 uppercase tracking-wide border-b border-gray-200 mb-1"
      >
        <span>Move</span>
        <span>Opening</span>
        <span></span>
      </div>

      <ul class="space-y-1">
        <li
          v-for="m in movesWithSan"
          :key="m.openingId + m.san"
          :class="[
            'grid grid-cols-[80px_minmax(0,1fr)_32px] items-center gap-2 rounded-lg px-2 py-2 transition',
            m.isFromCurrentOpening
              ? 'bg-blue-50 text-blue-900'
              : 'bg-gray-50 text-gray-700'
          ]"
        >
          <span class="font-semibold">
            {{ m.san }}
          </span>

          <div class="min-w-0 flex items-center gap-2">
            <span class="truncate text-xs opacity-80">
              {{ m.openingName }}
            </span>

            <span
              v-if="m.isFromCurrentOpening"
              class="shrink-0 rounded-md bg-blue-100 px-1.5 py-0.5 text-[10px] font-semibold text-blue-800"
            >
              Current
            </span>
          </div>

          <button
            v-if="m.isFromCurrentOpening"
            type="button"
            class="flex h-7 w-7 items-center justify-center rounded-md text-gray-500 hover:bg-red-100 hover:text-red-700 transition"
            title="Delete from here"
            @click="onDeleteFromHere(m)"
          >
            ✕
          </button>
          <span v-else></span>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { getCandidateMoves, type CandidateMove } from '@/services/repertoireService'
import { uciToSan } from '@/services/chessSan'

const props = defineProps<{
  fen: string
  currentOpeningId?: number
}>()

const emit = defineEmits<{
  (e: 'delete-from-here', move: CandidateMove & { san: string }): void
}>()

const moves = ref<CandidateMove[]>([])
const loading = ref(false)

const movesWithSan = computed(() =>
  moves.value.map(m => ({
    ...m,
    san: uciToSan(props.fen, m.moveUci)
  }))
)

function onDeleteFromHere(move: CandidateMove & { san: string }) {
  emit('delete-from-here', move)
}

async function load() {
  if (!props.fen) {
    moves.value = []
    return
  }

  loading.value = true
  try {
    moves.value = await getCandidateMoves({
      fen: props.fen,
      currentOpeningId: props.currentOpeningId
    })
  } finally {
    loading.value = false
  }
}

watch(() => props.fen, load, { immediate: true })
</script>
