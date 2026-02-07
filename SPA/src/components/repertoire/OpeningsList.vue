<template>
  <div class="w-full p-0 m-0 space-y-3">
    <div class="space-y-3">
      <section>
        <button class="text-sm font-medium mb-1 hover:bg-gray-100" @click="showOpening = true">
        Add Opening
      </button>
      </section>

      <section>
        <h2 class="text-sm font-medium mb-1">White Openings</h2>
        <ul class="space-y-0.5">
          <li
            v-for="o in whiteOpenings"
            :key="o.id"
            class="cursor-pointer hover:bg-gray-100 rounded px-2 py-0.5 text-sm"
            @click="openOpening(o.id)"
          >
            {{ o.name }}
          </li>
        </ul>
      </section>

      <section>
        <h2 class="text-sm font-medium mb-1">Black Openings</h2>
        <ul class="space-y-0.5">
          <li
            v-for="o in blackOpenings"
            :key="o.id"
            class="cursor-pointer hover:bg-gray-100 rounded px-2 py-0.5 text-sm"
            @click="openOpening(o.id)"
          >
            {{ o.name }}
          </li>
        </ul>
      </section>
    </div>

    <CreateOpeningModal
      v-if="showOpening"
      @created="() => { showOpening = false; reload() }"
      @close="showOpening = false"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { getRepertoireTree, type RepertoireItem } from '@/services/repertoireService'
import CreateOpeningModal from '@/components/repertoire/CreateOpeningModal.vue'
import { useRoute } from 'vue-router'

const items = ref<RepertoireItem[]>([])
const showOpening = ref(false)
const route = useRoute()

const WHITE = 1
const BLACK = 2

const emit = defineEmits<{
  (e: 'select', id: number): void
}>()

const whiteOpenings = computed(() =>
  items.value.filter(x => x.color === WHITE)
)

const blackOpenings = computed(() =>
  items.value.filter(x => x.color === BLACK)
)

watch(
  () => route.params.openingId,
  () => {
    reload()
  }
)

function openOpening(openingId: string  ) {
  if (openingId == null) return
  emit('select', Number(openingId))
}

async function reload() {
  items.value = await getRepertoireTree()
}

onMounted(reload)
</script>


