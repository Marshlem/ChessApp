<template>
  <ul class="space-y-1">
    <RepertoireNode
      v-for="item in rootItems"
      :key="item.id"
      :item="item"
      :all-items="items"
      @open-opening="$emit('open-opening', $event)"
    />
  </ul>
</template>

<script setup lang="ts">
import type { RepertoireItem } from '@/services/repertoireService'
import RepertoireNode from './RepertoireNode.vue'
import { computed } from 'vue'

const props = defineProps<{
  items: RepertoireItem[]
}>()

const rootItems = computed(() =>
  props.items.filter(x => !x.parentId)
)
</script>
