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

    <!-- Chessboard only -->
    <div class="border rounded p-4 flex justify-center">
        <ChessBoard
        v-if="currentFen"
        :key="boardKey"
        :fen="currentFen"
        @move="onBoardMove"
        />
      <div v-else class="text-sm text-gray-500">
        Loading board…
      </div>
    </div>

    <div v-if="tree" class="border rounded p-3">
        <h2 class="font-medium mb-2">Opening tree</h2>
        <OpeningTree :nodes="[tree]" @select="selectNode" />
    </div>
  </div>

    <div
    v-if="showPromotion"
    class="fixed inset-0 bg-black/40 flex items-center justify-center z-50"
    >
    <div class="bg-white rounded shadow p-4">
        <div class="grid grid-cols-4 gap-3">
            <button
            v-for="p in promotionPieces"
            :key="p"
            class="w-12 h-12 flex items-center justify-center"
            @click="confirmPromotion(p)"
            >
            <span
                class="piece"
                :class="promotionColor + p"
                style="width: 40px; height: 40px"
            />
            </button>
        </div>
    </div>
    </div>

</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import ChessBoard from '@/components/chess/ChessBoard.vue'
import OpeningTree from '@/components/repertoire/OpeningTree.vue'
import { computed } from 'vue'

interface Breadcrumb {
  id: number
  name: string
}

interface OpeningNode {
  id: number
  parentNodeId: number | null
  fen: string
  moveSan?: string
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
const promotionPieces = ['q', 'r', 'b', 'n'] as const
const promotionColor = computed(() =>
  currentFen.value.split(' ')[1] === 'w' ? 'w' : 'b'
)

type PromotionPiece = typeof promotionPieces[number]

const tree = computed(() => {
  if (!opening.value) return null
  return buildTree(opening.value.nodes, opening.value.rootNodeId)
})

const pendingPromotion = ref<{
  from: string
  to: string
} | null>(null)

onMounted(async () => {
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

  // 👇 promotion UX
  if (isPromotionMove(from, to, currentFen.value)) {
    pendingPromotion.value = { from, to }
    showPromotion.value = true
    return
  }

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
    moveSan: res.data.moveSan
  })

  boardKey.value++
}

function isPromotionMove(from: string, to: string, fen: string) {
  const rank = to[1]
  return rank === '1' || rank === '8'
}

async function confirmPromotion(piece: PromotionPiece) {
  if (!pendingPromotion.value) return

  const { from, to } = pendingPromotion.value

  showPromotion.value = false
  pendingPromotion.value = null

  await submitMove(`${from}${to}${piece}`)
}
</script>
