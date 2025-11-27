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

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Perfil
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: Perfil
    },
    {
      path: '/usuarios',
      name: 'usuario',
      component: Usuario
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
    },
    {
      path: '/ufs',
      name: 'ufs',
      component: Uf
    },
    {
      path: '/unidades',
      name: 'unidades',
      component: Unidade
    },
    {
      path: '/fontesOrcamentarias',
      name: 'fontesOrcamentarias',
      component: FonteOrcamentaria
    }
  ],
})

export default router
