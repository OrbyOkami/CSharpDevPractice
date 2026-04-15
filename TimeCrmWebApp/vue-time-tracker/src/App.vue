<template>
  <div id="app">
    <AppHeader />
    <main class="container mx-auto px-4 py-8">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import AppHeader from '@/components/Layout/AppHeader.vue'
</script>

<style>
/* глобальные стили при необходимости */
</style>