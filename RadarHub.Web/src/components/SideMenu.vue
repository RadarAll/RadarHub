<template>
  <div>
    <!-- MENU LATERAL -->
    <aside
      class="fixed top-0 left-0 h-screen bg-white flex flex-col items-center z-40 shadow-md overflow-hidden"
      :class="isCollapsed ? 'w-[70px]' : 'w-[220px]'"
    >
      <div class="flex items-center justify-center w-full py-6">
        <img
          :src="isCollapsed ? LogoMini : Logo"
          alt="RadarHub"
          class="h-10 w-auto px-[10px] transition-all duration-200 ease-in-out"
        />
      </div>

      <!-- LINKS -->
      <nav class="flex-1 w-full flex flex-col p-3 gap-2">
        <RouterLink
          v-for="item in menuItems"
          :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 p-3 rounded-lg text-gray-700 hover:bg-[#eef0e8] transition-all duration-200 ease-in-out"
          active-class="bg-[#E5EDD8] text-white"
        >
          <component
            :is="item.icon"
            class="w-5 h-5 min-w-[20px] transition-transform duration-200 ease-in-out text-[#1f5405]"
          />
          <span
            class="whitespace-nowrap text-sm font-medium transition-all duration-200 ease-in-out overflow-hidden text-[#1f5405]"
            :style="{ maxWidth: isCollapsed ? '0px' : '220px', opacity: isCollapsed ? 0 : 1 }"
          >
            {{ item.label }}
          </span>
        </RouterLink>
      </nav>

      <!-- LOGOUT -->
      <div class="p-3 border-t border-gray-200 w-full">
        <button
          @click="logout"
          class="flex items-center gap-3 w-full p-3 rounded-lg text-gray-700 hover:bg-red-50 hover:text-red-600 transition-all duration-150 ease-in-out"
        >
          <LogOut class="w-5 h-5" />
          <span v-show="!isCollapsed" class="text-sm font-medium">Sair</span>
        </button>
      </div>
    </aside>

    <!-- BOTÃO DE SETA CENTRAL -->
    <button
      @click="toggleMenu"
      class="fixed top-1/2 -translate-y-1/2 transition-all duration-200 ease-in-out bg-white hover:bg-[#eef0e8] border border-[#1f5405] text-[#1f5405] hover:text-[#E5EDD8] p-1.5 rounded-full flex items-center justify-center z-50"
      :class="isCollapsed ? 'left-[50px]' : 'left-[200px]'"
      style="transform-origin: center;"
      title="Alternar menu"
    >
      <ChevronLeft
        class="w-5 h-5 transition-transform duration-200 ease-in-out"
        :class="{ 'rotate-180': isCollapsed }"
      />
    </button>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { Home, Archive, Box, LogOut, ChevronLeft } from 'lucide-vue-next'

const isCollapsed = ref(true)
const toggleMenu = () => (isCollapsed.value = !isCollapsed.value)

const menuItems = [
  { to: '/', label: 'Home', icon: Home },
  { to: '/licitacoes', label: 'Licitações', icon: Archive },
  { to: '/orgaos', label: 'Órgãos', icon: Box },
]

function logout() {
  console.log('Logout...')
}
</script>

<style scoped>
aside {
  transition: width 0.25s ease-in-out, box-shadow 0.25s ease-in-out;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.08);
}

button {
  transition: all 0.2s ease-in-out;
}

/* Animação discreta da seta */
button:hover svg {
  transform: scale(1.1);
}
</style>
