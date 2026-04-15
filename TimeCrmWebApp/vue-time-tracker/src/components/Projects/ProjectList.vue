<template>
  <div class="mb-8">
    <h2 class="text-xl mb-2">Список проектов</h2>
    <div v-if="store.loading">Загрузка...</div>
    <table v-else class="min-w-full border">
      <thead>
        <tr>
          <th>Название</th>
          <th>Код</th>
          <th>Активен</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="project in store.projects" :key="project.id">
          <td>{{ project.name }}</td>
          <td>{{ project.code }}</td>
          <td>{{ project.isActive ? 'Да' : 'Нет' }}</td>
          <td>
            <button @click="$emit('edit', project)" class="text-blue-600 mr-2">Ред.</button>
            <button @click="handleDelete(project.id)" class="text-red-600">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useProjectsStore } from '@/stores/projects'

const store = useProjectsStore()
const emit = defineEmits(['edit'])

onMounted(() => {
  store.fetchProjects()
})

const handleDelete = async (id) => {
  if (confirm('Удалить проект?')) {
    try {
      await store.deleteProject(id)
    } catch (e) {
      alert('Ошибка удаления: ' + e.response?.data || e.message)
    }
  }
}
</script>