<template>
  <table class="w-full text-sm">
    <thead>
      <tr class="text-gray-500">
        <th class="w-8">#</th>
        <th>White</th>
        <th>Black</th>
      </tr>
    </thead>

    <tbody>
      <tr v-for="row in rows" :key="row.move" class="border-t">
        <td class="text-gray-400">{{ row.move }}</td>
        <td class="font-medium">{{ row.white }}</td>
        <td class="font-medium">{{ row.black }}</td>
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
