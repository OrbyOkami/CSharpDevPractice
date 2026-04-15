<template>
  <div>
    <h1 class="text-2xl font-bold mb-4">Управление задачами</h1>
    <TaskList @edit="taskToEdit = $event" />
    <TaskForm :task-to-edit="taskToEdit" @saved="taskToEdit = null" />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import TaskList from '@/components/Tasks/TaskList.vue'
import TaskForm from '@/components/Tasks/TaskForm.vue'

const taskToEdit = ref(null)
</script>