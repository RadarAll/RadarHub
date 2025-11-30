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
      path: '/',
      name: 'login',
      component: Login
    },
    {
      path: '/home',
      name: 'home',
      component: Perfil,
      meta: { requiresAuth: true }
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: Perfil,
      meta: { requiresAuth: true }
    },
    {
      path: '/usuarios',
      name: 'usuario',
      component: Usuario,
      meta: { requiresAuth: true }
    },
    {
      path: '/licitacoes',
      name: 'licitacoes',
      component: Licitacao,
      meta: { requiresAuth: true }
    },
    {
      path: '/orgaos',
      name: 'orgaos',
      component: Orgao,
      meta: { requiresAuth: true }
    },
    {
      path: '/modalidades',
      name: 'modalidade',
      component: Modalidade,
      meta: { requiresAuth: true }
    },
    {
      path: '/poderes',
      name: 'poderes',
      component: Poder,
      meta: { requiresAuth: true }
    },
    {
      path: '/municipios',
      name: 'municipios',
      component: Municipio,
      meta: { requiresAuth: true }
    },
    {
      path: '/tipos',
      name: 'tipos',
      component: Tipo,
      meta: { requiresAuth: true }
    },
    {
      path: '/tiposMargemPreferencia',
      name: 'tipoMargemPreferencia',
      component: TipoMargemPreferencia,
      meta: { requiresAuth: true }
    },
    {
      path: '/ufs',
      name: 'ufs',
      component: Uf,
      meta: { requiresAuth: true }
    },
    {
      path: '/unidades',
      name: 'unidades',
      component: Unidade,
      meta: { requiresAuth: true }
    },
    {
      path: '/fontesOrcamentarias',
      name: 'fontesOrcamentarias',
      component: FonteOrcamentaria,
      meta: { requiresAuth: true }
    },
    {
      path: '/sugestoes',
      name: 'sugestoes',
      component: SugestaoSegmento,
      meta: { requiresAuth: true }
    },
    {
      path: '/segmentos',
      name: 'segmentos',
      component: Segmento,
      meta: { requiresAuth: true }
    }
  ],
})

router.beforeEach((to, from, next) => {
  const logado = !!localStorage.getItem('token')

  if (to.meta.requiresAuth && !logado) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router
