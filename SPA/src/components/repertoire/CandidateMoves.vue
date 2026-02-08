<template>
  <div class="border rounded p-6 w-60">
    <div class="text-sm font-medium mb-2">Candidate moves</div>

    <div v-if="loading" class="text-sm text-gray-500">Loading…</div>
    <div v-else-if="!moves.length" class="text-sm text-gray-500">
      No candidate moves from this position
    </div>

    <div v-else>
      <!-- Headers -->
      <div
        class="flex justify-between px-2 py-1 text-xs font-semibold text-gray-500 uppercase tracking-wide border-b mb-1"
      >
        <span>Move</span>
        <span>Opening</span>
      </div>

      <ul class="space-y-1">
        <li
          v-for="m in movesWithSan"
          :key="m.openingId + m.san"
          :class="[
            'flex justify-between items-center px-2 py-1 rounded',
            m.isFromCurrentOpening
              ? 'bg-blue-50 text-blue-900'
              : 'bg-gray-50 text-gray-700'
          ]"
        >
          <span class="font-medium">{{ m.san }}</span>
          <span class="text-xs opacity-70 truncate max-w-[12rem]">
            {{ m.openingName }}
          </span>
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

const moves = ref<CandidateMove[]>([])
const loading = ref(false)

const movesWithSan = computed(() =>
  moves.value.map(m => ({
    ...m,
    san: uciToSan(props.fen, m.moveUci)
  }))
)

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
