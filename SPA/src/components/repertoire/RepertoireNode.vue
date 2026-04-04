<template>
  <li class="pl-1">
    <div
      class="relative flex items-center gap-2 cursor-pointer rounded-md px-2 py-1.5
             text-sm text-gray-800 hover:bg-gray-100 transition"
      @click="onClick"
    >
      <!-- connector -->
      <span class="absolute -left-3 top-1/2 -translate-y-1/2 w-2 h-px bg-gray-200"></span>

      <!-- name -->
      <span class="truncate">
        {{ item.name }}
      </span>

      <!-- optional badge (ateičiai) -->
      <!--
      <span class="text-[10px] px-1.5 py-0.5 rounded bg-gray-200 text-gray-700">
        Main
      </span>
      -->
    </div>

    <ul
      v-if="children.length"
      class="ml-3 pl-3 border-l border-gray-200 space-y-1"
    >
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
  emit('open-opening', props.item.id)
}
</script>