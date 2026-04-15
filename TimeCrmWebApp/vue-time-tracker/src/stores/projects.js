import { defineStore } from 'pinia'
import api from '@/services/api'

export const useProjectsStore = defineStore('projects', {
  state: () => ({
    projects: [],
    loading: false,
    error: null
  }),
  actions: {
    async fetchProjects() {
      this.loading = true
      try {
        const response = await api.getProjects()
        this.projects = response.data
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    },
    async createProject(project) {
      const response = await api.createProject(project)
      this.projects.push(response.data)
      return response.data
    },
    async updateProject(id, project) {
      await api.updateProject(id, project)
      const index = this.projects.findIndex(p => p.id === id)
      if (index !== -1) this.projects[index] = { ...this.projects[index], ...project }
    },
    async deleteProject(id) {
      await api.deleteProject(id)
      this.projects = this.projects.filter(p => p.id !== id)
    }
  }
})