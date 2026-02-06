<template>
  <div class="max-w-4xl mx-auto p-6 space-y-6">
    <h1 class="text-2xl font-semibold text-gray-900">Openings</h1>

    <div v-if="loading" class="text-gray-500">
      Loading repertoire…
    </div>

    <div v-else class="space-y-6">
      <!-- WHITE -->
      <section>
        <h2 class="text-lg font-medium mb-2">White</h2>
        <ul class="space-y-1">
          <li
            v-for="o in whiteOpenings"
            :key="o.id"
            class="cursor-pointer hover:bg-gray-100 rounded px-2 py-1"
            @click="openOpening(o.id)"
          >
            ♟️ {{ o.name }}
          </li>
        </ul>
      </section>

      <!-- BLACK -->
      <section>
        <h2 class="text-lg font-medium mb-2">Black</h2>
        <ul class="space-y-1">
          <li
            v-for="o in blackOpenings"
            :key="o.id"
            class="cursor-pointer hover:bg-gray-100 rounded px-2 py-1"
            @click="openOpening(o.id)"
          >
            ♟️ {{ o.name }}
          </li>
        </ul>
      </section>
    </div>

    <button @click="showOpening = true">+ Opening</button>

    <CreateOpeningModal
      v-if="showOpening"
      @created="() => { showOpening = false; reload() }"
      @close="showOpening = false"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { getRepertoireTree, type RepertoireItem } from '@/services/repertoireService'
import router from '@/router'
import CreateOpeningModal from '@/components/repertoire/CreateOpeningModal.vue'

const items = ref<RepertoireItem[]>([])
const loading = ref(true)
const showOpening = ref(false)

onMounted(async () => {
  await reload()
  loading.value = false
})

const WHITE = 1
const BLACK = 2

const whiteOpenings = computed(() =>
  items.value.filter(x => x.color === WHITE)
)

const blackOpenings = computed(() =>
  items.value.filter(x => x.color === BLACK)
)

function openOpening(openingId: string) {
  router.push({ name: 'opening-editor', params: { openingId } })
}

async function reload() {
  items.value = await getRepertoireTree()
}
</script>

