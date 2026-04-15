import axios from 'axios'

const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Интерсептор ответа: пробрасываем понятную ошибку
apiClient.interceptors.response.use(
  response => response,
  error => {
    let message = 'Произошла ошибка'
    if (error.response) {
      const data = error.response.data
      if (data.errors) {
        // Ошибки валидации (словарь)
        const messages = Object.values(data.errors).flat().join(', ')
        message = messages || data.title || 'Ошибка валидации'
      } else if (data.title) {
        message = data.title
      } else if (typeof data === 'string') {
        message = data
      }
    } else if (error.request) {
      message = 'Нет ответа от сервера'
    } else {
      message = error.message
    }
    return Promise.reject(new Error(message))
  }
)

export default {
  // Projects
  getProjects() {
    return apiClient.get('/projects')
  },
  getProject(id) {
    return apiClient.get(`/projects/${id}`)
  },
  createProject(project) {
    return apiClient.post('/projects', project)
  },
  updateProject(id, project) {
    return apiClient.put(`/projects/${id}`, project)
  },
  deleteProject(id) {
    return apiClient.delete(`/projects/${id}`)
  },

  // Tasks
  getTasks() {
    return apiClient.get('/tasks')
  },
  getActiveTasks() {
    return apiClient.get('/tasks/active')
  },
  getTask(id) {
    return apiClient.get(`/tasks/${id}`)
  },
  createTask(task) {
    return apiClient.post('/tasks', task)
  },
  updateTask(id, task) {
    return apiClient.put(`/tasks/${id}`, task)
  },
  deleteTask(id) {
    return apiClient.delete(`/tasks/${id}`)
  },

  // TimeEntries
  getTimeEntries(params) {
    return apiClient.get('/timeentries', { params })
  },
  getTimeEntry(id) {
    return apiClient.get(`/timeentries/${id}`)
  },
  createTimeEntry(entry) {
    return apiClient.post('/timeentries', entry)
  },
  updateTimeEntry(id, entry) {
    return apiClient.put(`/timeentries/${id}`, entry)
  },
  deleteTimeEntry(id) {
    return apiClient.delete(`/timeentries/${id}`)
  },
  getDailySummary(date) {
    return apiClient.get('/timeentries/daily-summary', { params: { date } })
  }
}