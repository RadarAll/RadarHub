<script setup>
import { RouterView, useRoute } from 'vue-router'
import SideMenu from './components/SideMenu.vue'

const route = useRoute()
</script>

<template>
  <SideMenu v-if="route.path !== '/' && route.path !== '/login'" />
  <main :class="route.path !== '/' && route.path !== '/login' ? 'ml-[70px]' : ''">
    <RouterView />
  </main>
</template>

<style scoped>
main {
  min-height: 100vh;
  transition: margin-left 0.3s ease;
}
</style>
