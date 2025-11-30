<script setup>
import { RouterView, useRoute } from 'vue-router'
import NavigationDrawer from './components/NavigationDrawer.vue';
import AppBar from './components/AppBar.vue';

const route = useRoute()
</script>

<template>
  <v-app>
    <NavigationDrawer v-if="route.path !== '/'"></NavigationDrawer>
    <main :class="route.path !== '/' && route.path !== '/login' ? 'ml-[70px]' : ''">
      <RouterView />
    </main>
  </v-app>
  
</template>

<style scoped>
main {
  min-height: 100vh;
  transition: margin-left 0.3s ease;
}
</style>
