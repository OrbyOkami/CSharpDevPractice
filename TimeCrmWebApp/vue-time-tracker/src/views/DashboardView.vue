<template>
  <div>
    <h1 class="text-2xl font-bold mb-4">Дневная сводка</h1>
    <div class="max-w-md">
      <label class="block mb-2">Выберите дату</label>
      <input type="date" v-model="selectedDate" @change="fetchSummary" class="border p-2 w-full mb-4" />
      
      <div v-if="summary" class="border rounded p-6 text-center" :class="stickerClass">
        <div class="text-6xl font-bold mb-2">{{ summary.totalHours }}</div>
        <div class="text-xl">часов</div>
        <div class="mt-4 text-sm">
          {{ summary.totalHours < 8 ? 'Недостаточно часов' : summary.totalHours === 8 ? 'Норма' : 'Переработка' }}
        </div>
      </div>
      <p v-else class="text-gray-500">Нет данных за выбранный день</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '@/services/api'
import { format } from 'date-fns'

const selectedDate = ref(format(new Date(), 'yyyy-MM-dd'))
const summary = ref(null)

const fetchSummary = async () => {
  try {
    const response = await api.getDailySummary(selectedDate.value)
    summary.value = response.data
  } catch (e) {
    summary.value = null
  }
}

const stickerClass = computed(() => {
  if (!summary.value) return ''
  switch (summary.value.color) {
    case 'yellow': return 'bg-yellow-200 border-yellow-500'
    case 'green': return 'bg-green-200 border-green-500'
    case 'red': return 'bg-red-200 border-red-500'
    default: return 'bg-gray-100'
  }
})

onMounted(fetchSummary)
</script>