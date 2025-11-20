<template>
        <v-main class="bg-grey-lighten-4">
            <v-container class="ma-8 mt-15">
                <v-row>
                    <v-col cols="8">
                        <v-card class="elevation-2 rounded-lg">
                            <v-card-title class="d-flex align center pa-4 bg-green-light">
                                <span class="text-h6 font-weight-bold text-green-primary">Modalidades</span>
                                <v-spacer></v-spacer>
                                <v-btn color="green-primary">
                                    <v-icon>mdi-import</v-icon>
                                    Importar
                                </v-btn>
                            </v-card-title>
                            <v-divider></v-divider>

                            <!-- Tabela -->

                            <v-data-table
                            :headers="headers"
                            :items="mock"
                            class="elevation-0"
                            :items-per-page="10"
                            @click:row="selectItem">

                            <template v-slot:item.id="{ item }">
                                <v-chip size="small" color="grey-lighten-1"> 
                                    #{{ item.id }}
                                </v-chip>
                            </template>

                            <template v-slot:item.nome="{ item }">
                                <div class="font-weight-medium text-body-1">{{ item.nome }}</div>
                            </template>
                            </v-data-table>

                        </v-card>
                    </v-col>

                    <v-col cols="4">
                        <!-- Coluna para detalhes -->
                         <v-card class="elevation-2 rounded-lg" style="border-top: 4px solid var(--neutral-dark);">
                            <v-card-title class="bg-green-light pa-4">
                                <v-icon color="green-dark-md" class="mr-2">mdi-information-outline</v-icon>
                                <span class="text-h6 text-green-primary font-weight-bold">Detalhes</span>
                            </v-card-title>
                            <v-divider></v-divider>

                            <v-card-text class="pa-4" v-if="selectedItem">
                                <div class="mb-4">
                                    <v-chip color="green-primary" label class="mb-3">
                                        <v-icon start>mdi-gavel</v-icon>
                                        {{ selectedItem.nome }}
                                    </v-chip>
                                </div>

                                <v-list density="compact" class="">
                                    <v-list-item class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-identifier</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">ID</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            #{{ selectedItem.id }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-information-box-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Nome</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.nome }}
                                        </v-list-item-subtitle>
                                    </v-list-item>
                                </v-list>
                            </v-card-text>

                            <v-card-text class="pa-8 text-center text-grey-darken-1" v-else>
                                <v-icon size="64" color="grey-lighten-2" class="mb-4">mdi-cursor-default-click</v-icon>
                                <div class="text-body-2">
                                    Selecione uma modalidade para ver os detalhes
                                </div>
                            </v-card-text>
                         </v-card>
                    </v-col>

                </v-row>
            </v-container>
        </v-main>

</template>

<script setup>
import { ref } from 'vue'

const selectedItem = ref(null)

const mock = [
  { "id": 1, "nome": "Concorrência" },
  { "id": 2, "nome": "Pregão Eletrônico" },
  { "id": 3, "nome": "Pregão Presencial" },
  { "id": 4, "nome": "Tomada de Preços" },
  { "id": 5, "nome": "Dispensa de Licitação" },
  { "id": 6, "nome": "Inexigibilidade" },
  { "id": 7, "nome": "Leilão" },
  { "id": 8, "nome": "Concurso" },
  { "id": 9, "nome": "Regime Diferenciado de Contratações (RDC)" },
  { "id": 10, "nome": "Diálogo Competitivo" },
  { "id": 11, "nome": "Chamamento Público" },
  { "id": 12, "nome": "Credenciamento" },
  { "id": 13, "nome": "Registro de Preços" },
  { "id": 14, "nome": "Cotação Eletrônica" },
  { "id": 15, "nome": "Manifestação de Interesse" }
]

function selectItem(event, { item }) {
    selectedItem.value = item
}
</script>