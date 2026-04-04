<template>
  <ul class="pl-4 ml-2 border-l border-gray-200 space-y-1">
    <li v-for="node in nodes" :key="node.id" class="relative">
      <div class="flex items-center gap-2">
        <span class="absolute -left-[21px] top-3 h-px w-3 bg-gray-200"></span>

        <button
          type="button"
          class="text-sm font-medium text-gray-800 px-2 py-1 rounded-md hover:bg-gray-100 transition text-left"
          @click="$emit('select', node)"
        >
          {{ node.moveSan ?? 'start' }}
        </button>
      </div>

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