<template>
  <div class="border p-4 rounded">
    <h3 class="text-lg mb-3">{{ isEdit ? 'Редактировать' : 'Новая' }} проводка</h3>
    <form @submit.prevent="submit">
      <div class="mb-3">
        <label class="block">Дата</label>
        <input type="date" v-model="form.date" class="border p-2 w-full" required />
      </div>
      <div class="mb-3">
        <label class="block">Часы (0.01 - 24)</label>
        <input type="number" step="0.01" min="0.01" max="24" v-model="form.hours" class="border p-2 w-full" required />
      </div>
      <div class="mb-3">
        <label class="block">Задача</label>
        <select v-model="form.taskId" class="border p-2 w-full" required :disabled="isEdit && !canChangeTask">
          <option v-for="task in activeTasks" :key="task.id" :value="task.id">
            {{ task.name }} ({{ task.project?.name }})
          </option>
        </select>
        <p v-if="isEdit && !canChangeTask" class="text-sm text-red-500">
          Нельзя изменить задачу, т.к. она стала неактивной после создания проводки.
        </p>
      </div>
      <div class="mb-3">
        <label class="block">Описание</label>
        <input v-model="form.description" class="border p-2 w-full" />
      </div>
      <div class="flex gap-2">
        <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded">
          {{ isEdit ? 'Обновить' : 'Создать' }}
        </button>
        <button v-if="isEdit" type="button" @click="cancel" class="bg-gray-300 px-4 py-2 rounded">
          Отмена
        </button>
      </div>
      <div v-if="errorMessage" class="mb-3 p-2 bg-red-100 text-red-700 rounded">
        {{ errorMessage }}
      </div>
      <div v-if="hoursExceedWarning" class="mb-3 p-2 bg-yellow-100 text-yellow-800 rounded">
        Внимание: сумма часов за этот день превысит 8 часов.
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import api from '@/services/api'
import { useTasksStore } from '@/stores/tasks'

const props = defineProps({
  entryToEdit: { type: Object, default: null }
})
const emit = defineEmits(['saved'])

const tasksStore = useTasksStore()
const isEdit = ref(false)
const canChangeTask = ref(true)
const errorMessage = ref('')
const form = ref({
  id: 0,
  date: new Date().toISOString().substr(0, 10),
  hours: 0,
  taskId: null,
  description: ''
})

// Предупреждение о превышении 8 часов (без блокировки)
const hoursExceedWarning = ref(false)

const activeTasks = computed(() => tasksStore.activeTasks)

onMounted(async () => {
  await tasksStore.fetchActiveTasks()
  if (!isEdit.value && activeTasks.value.length) {
    form.value.taskId = activeTasks.value[0].id
  }
})

// Проверка текущей суммы часов за выбранную дату (для предупреждения)
const checkDailyLimit = async () => {
  if (!form.value.date) return
  try {
    const summary = await api.getDailySummary(form.value.date)
    const currentTotal = summary.data.totalHours || 0
    const newTotal = isEdit.value
      ? currentTotal - (props.entryToEdit?.hours || 0) + form.value.hours
      : currentTotal + form.value.hours
    hoursExceedWarning.value = newTotal > 8
  } catch {
    hoursExceedWarning.value = false
  }
}

watch(() => form.value.date, checkDailyLimit)
watch(() => form.value.hours, checkDailyLimit)

watch(() => props.entryToEdit, async (newVal) => {
  errorMessage.value = ''
  if (newVal) {
    isEdit.value = true
    form.value = { ...newVal }
    try {
      const response = await api.getTimeEntry(newVal.id)
      canChangeTask.value = response.data.taskWasActiveAtCreation
    } catch {
      canChangeTask.value = false
    }
  } else {
    isEdit.value = false
    form.value = {
      id: 0,
      date: new Date().toISOString().substr(0, 10),
      hours: 0,
      taskId: activeTasks.value[0]?.id || null,
      description: ''
    }
    canChangeTask.value = true
  }
  checkDailyLimit()
}, { immediate: true })

const submit = async () => {
  errorMessage.value = ''
  try {
    if (isEdit.value) {
      await api.updateTimeEntry(form.value.id, form.value)
    } else {
      await api.createTimeEntry(form.value)
    }
    emit('saved')
    cancel()
  } catch (e) {
    errorMessage.value = e.message
  }
}

const cancel = () => {
  form.value = {
    id: 0,
    date: new Date().toISOString().substr(0, 10),
    hours: 0,
    taskId: activeTasks.value[0]?.id || null,
    description: ''
  }
  isEdit.value = false
}
</script>