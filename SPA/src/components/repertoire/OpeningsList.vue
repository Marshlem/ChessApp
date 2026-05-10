<template>
  <div class="w-35 rounded-xl border border-gray-200 bg-white p-4">
    <div class="mb-3 items-center">

      <button
        type="button"
        class="rounded-md bg-gray-900 px-3 py-1.5 text-xs font-semibold text-white transition hover:bg-gray-800 active:bg-gray-700"
        @click="showOpening = true"
      >
        + Add Opening
      </button>
    </div>

    <section>
      <h2 class="mb-1 text-xs font-semibold uppercase tracking-wide text-gray-500">
        White Openings
      </h2>

      <div v-if="!whiteOpenings.length" class="text-sm text-gray-500">
        No white openings
      </div>

      <ul v-else class="space-y-1">
        <li
          v-for="o in whiteOpenings"
          :key="o.id"
          :class="[
            'grid grid-cols-[minmax(0,1fr)_32px] items-center gap-2 rounded-lg px-2 py-2 transition',
            isActive(o.openingId)
              ? 'bg-blue-50 text-blue-900'
              : 'bg-gray-50 text-gray-700 hover:bg-gray-100'
          ]"
        >
          <button
            type="button"
            class="min-w-0 text-left text-sm font-medium"
            @click="openOpening(o.openingId)"
          >
            <span class="block truncate">
              {{ o.name }}
            </span>
          </button>

          <button
            type="button"
            class="flex h-7 w-7 items-center justify-center rounded-md text-gray-500 transition hover:bg-red-100 hover:text-red-700"
            title="Delete opening"
            @click.stop="askDelete(o)"
          >
            ✕
          </button>
        </li>
      </ul>
    </section>

    <section class="mt-4">
      <h2 class="mb-1 text-xs font-semibold uppercase tracking-wide text-gray-500">
        Black Openings
      </h2>

      <div v-if="!blackOpenings.length" class="text-sm text-gray-500">
        No black openings
      </div>

      <ul v-else class="space-y-1">
        <li
          v-for="o in blackOpenings"
          :key="o.id"
          :class="[
            'grid grid-cols-[minmax(0,1fr)_32px] items-center gap-2 rounded-lg px-2 py-2 transition',
            isActive(o.openingId)
              ? 'bg-blue-50 text-blue-900'
              : 'bg-gray-50 text-gray-700 hover:bg-gray-100'
          ]"
        >
          <button
            type="button"
            class="min-w-0 text-left text-sm font-medium"
            @click="openOpening(o.openingId)"
          >
            <span class="block truncate">
              {{ o.name }}
            </span>
          </button>

          <button
            type="button"
            class="flex h-7 w-7 items-center justify-center rounded-md text-gray-500 transition hover:bg-red-100 hover:text-red-700"
            title="Delete opening"
            @click.stop="askDelete(o)"
          >
            ✕
          </button>
        </li>
      </ul>
    </section>

    <CreateOpeningModal
      v-if="showOpening"
      @created="onCreated"
      @close="showOpening = false"
    />

    <div
      v-if="openingToDelete"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
    >
      <div class="w-96 rounded-xl bg-white p-5 shadow-lg">
        <div class="text-base font-semibold text-gray-900">
          Delete opening?
        </div>

        <div class="mt-2 text-sm text-gray-600">
          This will delete the opening and all its moves.
        </div>

        <div class="mt-2 rounded-lg bg-gray-50 px-3 py-2 text-sm font-medium text-gray-800">
          {{ openingToDelete.name }}
        </div>

        <div v-if="deleteError" class="mt-3 text-sm text-red-600">
          {{ deleteError }}
        </div>

        <div class="mt-5 flex justify-end gap-2">
          <button
            type="button"
            class="rounded-md px-3 py-1.5 text-sm text-gray-600 transition hover:bg-gray-100"
            :disabled="deleting"
            @click="openingToDelete = null"
          >
            Cancel
          </button>

          <button
            type="button"
            class="rounded-md bg-red-600 px-3 py-1.5 text-sm font-semibold text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:bg-red-300"
            :disabled="deleting"
            @click="confirmDelete"
          >
            {{ deleting ? 'Deleting…' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import {
  getRepertoireTree,
  deleteOpening,
  type RepertoireItem
} from '@/services/repertoireService'
import CreateOpeningModal from '@/components/repertoire/CreateOpeningModal.vue'
import { useRoute } from 'vue-router'

const items = ref<RepertoireItem[]>([])
const showOpening = ref(false)
const route = useRoute()

const openingToDelete = ref<RepertoireItem | null>(null)
const deleting = ref(false)
const deleteError = ref<string | null>(null)

const WHITE = 1
const BLACK = 2

const emit = defineEmits<{
  (e: 'select', id: number): void
  (e: 'deleted', id: number): void
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

function openOpening(openingId?: number | null) {
  if (openingId == null) return

  emit('select', openingId)
}

function isActive(openingId?: number | null) {
  return openingId != null && String(openingId) === String(route.params.openingId)
}

function askDelete(opening: RepertoireItem) {
  openingToDelete.value = opening
  deleteError.value = null
}

async function confirmDelete() {
  if (!openingToDelete.value?.openingId) return

  deleting.value = true
  deleteError.value = null

  try {
    const deletedOpeningId = openingToDelete.value.openingId

    await deleteOpening(deletedOpeningId)

    openingToDelete.value = null

    await reload()

    emit('deleted', deletedOpeningId)
  } catch {
    deleteError.value = 'Failed to delete opening.'
  } finally {
    deleting.value = false
  }
}

async function onCreated() {
  showOpening.value = false
  await reload()
}

async function reload() {
  items.value = await getRepertoireTree()
}

onMounted(reload)
</script>