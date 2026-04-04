<template>
  <table class="w-full text-sm border-separate border-spacing-y-1">
    <thead>
      <tr class="text-gray-400 text-xs uppercase tracking-wide">
        <th class="w-8 text-left px-2">#</th>
        <th class="text-left px-2">White</th>
        <th class="text-left px-2">Black</th>
      </tr>
    </thead>

    <tbody>
      <tr
        v-for="(row, index) in rows"
        :key="row.move"
        class="bg-white hover:bg-gray-50 rounded"
        :class="{
          'bg-gray-100 font-semibold': index === rows.length - 1
        }"
      >
        <td class="text-gray-400 px-2 py-1">{{ row.move }}</td>

        <td class="px-2 py-1">
          <span v-if="row.white" class="px-2 py-0.5 rounded bg-gray-100">
            {{ row.white }}
          </span>
        </td>

        <td class="px-2 py-1">
          <span v-if="row.black" class="px-2 py-0.5 rounded bg-gray-100">
            {{ row.black }}
          </span>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script setup lang="ts">
defineProps<{
  rows: {
    move: number
    white?: string
    black?: string
  }[]
}>()
</script>
