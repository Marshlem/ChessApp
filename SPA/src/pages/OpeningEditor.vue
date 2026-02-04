<template>
  <div class="max-w-6xl mx-auto p-6 space-y-4">
    <!-- HEADER -->
    <div class="flex items-center gap-2 text-sm text-gray-500">
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

    <!-- MAIN LAYOUT -->
    <div class="grid grid-cols-3 gap-6">
      <!-- LEFT: move list (vėliau) -->
      <div class="col-span-1 border rounded p-3">
        <h2 class="font-medium mb-2">Move list</h2>
        <p class="text-sm text-gray-500">
          (will be added)
        </p>
      </div>

      <!-- CENTER: board placeholder -->
      <div class="col-span-1 border rounded p-3 flex items-center justify-center">
        <div class="text-gray-400 text-sm">
          Chessboard placeholder
        </div>
      </div>

      <!-- RIGHT: candidate moves -->
      <div class="col-span-1 border rounded p-3">
        <h2 class="font-medium mb-2">Candidate moves</h2>

        <div v-if="loading" class="text-sm text-gray-500">
          Loading…
        </div>

        <ul v-else class="space-y-2">
          <li
            v-for="m in candidateMoves"
            :key="m.nodeId"
            class="border rounded px-2 py-1 hover:bg-gray-50 cursor-pointer"
            @click="selectNode(m.nodeId)"
          >
            <div class="flex justify-between">
              <span class="font-mono">
                {{ m.moveSan }}
              </span>

              <span
                class="text-xs"
                :class="m.lineType === 1 ? 'text-green-600' : 'text-blue-600'"
              >
                {{ m.lineType === 1 ? 'Main' : 'Side' }}
              </span>
            </div>

            <div class="text-xs text-gray-500">
              trained: {{ m.trainedCount }} / failed: {{ m.failedCount }}
            </div>

            <div v-if="m.comment" class="text-xs italic text-gray-600">
              {{ m.comment }}
            </div>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'

interface Breadcrumb {
  id: string
  name: string
}

interface OpeningDetails {
  rootNodeId: string
  breadcrumbs: Breadcrumb[]
}

const route = useRoute()
const openingId = route.params.openingId as string

const opening = ref<OpeningDetails | null>(null)
const candidateMoves = ref<any[]>([])
const loading = ref(true)

const currentNodeId = ref<string | null>(null)

onMounted(async () => {
  const openingRes = await api.get(`/openings/${openingId}`)
  opening.value = openingRes.data

  if (!opening.value) return   

  currentNodeId.value = opening.value.rootNodeId
  await loadChildren()
})

async function loadChildren() {
  if (!currentNodeId.value) return

  loading.value = true
  const res = await api.get(
    `/openings/${openingId}/nodes/${currentNodeId.value}/children`
  )

  candidateMoves.value = res.data
  loading.value = false
}

async function selectNode(nodeId: string) {
  currentNodeId.value = nodeId
  await loadChildren()
}
</script>
