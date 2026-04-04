<template>
  <div class="border rounded-xl bg-white p-4">
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
</template>

<script setup lang="ts">
import type { GetTrainingSummaryResponse } from '@/services/trainingService'

defineProps<{
  summary: GetTrainingSummaryResponse
}>()
</script>