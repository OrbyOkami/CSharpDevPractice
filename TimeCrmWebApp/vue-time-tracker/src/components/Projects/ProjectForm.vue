<template>
  <div class="border p-4 rounded">
    <h3 class="text-lg mb-3">{{ isEdit ? 'Редактировать' : 'Создать' }} проект</h3>
    <form @submit.prevent="submit">
      <div class="mb-3">
        <label class="block">Название</label>
        <input v-model="form.name" class="border p-2 w-full" required />
      </div>
      <div class="mb-3">
        <label class="block">Код</label>
        <input v-model="form.code" class="border p-2 w-full" required />
      </div>
      <div class="mb-3">
        <label class="inline-flex items-center">
          <input type="checkbox" v-model="form.isActive" />
          <span class="ml-2">Активен</span>
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
import { ref, watch } from 'vue'
import { useProjectsStore } from '@/stores/projects'

const props = defineProps({
  projectToEdit: { type: Object, default: null }
})
const emit = defineEmits(['saved'])

const store = useProjectsStore()
const isEdit = ref(false)
const form = ref({
  id: 0,
  name: '',
  code: '',
  isActive: true
})

watch(() => props.projectToEdit, (newVal) => {
  if (newVal) {
    isEdit.value = true
    form.value = { ...newVal }
  } else {
    isEdit.value = false
    form.value = { id: 0, name: '', code: '', isActive: true }
  }
}, { immediate: true })

const submit = async () => {
  try {
    if (isEdit.value) {
      await store.updateProject(form.value.id, form.value)
    } else {
      await store.createProject(form.value)
    }
    emit('saved')
    cancel()
  } catch (e) {
    alert('Ошибка: ' + e.response?.data || e.message)
  }
}

const cancel = () => {
  form.value = { id: 0, name: '', code: '', isActive: true }
  isEdit.value = false
}
</script>