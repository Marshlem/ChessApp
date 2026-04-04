<template>
  <div class="border rounded-xl bg-white p-4 space-y-4">
    <div>
      <div class="text-sm text-gray-500">Opening</div>
      <div class="font-medium">{{ openingName }}</div>
    </div>

    <div class="flex justify-center">
      <ChessBoard
        :key="boardKey"
        :fen="fen"
        :orientation="orientation"
        @move="emit('move', $event)"
        @promotion="onPromotion"
      />
    </div>

    <div v-if="feedbackMessage" class="border rounded p-3 text-sm">
      {{ feedbackMessage }}
    </div>
  </div>
</template>

<script setup lang="ts">
import ChessBoard from '@/components/chess/ChessBoard.vue'

defineProps<{
  boardKey: number
  fen: string
  openingName: string
  orientation: 'white' | 'black'
  feedbackMessage: string
}>()

const emit = defineEmits<{
  (e: 'move', value: string): void
  (e: 'promotion', from: string, to: string): void
}>()

function onPromotion(from: string, to: string) {
  emit('promotion', from, to)
}
</script>