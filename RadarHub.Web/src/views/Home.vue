<template>
  <div class="p-8 bg-neutral-100 min-h-screen">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-neutral-800 tracking-tight text-green-primary">Bem-vindo ao RadarHub</h1>
    </div>

    <div class="bg-white border border-neutral-200 rounded-2xl shadow-sm p-8">
      <div class="grid grid-cols-2 gap-6">
        <RouterLink
          to="/licitacoes"
          class="p-6 rounded-lg border border-neutral-200 hover:shadow-md transition-all duration-200 cursor-pointer"
        >
          <div class="text-green-primary text-3xl mb-2">📋</div>
          <h3 class="font-semibold text-neutral-800">Licitações</h3>
          <p class="text-neutral-600 text-sm">Visualizar e importar licitações</p>
        </RouterLink>

        <RouterLink
          to="/orgaos"
          class="p-6 rounded-lg border border-neutral-200 hover:shadow-md transition-all duration-200 cursor-pointer"
        >
          <div class="text-green-primary text-3xl mb-2">🏛️</div>
          <h3 class="font-semibold text-neutral-800">Órgãos</h3>
          <p class="text-neutral-600 text-sm">Visualizar e importar órgãos</p>
        </RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>

<style scoped>
</style>
