<template>
  <div class="max-w-5xl mx-auto p-6 space-y-6">
    <h1 class="text-2xl font-semibold text-gray-900">
      Practicing
    </h1>

    <template v-if="!trainingStarted">
      <TrainingSetupPanel v-model="setup" />

      <div class="border rounded-xl bg-white overflow-hidden">
        <TrainingTabSwitcher
          :active-tab="activeTab"
          @change="activeTab = $event"
        />

        <ScheduledTrainingTab
          v-if="activeTab === 'scheduled'"
          :days="scheduledDays"
          @start-opening="startScheduledOpening"
          @start-all="startAllScheduled"
        />

        <CustomTrainingTab
          v-else
          :grouped-openings="groupedOpenings"
          :selected-opening-id="selectedCustomOpeningId"
          @update:selected-opening-id="selectedCustomOpeningId = $event"
          @start="startCustomOpening"
        />
      </div>
    </template>

    <template v-else>
      <TrainingSessionHeader
        :opening-name="startedOpeningName"
        @back="backToSetup"
      />

      <TrainingSummaryCards
        v-if="summary"
        :summary="summary"
      />

      <TrainingBoardCard
        v-if="position"
        :board-key="boardKey"
        :fen="position.fen"
        :opening-name="position.openingName"
        :orientation="boardOrientation"
        :feedback-message="feedbackMessage"
        @move="onBoardMove"
        @promotion="onPromotion"
      />

      <div
        v-else
        class="border rounded-xl bg-white p-4 text-sm text-gray-500"
      >
        No training positions available
      </div>
    </template>

    <PromotionModal
      :show="showPromotion"
      :promotion-color="promotionColor"
      @confirm="confirmPromotion"
      @cancel="cancelPromotion"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onUnmounted, ref } from 'vue'
import TrainingSetupPanel from '@/components/training/setup/TrainingSetupPanel.vue'
import TrainingTabSwitcher from '@/components/training/setup/TrainingTabSwitcher.vue'
import ScheduledTrainingTab from '@/components/training/setup/ScheduledTrainingTab.vue'
import CustomTrainingTab from '@/components/training/setup/CustomTrainingTab.vue'
import TrainingSessionHeader from '@/components/training/session/TrainingSessionHeader.vue'
import TrainingSummaryCards from '@/components/training/session/TrainingSummaryCards.vue'
import TrainingBoardCard from '@/components/training/session/TrainingBoardCard.vue'
import PromotionModal from '@/components/training/session/PromotionModal.vue'
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

type SetupLineDepth = 'mainline' | 'sidelines' | 'all'
type TrainingTab = 'scheduled' | 'custom'

type TrainingSetup = {
  linesToLearn: number
  lineDepth: SetupLineDepth
}

type MockOpening = {
  id: number
  name: string
  color: 'White' | 'Black'
  lines: number
}

type ScheduledDay = {
  key: string
  monthLabel: string
  weekLabel: string
  white: MockOpening[]
  black: MockOpening[]
}

type GroupedOpenings = {
  label: string
  items: MockOpening[]
}

const setup = ref<TrainingSetup>({
  linesToLearn: 8,
  lineDepth: 'mainline'
})

const activeTab = ref<TrainingTab>('scheduled')
const trainingStarted = ref(false)
const startedOpeningName = ref('')

const summary = ref<GetTrainingSummaryResponse | null>(null)
const position = ref<GetNextTrainingPositionResponse | null>(null)
const feedbackMessage = ref('')
const submitting = ref(false)
const boardKey = ref(0)

const showPromotion = ref(false)
const promotionPieces = ['Q', 'R', 'B', 'N'] as const
const pendingPromotion = ref<{ from: string; to: string } | null>(null)

const selectedCustomOpeningId = ref<number | ''>('')

let autoPlayTimeoutId: number | null = null

type PromotionPiece = typeof promotionPieces[number]

const openings = ref<MockOpening[]>([
  { id: 1, name: 'Italian Game', color: 'White', lines: 8 },
  { id: 2, name: 'Queen’s Gambit', color: 'White', lines: 6 },
  { id: 3, name: 'London System', color: 'White', lines: 15 },
  { id: 4, name: 'Sicilian Defense', color: 'Black', lines: 9 },
  { id: 5, name: 'French Defense', color: 'Black', lines: 7 },
  { id: 6, name: 'Caro-Kann Defense', color: 'Black', lines: 6 }
])

const scheduledDays = ref<ScheduledDay[]>([
  {
    key: 'day-1',
    monthLabel: 'March 17',
    weekLabel: 'Monday',
    white: [
      { id: 1, name: 'Italian Game', color: 'White', lines: 8 },
      { id: 2, name: 'Queen’s Gambit', color: 'White', lines: 6 }
    ],
    black: [
      { id: 4, name: 'Sicilian Defense', color: 'Black', lines: 9 }
    ]
  },
  {
    key: 'day-2',
    monthLabel: 'March 18',
    weekLabel: 'Tuesday',
    white: [
      { id: 3, name: 'London System', color: 'White', lines: 15 }
    ],
    black: [
      { id: 5, name: 'French Defense', color: 'Black', lines: 7 },
      { id: 6, name: 'Caro-Kann Defense', color: 'Black', lines: 6 }
    ]
  }
])

const groupedOpenings = computed<GroupedOpenings[]>(() => [
  {
    label: 'White',
    items: openings.value.filter(x => x.color === 'White')
  },
  {
    label: 'Black',
    items: openings.value.filter(x => x.color === 'Black')
  }
])

const selectedCustomOpening = computed(() => {
  if (selectedCustomOpeningId.value === '') return null
  return openings.value.find(x => x.id === selectedCustomOpeningId.value) ?? null
})

const boardOrientation = computed<'white' | 'black'>(() => {
  if (!position.value) return 'white'
  return position.value.repertoireColor === BLACK ? 'black' : 'white'
})

const promotionColor = computed(() => {
  if (!position.value) return 'w'
  return position.value.sideToMove === 'w' ? 'w' : 'b'
})

onUnmounted(() => {
  clearAutoPlayTimeout()
})

async function startScheduledOpening(item: MockOpening) {
  startedOpeningName.value = item.name
  await startTraining()
}

async function startAllScheduled() {
  startedOpeningName.value = 'Scheduled training'
  await startTraining()
}

async function startCustomOpening() {
  if (!selectedCustomOpening.value) return

  startedOpeningName.value = selectedCustomOpening.value.name
  await startTraining()
}

async function startTraining() {
  trainingStarted.value = true
  await loadPage()
}

function backToSetup() {
  clearAutoPlayTimeout()
  trainingStarted.value = false
  position.value = null
  feedbackMessage.value = ''
  startedOpeningName.value = ''
}

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
  const uci = `${from}${to}${piece}`

  showPromotion.value = false
  pendingPromotion.value = null

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