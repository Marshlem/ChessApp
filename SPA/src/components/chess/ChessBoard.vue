<template>
  <div class="cg-wrap">
    <div ref="boardEl" class="cg-board"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { Chessground } from 'chessground'

const props = defineProps<{ fen: string }>()
const emit = defineEmits<{
  (e: 'move', uci: string): void
}>()

const boardEl = ref<HTMLElement | null>(null)
let ground: any = null

onMounted(() => {
  ground = Chessground(boardEl.value!, {
    fen: props.fen,
    viewOnly: false,
    movable: {
    free: true,   // ⭐ SVARBIAUSIA
    color: 'both',
    events: {
        after: (orig: string, dest: string) => {
        emit('move', `${orig}${dest}`)
        }
    }
    }
  })
})

watch(
  () => props.fen,
  fen => {
    if (ground && fen) {
      ground.set({ fen })
    }
  }
)
</script>
