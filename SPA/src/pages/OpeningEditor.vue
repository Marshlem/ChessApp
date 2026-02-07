<template>
  <div class="max-w-4xl mx-auto p-6 space-y-4">
    <!-- Breadcrumbs -->
    <div class="text-sm text-gray-500">
      <span
        v-for="(b, i) in opening?.breadcrumbs"
        :key="b.id"
      >
        <span v-if="i > 0"> / </span>
        {{ b.name }}
      </span>
    </div>

    <h1 class="text-2xl font-semibold">
      Opening editor
    </h1>

    <div class="display flex">
        <div class="display inline-block w-32 mr-6">
            <MoveTable
            v-if="opening"
            :rows="moveTable"
            />
        </div>

        <!-- Chessboard only -->
        <div class="flex justify-center">
            <ChessBoard
            v-if="currentFen"
            :key="boardKey"
            :fen="currentFen"
            @move="onBoardMove"
            @promotion="onPromotion"
            />
            <div v-else class="text-sm text-gray-500">
                Loading board…
            </div>
        </div>
        <div>
            <!-- /placehoder for future features like comments, position evaluation, etc. -->
        </div>
    </div>


    <div v-if="tree" class="border rounded p-3">
        <h2 class="font-medium mb-2">Opening tree</h2>
        <OpeningTree :nodes="[tree]" @select="selectNode" />
    </div>
  </div>

    <div
    v-if="showPromotion"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
    >
    <div class="bg-white rounded shadow p-4 space-y-3">
        <div class="grid grid-cols-4 gap-3">
        <button
            v-for="p in promotionPieces"
            :key="p"
            type="button"
            class="w-12 h-12 flex items-center justify-center rounded hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
            @click="confirmPromotion(p)"
        >
            <img
            :src="`/chess-pieces/${promotionColor}${p}.svg`"
            alt=""
            class="w-10 h-10 select-none pointer-events-none"
            draggable="false"
            />
        </button>
        </div>

        <!-- Cancel -->
        <div class="flex justify-end">
        <button
            type="button"
            class="text-sm text-gray-600 hover:text-gray-900"
            @click="cancelPromotion"
        >
            Cancel
        </button>
        </div>
    </div>
    </div>

</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import ChessBoard from '@/components/chess/ChessBoard.vue'
import OpeningTree from '@/components/repertoire/OpeningTree.vue'
import { computed } from 'vue'
import { uciToSan } from '@/services/chessSan'
import MoveTable from '@/components/repertoire/MoveTable.vue'

interface Breadcrumb {
  id: number
  name: string
}

interface OpeningNode {
  id: number
  parentNodeId: number | null
  fen: string
  moveSan?: string
  moveUci?: string 
}

interface OpeningDetails {
  rootNodeId: number
  breadcrumbs: Breadcrumb[]
  nodes: OpeningNode[]
}

interface TreeNode {
  id: number
  parentNodeId: number | null
  fen: string
  moveSan?: string
  children: TreeNode[]
}

const route = useRoute()
const openingId = route.params.openingId as string

const opening = ref<OpeningDetails | null>(null)
const currentFen = ref<string>('')
const currentNodeId = ref<number | null>(null)
const lastFen = ref<string>('')
const boardKey = ref(0)
const showPromotion = ref(false)
const promotionPieces = ['Q', 'R', 'B', 'N'] as const
const promotionColor = computed(() =>
  currentFen.value.split(' ')[1] === 'w' ? 'w' : 'b'
)

type PromotionPiece = typeof promotionPieces[number]

const tree = computed(() => {
  if (!opening.value) return null
  return buildTree(opening.value.nodes, opening.value.rootNodeId)
})

