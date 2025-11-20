import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Licitacao from '../views/Licitacao.vue'
import Orgao from '../views/Orgao.vue'
import Modalidade from '@/views/Modalidade.vue'
import Vezero from '@/views/vezero.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/licitacoes',
      name: 'licitacoes',
      component: Licitacao
    },
    {
      path: '/orgaos',
      name: 'orgaos',
      component: Orgao
    },
    {
      path: '/modalidades',
      name: 'modalidade',
      component: Modalidade
    },
    {
      path: '/vezero',
      name: 'v0',
      component: Vezero
    },
  ],
})

export default router
