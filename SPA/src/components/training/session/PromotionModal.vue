<template>
  <div
    v-if="show"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
  >
    <div class="bg-white rounded shadow p-4 space-y-3">
      <div class="grid grid-cols-4 gap-3">
        <button
          v-for="piece in promotionPieces"
          :key="piece"
          type="button"
          class="w-12 h-12 flex items-center justify-center rounded hover:bg-gray-100"
          @click="emit('confirm', piece)"
        >
          <img
            :src="`/chess-pieces/${promotionColor}${piece}.svg`"
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
          @click="emit('cancel')"
        >
          Cancel
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const promotionPieces = ['Q', 'R', 'B', 'N'] as const
type PromotionPiece = typeof promotionPieces[number]

defineProps<{
  show: boolean
  promotionColor: string
}>()

const emit = defineEmits<{
  (e: 'confirm', value: PromotionPiece): void
  (e: 'cancel'): void
}>()
</script>