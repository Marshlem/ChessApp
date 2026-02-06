<template>
  <ul class="pl-4 border-l">
    <li v-for="node in nodes" :key="node.id" class="mt-1">
      <span
        class="text-sm font-mono cursor-pointer hover:underline"
        @click="$emit('select', node)"
      >
        {{ node.moveSan ?? 'start' }}
      </span>

      <OpeningTree
        v-if="node.children.length"
        :nodes="node.children"
        @select="$emit('select', $event)"
      />
    </li>
  </ul>
</template>

<script setup lang="ts">
defineProps<{
  nodes: {
    id: number
    fen: string
    moveSan?: string
    children: any[]
  }[]
}>()

defineEmits<{
  (e: 'select', node: any): void
}>()
</script>
