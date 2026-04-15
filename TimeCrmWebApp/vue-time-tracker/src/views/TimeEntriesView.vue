<template>
  <div>
    <h1 class="text-2xl font-bold mb-4">Учет рабочего времени</h1>
    <TimeEntryList />
    <TimeEntryForm @saved="refreshList" />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import TimeEntryList from '@/components/TimeEntries/TimeEntryList.vue'
import TimeEntryForm from '@/components/TimeEntries/TimeEntryForm.vue'

const listKey = ref(0)
const refreshList = () => listKey.value++
</script>