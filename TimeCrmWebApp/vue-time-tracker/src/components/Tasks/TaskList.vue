<template>
  <div class="mb-8">
    <h2 class="text-xl mb-2">Список задач</h2>
    <div v-if="store.loading">Загрузка...</div>
    <table v-else class="min-w-full border">
      <thead>
        <tr>
          <th>Название</th>
          <th>Проект</th>
          <th>Активна</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="task in store.tasks" :key="task.id">
          <td>{{ task.name }}</td>
          <td>{{ task.project?.name || '—' }}</td>
          <td>{{ task.isActive ? 'Да' : 'Нет' }}</td>
          <td>
            <button @click="$emit('edit', task)" class="text-blue-600 mr-2">Ред.</button>
            <button @click="handleDelete(task.id)" class="text-red-600">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useTasksStore } from '@/stores/tasks'

const store = useTasksStore()
defineEmits(['edit'])

onMounted(() => {
  store.fetchTasks()
})

const handleDelete = async (id) => {
  if (confirm('Удалить задачу?')) {
    try {
      await store.deleteTask(id)
    } catch (e) {
      alert('Ошибка удаления: ' + e.response?.data || e.message)
    }
  }
}
</script>