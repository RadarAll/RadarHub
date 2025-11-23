<template>
        <v-navigation-drawer permanent :width="200" class="elevation-1">
                <div class="d-flex align-center justify-center w-full py-8">
                    <v-img :src="logo" :width="40" :height="40"/>
                </div>

                <v-divider class="mb-4"></v-divider>

                <v-list density="comfortable">
                        <v-list-item class="mb-1 pl-8" to="/">
                                <div class="d-flex flex-row align-center ga-3">
                                        <v-icon color="green-primary">mdi-home-variant-outline</v-icon>
                                        <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Dashboard</span>
                                </div>
                        </v-list-item>

                        <v-list-item class="mb-1 pl-8" to="/eventos">
                                        <div class="d-flex flex-row align-center ga-3">
                                                <v-icon color="green-primary">mdi-calendar-blank-outline</v-icon>
                                                <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Eventos</span>
                                        </div>
                        </v-list-item>

                        <v-list-item class="mb-1 pl-8" to="/usuarios">
                                 <div class="d-flex flex-row align-center ga-3">
                                        <v-icon color="green-primary">mdi-account</v-icon>
                                        <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Usuarios</span>
                                </div>
                        </v-list-item>

                        <v-list-item class="mb-1 pl-8" to="/licitacoes">
                                <div class="d-flex flex-row align-center ga-3">
                                        <v-icon color="green-primary">mdi-gavel</v-icon>
                                        <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Licitações</span>
                                </div>
                        </v-list-item>

                        <v-list-item class="mb-1 pl-8" to="/historico">
                                <div class="d-flex flex-row align-center ga-3">
                                        <v-icon color="green-primary">mdi-history</v-icon>
                                        <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Histórico</span>
                                </div>
                        </v-list-item>

                        <v-list-item class="mb-1 pl-8" link>
                                        <v-menu transition="slide-y-transition" class="pa-0">
                                                <template v-slot:activator="{ props }">
                                                        <div v-bind="props" class="d-flex flex-row text-decoration-none ga-3 w-full">
                                                                <v-icon color="green-primary">mdi-filter-variant</v-icon>
                                                                <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Filtros</span>
                                                        </div>
                                                </template>
                                                <v-list density="compact" class="pa-0 bg-green-light">                         
                                                        <template v-for="(filtro, index) in filtros" :key="filtro.label">
                                                                <v-list-item :to="filtro.rota">
                                                                        <v-list-item-title class="text-caption text-green-dark-md font-weight-medium">{{ filtro.nome }}</v-list-item-title>
                                                                </v-list-item>
                                                                <v-divider></v-divider>
                                                        </template>
                                                </v-list>   
                                        </v-menu>               
                        </v-list-item>
                </v-list>


                
        </v-navigation-drawer>
</template>

<script setup>
import logo from '@/assets/logo.png'

const filtros = [
        {nome: 'Modalidade', rota: '/modalidades'},
        {nome: 'Municipio', rota: '/municipios'},
        {nome: 'Orgão', rota: '/orgaos'},
        {nome: 'Poder', rota: '/poderes'},
        {nome: 'Tipo', rota: '/tipos'},
        {nome: 'Tipo Margem Preferência', rota: '/tiposMargemPreferencia'},
        {nome: 'Uf', rota: '/ufs'},
        {nome: 'Unidade', rota: '/unidades'},
        {nome: 'Fonte Orçamentaria', rota: '/fontesOrcamentarias'}
]
</script>