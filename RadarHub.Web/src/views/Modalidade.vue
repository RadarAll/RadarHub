<template>
    <v-app>
        <v-navigation-drawer permanent :width="200" class="elevation-1">
                <div class="d-flex align-center justify-center w-full py-5">
                    <v-img :src="logo" :width="40" :height="40"/>
                </div>
        </v-navigation-drawer>
        <v-app-bar class="elevation-1"></v-app-bar>
        <v-main>
            <v-container class="ma-2">
                <h1 class="text-green-primary">
                    Modalidades de Licitações
                </h1>
                <v-data-table>
                    <template #top>
                        <v-toolbar color="green-light" flat>
                            <v-toolbar-title class="">Licitações</v-toolbar-title>
                        </v-toolbar>
                    </template> 
                </v-data-table>
                
            </v-container>
        </v-main>
    </v-app>

</template>

<script setup>
import logo from '@/assets/logo.png'

const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Órgão', key: 'orgao' },
  { title: 'Valor', key: 'valor' },
  { title: 'Data', key: 'data' },
]

const items = [
  { id: 1, orgao: 'Prefeitura de Marília', valor: 'R$ 120.000', data: '10/11/2025' },
  { id: 2, orgao: 'Governo SP', valor: 'R$ 250.000', data: '12/11/2025' },
  { id: 3, orgao: 'Ministério da Saúde', valor: 'R$ 1.000.000', data: '15/11/2025' },
]

</script>