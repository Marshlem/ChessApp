<template>
  <div class="cg-wrap">
    <div ref="boardEl" class="cg-board"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { Chessground } from 'chessground'
import { Chess } from 'chess.js'
import type { Key } from 'chessground/types'
import type { Square } from 'chess.js'

const props = defineProps<{ fen: string }>()
const chess = ref<Chess | null>(null)

const emit = defineEmits<{
  (e: 'move', uci: string): void
  (e: 'promotion', from: string, to: string): void
}>()

const boardEl = ref<HTMLElement | null>(null)
let ground: any = null

onMounted(() => {
  chess.value = new Chess(props.fen)

  ground = Chessground(boardEl.value!, {
    fen: props.fen,
    viewOnly: false,
    coordinates: true,
    movable: {
      free: false,
      color: 'both',
      dests: computeDests(),
      events: {
        after: (orig: string, dest: string) => {
        if (!chess.value) return

        const moves = chess.value.moves({
            square: orig as unknown as Square,
            verbose: true
        })

        const promotionMove = moves.find(
            m => m.to === dest && m.isPromotion()
        )

        if (promotionMove) {
            emit('promotion', orig, dest)
            return
        }

        emit('move', `${orig}${dest}`)
        }
      }
    }
  })
})

watch(
  () => props.fen,
  fen => {
    if (!fen) return

    chess.value = new Chess(fen)

    if (ground) {
      ground.set({
        fen,
        movable: {
          free: false,
          color: 'both',
          dests: computeDests()
        }
      })
    }
  },
  { immediate: true }
)

function computeDests(): Map<Key, Key[]> {
  const dests = new Map<Key, Key[]>()

  if (!chess.value) return dests

  const files = ['a','b','c','d','e','f','g','h']
  const ranks = ['1','2','3','4','5','6','7','8']

  for (const f of files) {
    for (const r of ranks) {
      const square = `${f}${r}` as Key
      const chessSquare = square as unknown as Square // ⭐ VIENINTELĖ VIETA

      const moves = chess.value.moves({
        square: chessSquare,
        verbose: true
      })

      if (moves.length) {
        dests.set(
          square,
          moves.map(m => m.to as Key)
        )
      }
    }
  }

  return dests
}


</script>
