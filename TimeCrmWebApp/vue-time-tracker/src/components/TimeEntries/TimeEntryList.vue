<template>
  <div class="mb-8">
    <h2 class="text-xl mb-2">Проводки</h2>
    
    <!-- Фильтры -->
    <div class="flex gap-4 mb-4 items-end">
      <div>
        <label class="block text-sm">Дата</label>
        <input type="date" v-model="filters.date" class="border p-2" />
      </div>
      <div>
        <label class="block text-sm">Месяц</label>
        <select v-model="filters.month" class="border p-2">
          <option :value="null">Все</option>
          <option v-for="m in 12" :key="m" :value="m">{{ m }}</option>
        </select>
      </div>
      <div>
        <label class="block text-sm">Год</label>
        <input type="number" v-model="filters.year" class="border p-2 w-24" />
      </div>
      <button @click="applyFilters" class="bg-blue-500 text-white px-4 py-2 rounded">
        Применить
      </button>
      <button @click="clearFilters" class="bg-gray-300 px-4 py-2 rounded">
        Сбросить
      </button>
    </div>

    <div v-if="loading">Загрузка...</div>
    <table v-else class="min-w-full border">
      <thead>
        <tr>
          <th>Дата</th>
          <th>Часы</th>
          <th>Задача</th>
          <th>Описание</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="entry in entries" :key="entry.id">
          <td>{{ formatDate(entry.date) }}</td>
          <td>{{ entry.hours }}</td>
          <td>{{ entry.task?.name }}</td>
          <td>{{ entry.description }}</td>
          <td>
            <button @click="$emit('edit', entry)" class="text-blue-600 mr-2">Ред.</button>
            <button @click="handleDelete(entry.id)" class="text-red-600">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import { format, parseISO } from 'date-fns'

const entries = ref([])
const loading = ref(false)
const filters = ref({
  date: '',
  month: null,
  year: null
})

const emit = defineEmits(['edit'])

const fetchEntries = async (params = {}) => {
  loading.value = true
  try {
    const response = await api.getTimeEntries(params)
    entries.value = response.data
  } finally {
    loading.value = false
  }
}

const applyFilters = () => {
  const params = {}
  if (filters.value.date) {
    params.date = filters.value.date
  } else if (filters.value.month && filters.value.year) {
    params.month = filters.value.month
    params.year = filters.value.year
  }
  fetchEntries(params)
}

const clearFilters = () => {
  filters.value = { date: '', month: null, year: null }
  fetchEntries()
}

const handleDelete = async (id) => {
  if (confirm('Удалить проводку?')) {
    try {
      await api.deleteTimeEntry(id)
      await fetchEntries()
    } catch (e) {
      alert('Ошибка: ' + e.response?.data || e.message)
    }
  }
}

const formatDate = (dateStr) => {
  return format(parseISO(dateStr), 'dd.MM.yyyy')
}

defineExpose({ refresh: fetchEntries })
onMounted(fetchEntries)
</script>