const moveTable = computed(() => {
  if (!opening.value || !currentNodeId.value) return []

  const line = buildLine(opening.value.nodes, currentNodeId.value)

  const rows: { move: number; white?: string; black?: string }[] = []

  line.forEach(node => {
    if (!node.parentNodeId || !node.moveUci) return

    const parent = opening.value!.nodes.find(n => n.id === node.parentNodeId)
    if (!parent) return

    // 1) Teisingas SAN iš parent FEN + UCI
    const san = uciToSan(parent.fen, node.moveUci)

    // 2) Iš parent FEN nusprendžiam į kurį stulpelį dėti
    const [, sideToMove, , , , fullmove] = parent.fen.split(' ')
    const moveNumber = Number(fullmove)

    let row = rows.find(r => r.move === moveNumber)
    if (!row) {
      row = { move: moveNumber }
      rows.push(row)
    }

    // jei parent’e ėjo baltieji -> šitas node yra baltųjų ėjimas
    if (sideToMove === 'w') row.white = san
    else row.black = san
  })

  return rows
})


const pendingPromotion = ref<{
  from: string
  to: string
} | null>(null)

onMounted(async () => {
  window.addEventListener('keydown', onKeyDown)
  const res = await api.get(`/openings/${openingId}`)
  opening.value = res.data

  if (!opening.value) return

  const rootNode = opening.value.nodes.find(
    n => n.id === opening.value!.rootNodeId
  )

  if (!rootNode) {
    console.error('Root node not found')
    return
  }

  currentFen.value = rootNode.fen
  currentNodeId.value = rootNode.id
})

onUnmounted(() => {
  window.removeEventListener('keydown', onKeyDown)
})

function buildTree(nodes: OpeningNode[], rootId: number): TreeNode | null {
  const map = new Map<number, TreeNode>()

  for (const n of nodes) {
    map.set(n.id, {
      id: n.id,
      parentNodeId: n.parentNodeId,
      fen: n.fen,
      moveSan: n.moveSan,
      children: []
    })
  }

  for (const node of map.values()) {
    if (node.parentNodeId !== null) {
      map.get(node.parentNodeId)?.children.push(node)
    }
  }

  return map.get(rootId) ?? null
}

function selectNode(node: TreeNode) {
  currentFen.value = node.fen
  currentNodeId.value = node.id
}

async function onBoardMove(uci: string) {
  if (!opening.value || !currentNodeId.value)
    return

  const from = uci.slice(0, 2)
  const to = uci.slice(2, 4)

  await submitMove(uci)
}

async function submitMove(uci: string) {
  const parentId = currentNodeId.value!
  lastFen.value = currentFen.value

  const res = await api.post(
    `/openings/${openingId}/nodes`,
    {
      parentNodeId: parentId,
      moveUci: uci
    }
  )

  if (!res.data.success) {
    currentFen.value = lastFen.value
    boardKey.value++
    return
  }

  currentFen.value = res.data.fen
  currentNodeId.value = res.data.nodeId

  opening.value!.nodes.push({
    id: res.data.nodeId,
    parentNodeId: parentId,
    fen: res.data.fen,
    moveUci: uci, 
    moveSan: res.data.moveSan
  })

  boardKey.value++
}

async function confirmPromotion(piece: PromotionPiece) {
  if (!pendingPromotion.value) return

  const { from, to } = pendingPromotion.value

  showPromotion.value = false
  pendingPromotion.value = null

  await submitMove(`${from}${to}${piece}`)
}

function onPromotion(from: string, to: string) {
  lastFen.value = currentFen.value   
  pendingPromotion.value = { from, to }
  showPromotion.value = true
}

function cancelPromotion() {
  showPromotion.value = false
  pendingPromotion.value = null
  currentFen.value = lastFen.value
  boardKey.value++
}

function onKeyDown(e: KeyboardEvent) {
  if (e.key === 'Escape' && showPromotion.value) {
    cancelPromotion()
  }
}

function buildLine(
  nodes: OpeningNode[],
  currentId: number
): OpeningNode[] {
  const map = new Map(nodes.map(n => [n.id, n]))
  const line: OpeningNode[] = []

  let cur = map.get(currentId)
  while (cur && cur.parentNodeId) {
    line.unshift(cur)
    cur = map.get(cur.parentNodeId)
  }

  return line
}

</script>
