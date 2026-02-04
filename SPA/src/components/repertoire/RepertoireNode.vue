<template>
  <li class="pl-2">
    <div
      class="flex items-center gap-2 cursor-pointer hover:bg-gray-100 rounded px-2 py-1"
      @click="onClick"
    >
      <span v-if="item.type === 1">📁</span>
      <span v-else>♟️</span>

      <span>{{ item.name }}</span>
    </div>

    <ul v-if="children.length" class="ml-4">
      <RepertoireNode
        v-for="c in children"
        :key="c.id"
        :item="c"
        :all-items="allItems"
        @open-opening="$emit('open-opening', $event)"
      />
    </ul>
  </li>
</template>

<script setup lang="ts">
import type { RepertoireItem } from '@/services/repertoireService'
import { computed } from 'vue'

const props = defineProps<{
  item: RepertoireItem
  allItems: RepertoireItem[]
}>()

const emit = defineEmits<{
  (e: 'open-opening', openingId: string): void
}>()

const children = computed(() =>
  props.allItems.filter(x => x.parentId === props.item.id)
)

function onClick() {
  if (props.item.type === 2) {
    emit('open-opening', props.item.id)
  }
}
</script>
