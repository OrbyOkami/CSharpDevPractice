import { defineStore } from 'pinia'
import api from '@/services/api'

export const useTasksStore = defineStore('tasks', {
  state: () => ({
    tasks: [],
    activeTasks: [],
    loading: false,
    error: null
  }),
  actions: {
    async fetchTasks() {
      this.loading = true
      try {
        const response = await api.getTasks()
        this.tasks = response.data
        this.activeTasks = this.tasks.filter(t => t.isActive)
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    },
    async fetchActiveTasks() {
      const response = await api.getActiveTasks()
      this.activeTasks = response.data
    },
    async createTask(task) {
      const response = await api.createTask(task)
      this.tasks.push(response.data)
      if (response.data.isActive) this.activeTasks.push(response.data)
      return response.data
    },
    async updateTask(id, task) {
      await api.updateTask(id, task)
      const index = this.tasks.findIndex(t => t.id === id)
      if (index !== -1) {
        this.tasks[index] = { ...this.tasks[index], ...task }
        // обновить activeTasks
        this.activeTasks = this.tasks.filter(t => t.isActive)
      }
    },
    async deleteTask(id) {
      await api.deleteTask(id)
      this.tasks = this.tasks.filter(t => t.id !== id)
      this.activeTasks = this.activeTasks.filter(t => t.id !== id)
    }
  }
})