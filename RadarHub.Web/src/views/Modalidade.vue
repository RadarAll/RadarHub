<template>
    <v-app>
        <v-navigation-drawer permanent :width="250" class="elevation-1">
                <div class="d-flex align-center justify-center w-full py-5">
                    <v-img :src="logo" :width="40" :height="40"/>
                </div>
        </v-navigation-drawer>
        <v-app-bar class="elevation-1"></v-app-bar>
        <v-main class="bg-grey-lighten-5">
            <v-container class="ma-2 mt-15">
                <v-row>
                    <v-col cols="8">
                        <h1 class="text-green-light-md mb-5"> Modalidades de Licitações </h1>
                        <v-table class="border rounded elevation-2" density="comfortable">
                            <thead>
                                <tr>
                                    <th class="text-h6 text-green-primary font-weight-semibold">
                                        Id
                                    </th>
                                    <th class="text-h6 text-green-primary font-weight-semibold">
                                        Nome
                                    </th>
                                    <th class="text-h6 text-green-primary font-weight-semibold">
                                        Ações
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr
                                    v-for="item in desserts"
                                    :key="item.nome"
                                >
                                    <td class="text-body-1">{{ item.id }}</td>
                                    <td class="text-body-1">{{ item.nome }}</td>
                                </tr>
                            </tbody>
                        </v-table>
                    </v-col>

                    <v-col cols="4">
                        <!-- Coluna para detalhes -->
                    </v-col>

                </v-row>
            </v-container>
        </v-main>
    </v-app>

</template>

<script setup>
import logo from '@/assets/logo.png'

const desserts = [
  { id: 1, nome: 'Concorrência Pública' },
  { id: 2, nome: 'Tomada de Preços' },
  { id: 3, nome: 'Convite' },
  { id: 4, nome: 'Leilão' },
  { id: 5, nome: 'Pregão Eletrônico' },
  { id: 6, nome: 'RDC Integrado' },
]

</script>