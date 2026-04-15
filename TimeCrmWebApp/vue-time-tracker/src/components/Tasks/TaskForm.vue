<template>
  <div class="border p-4 rounded">
    <h3 class="text-lg mb-3">{{ isEdit ? 'Редактировать' : 'Создать' }} задачу</h3>
    <form @submit.prevent="submit">
      <div class="mb-3">
        <label class="block">Название</label>
        <input v-model="form.name" class="border p-2 w-full" required />
      </div>
      <div class="mb-3">
        <label class="block">Проект</label>
        <select v-model="form.projectId" class="border p-2 w-full" required>
          <option v-for="p in projectsStore.projects" :key="p.id" :value="p.id">
            {{ p.name }}
          </option>
        </select>
      </div>
      <div class="mb-3">
        <label class="inline-flex items-center">
          <input type="checkbox" v-model="form.isActive" />
          <span class="ml-2">Активна</span>
        </label>
      </div>
      <div class="flex gap-2">
        <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded">
          {{ isEdit ? 'Обновить' : 'Создать' }}
        </button>
        <button v-if="isEdit" type="button" @click="cancel" class="bg-gray-300 px-4 py-2 rounded">
          Отмена
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { useTasksStore } from '@/stores/tasks'
import { useProjectsStore } from '@/stores/projects'

const props = defineProps({
  taskToEdit: { type: Object, default: null }
})
const emit = defineEmits(['saved'])

const tasksStore = useTasksStore()
const projectsStore = useProjectsStore()
const isEdit = ref(false)
const form = ref({
  id: 0,
  name: '',
  projectId: null,
  isActive: true
})

onMounted(async () => {
  await projectsStore.fetchProjects()
})

watch(() => props.taskToEdit, (newVal) => {
  if (newVal) {
    isEdit.value = true
    form.value = { ...newVal, projectId: newVal.projectId }
  } else {
    isEdit.value = false
    form.value = { id: 0, name: '', projectId: projectsStore.projects[0]?.id || null, isActive: true }
  }
}, { immediate: true })

const submit = async () => {
  try {
    if (isEdit.value) {
      await tasksStore.updateTask(form.value.id, form.value)
    } else {
      await tasksStore.createTask(form.value)
    }
    emit('saved')
    cancel()
  } catch (e) {
    alert('Ошибка: ' + e.response?.data || e.message)
  }
}

const cancel = () => {
  form.value = { id: 0, name: '', projectId: projectsStore.projects[0]?.id || null, isActive: true }
  isEdit.value = false
}
</script>