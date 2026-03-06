<template>
  <div class="max-w-3xl mx-auto p-6 space-y-4">
    <h1 class="text-2xl font-semibold text-gray-900">
      Practicing
    </h1>

    <div v-if="summary" class="border rounded p-3">
      <div class="grid grid-cols-3 gap-4 text-sm">
        <div>
          <div class="text-gray-500">Total</div>
          <div class="text-lg font-semibold">{{ summary.totalPositions }}</div>
        </div>

        <div>
          <div class="text-gray-500">New</div>
          <div class="text-lg font-semibold">{{ summary.newPositions }}</div>
        </div>

        <div>
          <div class="text-gray-500">Due</div>
          <div class="text-lg font-semibold">{{ summary.duePositions }}</div>
        </div>
      </div>
    </div>

    <div v-if="position" class="border rounded p-3 space-y-4">
      <div>
        <div class="text-sm text-gray-500">Opening</div>
        <div class="font-medium">{{ position.openingName }}</div>
      </div>

      <div class="flex justify-center">
        <ChessBoard
          :key="boardKey"
          :fen="position.fen"
          :orientation="boardOrientation"
          @move="onBoardMove"
          @promotion="onPromotion"
        />
      </div>

      <div v-if="feedbackMessage" class="border rounded p-3 text-sm">
        {{ feedbackMessage }}
      </div>
    </div>

    <div v-else class="border rounded p-3 text-sm text-gray-500">
      No training positions available
    </div>

    <div
      v-if="showPromotion"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
    >
      <div class="bg-white rounded shadow p-4 space-y-3">
        <div class="grid grid-cols-4 gap-3">
          <button
            v-for="p in promotionPieces"
            :key="p"
            type="button"
            class="w-12 h-12 flex items-center justify-center rounded hover:bg-gray-100"
            @click="confirmPromotion(p)"
          >
            <img
              :src="`/chess-pieces/${promotionColor}${p}.svg`"
              alt=""
              class="w-10 h-10 select-none pointer-events-none"
              draggable="false"
            />
          </button>
        </div>

        <div class="flex justify-end">
          <button
            type="button"
            class="text-sm text-gray-600 hover:text-gray-900"
            @click="cancelPromotion"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import ChessBoard from '@/components/chess/ChessBoard.vue'
import {
  getNextTrainingPosition,
  getTrainingSummary,
  submitTrainingAnswer,
  type GetNextTrainingPositionResponse,
  type GetTrainingSummaryResponse,
  type SubmitTrainingAnswerResponse
} from '@/services/trainingService'

const WHITE = 1
const BLACK = 2

const summary = ref<GetTrainingSummaryResponse | null>(null)
const position = ref<GetNextTrainingPositionResponse | null>(null)
const feedbackMessage = ref('')
const submitting = ref(false)
const boardKey = ref(0)

const showPromotion = ref(false)
const promotionPieces = ['Q', 'R', 'B', 'N'] as const
const pendingPromotion = ref<{ from: string; to: string } | null>(null)

let autoPlayTimeoutId: number | null = null

type PromotionPiece = typeof promotionPieces[number]

const boardOrientation = computed<'white' | 'black'>(() => {
  if (!position.value) return 'white'
  return position.value.repertoireColor === BLACK ? 'black' : 'white'
})

const promotionColor = computed(() => {
  if (!position.value) return 'w'
  return position.value.sideToMove === 'w' ? 'w' : 'b'
})

onMounted(async () => {
  await loadPage()
})

onUnmounted(() => {
  clearAutoPlayTimeout()
})

async function loadPage() {
  await loadSummary()
  await loadNextPosition()
}

async function loadSummary() {
  summary.value = await getTrainingSummary()
}

async function loadNextPosition() {
  clearAutoPlayTimeout()

  position.value = await getNextTrainingPosition()
  feedbackMessage.value = ''
  boardKey.value++

  await maybeAutoPlayOpponentMove()
}

function isUsersTurn(): boolean {
  if (!position.value) return false

  return (
    (position.value.repertoireColor === WHITE && position.value.sideToMove === 'w') ||
    (position.value.repertoireColor === BLACK && position.value.sideToMove === 'b')
  )
}

function getExpectedMove() {
  if (!position.value) return null
  return position.value.moveOptions[0] ?? null
}

async function onBoardMove(uci: string) {
  if (!position.value || submitting.value) return
  if (!isUsersTurn()) return

  const expectedMove = getExpectedMove()
  if (!expectedMove) return

  if (expectedMove.moveUci !== uci) {
    feedbackMessage.value = `Incorrect. Correct move: ${expectedMove.moveSan}`
    boardKey.value++
    return
  }

  await submitAnswerInternal(expectedMove.openingNodeId)
}

function onPromotion(from: string, to: string) {
  pendingPromotion.value = { from, to }
  showPromotion.value = true
}

async function confirmPromotion(piece: PromotionPiece) {
  if (!pendingPromotion.value || !position.value || submitting.value) return
  if (!isUsersTurn()) return

  const expectedMove = getExpectedMove()
  if (!expectedMove) {
    cancelPromotion()
    return
  }

  const { from, to } = pendingPromotion.value

  showPromotion.value = false
  pendingPromotion.value = null

  const uci = `${from}${to}${piece}`

  if (expectedMove.moveUci !== uci) {
    feedbackMessage.value = `Incorrect. Correct move: ${expectedMove.moveSan}`
    boardKey.value++
    return
  }

  await submitAnswerInternal(expectedMove.openingNodeId)
}

function cancelPromotion() {
  showPromotion.value = false
  pendingPromotion.value = null
  boardKey.value++
}

async function submitAnswerInternal(selectedOpeningNodeId: number) {
  if (!position.value) return

  submitting.value = true

  try {
    const result: SubmitTrainingAnswerResponse = await submitTrainingAnswer({
      openingNodeId: position.value.openingNodeId,
      selectedOpeningNodeId
    })

    await loadSummary()

    if (!result.isCorrect) {
      feedbackMessage.value = `Incorrect. Correct move: ${result.correctMoveSan}`
      boardKey.value++
      return
    }

    updatePositionFromSubmitResult(result)
    feedbackMessage.value = ''

    if (positionEnded()) {
      await loadNextPosition()
      return
    }

    await maybeAutoPlayOpponentMove()
  } finally {
    submitting.value = false
  }
}

function updatePositionFromSubmitResult(result: SubmitTrainingAnswerResponse) {
  if (!position.value) return

  position.value = {
    ...position.value,
    openingNodeId: result.currentOpeningNodeId,
    fen: result.currentFen,
    sideToMove: result.currentSideToMove,
    moveOptions: result.moveOptions
  }

  boardKey.value++
}

function positionEnded(): boolean {
  return !position.value || position.value.moveOptions.length === 0
}

async function maybeAutoPlayOpponentMove() {
  if (!position.value) return
  if (isUsersTurn()) return

  const expectedMove = getExpectedMove()
  if (!expectedMove) {
    await loadNextPosition()
    return
  }

  clearAutoPlayTimeout()

  await new Promise<void>(resolve => {
    autoPlayTimeoutId = window.setTimeout(() => resolve(), 1000)
  })

  autoPlayTimeoutId = null

  if (!position.value || isUsersTurn()) return

  await submitAnswerInternal(expectedMove.openingNodeId)
}

function clearAutoPlayTimeout() {
  if (autoPlayTimeoutId !== null) {
    window.clearTimeout(autoPlayTimeoutId)
    autoPlayTimeoutId = null
  }
}
</script>