import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Licitacao from '../views/Licitacao.vue'
import Orgao from '../views/Orgao.vue'
import Modalidade from '@/views/Modalidade.vue'
import Poder from '@/views/Poder.vue'
import Municipio from '@/views/Municipio.vue'
import Tipo from '@/views/Tipo.vue'
import TipoMargemPreferencia from '@/views/TipoMargemPreferencia.vue'

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
      path: '/poderes',
      name: 'poderes',
      component: Poder
    },
    {
      path: '/municipios',
      name: 'municipios',
      component: Municipio
    },
    {
      path: '/tipos',
      name: 'tipos',
      component: Tipo
    },
    {
      path: '/tiposMargemPreferencia',
      name: 'tipoMargemPreferencia',
      component: TipoMargemPreferencia
    }
  ],
})

export default router
