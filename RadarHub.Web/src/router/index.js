import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Licitacao from '../views/Licitacao.vue'
import Orgao from '../views/Orgao.vue'
import Modalidade from '@/views/Modalidade.vue'
import Poder from '@/views/Poder.vue'
import Municipio from '@/views/Municipio.vue'
import Tipo from '@/views/Tipo.vue'
import TipoMargemPreferencia from '@/views/TipoMargemPreferencia.vue'
import Uf from '@/views/Uf.vue'
import Unidade from '@/views/Unidade.vue'
import FonteOrcamentaria from '@/views/FonteOrcamentaria.vue'
import Perfil from '@/views/Perfil.vue'
import Usuario from '@/views/Usuario.vue'
import Login from '@/views/Login.vue'
import SugestaoSegmento from '@/views/SugestaoSegmento.vue'
import Segmento from '@/views/Segmento.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login
    },
    {
      path: '/',
      name: 'home',
      component: Perfil,
      meta: { requireAuth: true }
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: Perfil,
      meta: { requireAuth: true }
    },
    {
      path: '/usuarios',
      name: 'usuario',
      component: Usuario,
      meta: { requireAuth: true }
    },
    {
      path: '/licitacoes',
      name: 'licitacoes',
      component: Licitacao,
      meta: { requireAuth: true }
    },
    {
      path: '/orgaos',
      name: 'orgaos',
      component: Orgao,
      meta: { requireAuth: true }
    },
    {
      path: '/modalidades',
      name: 'modalidade',
      component: Modalidade,
      meta: { requireAuth: true }
    },
    {
      path: '/poderes',
      name: 'poderes',
      component: Poder,
      meta: { requireAuth: true }
    },
    {
      path: '/municipios',
      name: 'municipios',
      component: Municipio,
      meta: { requireAuth: true }
    },
    {
      path: '/tipos',
      name: 'tipos',
      component: Tipo,
      meta: { requireAuth: true }
    },
    {
      path: '/tiposMargemPreferencia',
      name: 'tipoMargemPreferencia',
      component: TipoMargemPreferencia,
      meta: { requireAuth: true }
    },
    {
      path: '/ufs',
      name: 'ufs',
      component: Uf,
      meta: { requireAuth: true }
    },
    {
      path: '/unidades',
      name: 'unidades',
      component: Unidade,
      meta: { requireAuth: true }
    },
    {
      path: '/fontesOrcamentarias',
      name: 'fontesOrcamentarias',
      component: FonteOrcamentaria,
      meta: { requireAuth: true }
    },
    {
      path: '/sugestoes',
      name: 'sugestoes',
      component: SugestaoSegmento,
      meta: { requireAuth: true }
    },
    {
      path: '/segmentos',
      name: 'segmentos',
      component: Segmento,
      meta: { requireAuth: true }
    }
  ],
})

router.beforeEach((to, from, next) => {
  const logado = !!localStorage.getItem('token')

  if (to.meta.requireAuth && !logado) {
    next('/login')
  } else {
    next()
  }
})

export default router
