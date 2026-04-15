<template>
  <div>
    <h1 class="text-2xl font-bold mb-4">Управление проектами</h1>
    <ProjectList />
    <ProjectForm />
  </div>
</template>

<script setup>
import ProjectList from '@/components/Projects/ProjectList.vue'
import ProjectForm from '@/components/Projects/ProjectForm.vue'
</script>