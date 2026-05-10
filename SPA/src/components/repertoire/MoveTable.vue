<template>
  <div class="w-full rounded-xl border border-gray-200 bg-white p-4">
    <div class="mb-3 text-sm font-semibold text-gray-900">
      Move list
    </div>

    <table class="w-full text-sm border-separate border-spacing-y-1">
      <thead>
        <tr class="text-gray-500 text-xs uppercase tracking-wide">
          <th class="w-8 text-left px-2">#</th>
          <th class="text-left px-2">White</th>
          <th class="text-left px-2">Black</th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="(row, index) in rows"
          :key="row.move"
          :class="[
            'rounded-lg transition',
            index === rows.length - 1
              ? 'bg-blue-50 text-blue-900 font-semibold'
              : 'bg-gray-50 text-gray-700 hover:bg-gray-100'
          ]"
        >
          <td class="text-gray-500 px-2 py-1">
            {{ row.move }}
          </td>

          <td class="px-2 py-1">
            <span
              v-if="row.white"
              class="px-2 py-0.5 rounded-md bg-white/80"
            >
              {{ row.white }}
            </span>
          </td>

          <td class="px-2 py-1">
            <span
              v-if="row.black"
              class="px-2 py-0.5 rounded-md bg-white/80"
            >
              {{ row.black }}
            </span>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="!rows.length" class="text-sm text-gray-500">
      No moves in this line
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  rows: {
    move: number
    white?: string
    black?: string
    whiteComment?: string | null
    blackComment?: string | null
  }[]
}>()
</script>