<template>
  <div class="w-80 rounded-xl border border-gray-200 bg-white p-4">
    <div class="mb-3 text-sm font-semibold text-gray-900">
      Candidate moves
    </div>

    <div v-if="loading" class="text-sm text-gray-500">Loading…</div>

    <div v-else-if="!moves.length" class="text-sm text-gray-500">
      No candidate moves from this position
    </div>

    <div v-else>
      <div
        class="grid grid-cols-[70px_minmax(0,1fr)_92px_32px] items-center px-2 py-1 text-[11px] font-semibold text-gray-500 uppercase tracking-wide border-b border-gray-200 mb-1"
      >
        <span>Move</span>
        <span>Opening</span>
        <span>Line</span>
        <span></span>
      </div>

      <ul class="space-y-1">
        <li
          v-for="m in movesWithSan"
          :key="m.nodeId"
          :class="[
            'grid grid-cols-[70px_minmax(0,1fr)_92px_32px] items-center gap-2 rounded-lg px-2 py-2 transition',
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

          <select
            class="h-7 rounded-md border border-gray-300 bg-white px-1.5 text-xs text-gray-700 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-400"
            :value="m.lineType"
            :disabled="!m.isFromCurrentOpening || savingNodeId === m.nodeId"
            @change="onLineTypeChange(m, Number(($event.target as HTMLSelectElement).value) as LineType)"
          >
            <option
              v-for="option in lineTypeOptions"
              :key="option.value"
              :value="option.value"
              :disabled="!canChangeTo(option.value)"
            >
              {{ option.label }}
            </option>
          </select>

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

      <div v-if="error" class="mt-2 text-xs text-red-600">
        {{ error }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import {
  getCandidateMoves,
  updateCandidateMoveLineType,
  LineType,
  type CandidateMove
} from '@/services/repertoireService'
import { uciToSan } from '@/services/chessSan'

type CandidateMoveWithSan = CandidateMove & {
  san: string
}

const props = defineProps<{
  fen: string
  currentOpeningId?: number
  currentLineType?: LineType
}>()

const emit = defineEmits<{
  (e: 'delete-from-here', move: CandidateMoveWithSan): void
}>()

const moves = ref<CandidateMove[]>([])
const loading = ref(false)
const savingNodeId = ref<number | null>(null)
const error = ref<string | null>(null)

const lineTypeOptions: { value: LineType; label: string }[] = [
  { value: LineType.Main, label: 'Main' },
  { value: LineType.Side, label: 'Side' },
  { value: LineType.Other, label: 'Other' }
]

const currentLineType = computed<LineType>(() => props.currentLineType ?? LineType.Main)

  const movesWithSan = computed<CandidateMoveWithSan[]>(() =>
  moves.value.map(m => ({
    ...m,
    san: uciToSan(props.fen, m.moveUci)
  }))
)

function getLineTypeRank(lineType: LineType) {
  return lineType
}

function canChangeTo(lineType: LineType) {
  return getLineTypeRank(lineType) >= getLineTypeRank(currentLineType.value)
}

function updateLocalLineType(move: CandidateMove, lineType: LineType) {
  const index = moves.value.findIndex(x => x.nodeId === move.nodeId)

  if (index === -1) return

  const current = moves.value[index]
  if (!current) return

  moves.value[index] = {
    ...current,
    lineType
  }
}

async function onLineTypeChange(move: CandidateMoveWithSan, lineType: LineType) {
  error.value = null

  if (move.lineType === lineType) return

  if (!canChangeTo(lineType)) {
    error.value = `Cannot change ${currentLineType.value} line to ${lineType} line.`
    return
  }

  const previousLineType = move.lineType

  savingNodeId.value = move.nodeId
  updateLocalLineType(move, lineType)

  try {
    await updateCandidateMoveLineType({
      openingId: move.openingId,
      nodeId: move.nodeId,
      lineType
    })
  } catch {
    updateLocalLineType(move, previousLineType)
    error.value = 'Failed to update line type.'
  } finally {
    savingNodeId.value = null
  }
}

function onDeleteFromHere(move: CandidateMoveWithSan) {
  emit('delete-from-here', move)
}

async function load() {
  if (!props.fen) {
    moves.value = []
    return
  }

  loading.value = true
  error.value = null

  try {
    moves.value = await getCandidateMoves({
      fen: props.fen,
      currentOpeningId: props.currentOpeningId
    })
  } finally {
    loading.value = false
  }
}

watch(
  () => [props.fen, props.currentOpeningId],
  load,
  { immediate: true }
)
</script>