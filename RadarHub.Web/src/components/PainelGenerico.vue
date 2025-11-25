<template>
                <v-row class="mt-16">
                    <v-col cols="12" md="8" class="d-flex justify-space-between">
                        <v-btn @click="carregar" color="blue-primary">
                            <v-icon class="mdi-spin">mdi-sync</v-icon>
                            Carregar
                        </v-btn>
                        <v-btn @click="importar" color="green-primary">
                            <v-icon>mdi-import</v-icon>
                            Importar
                        </v-btn>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12" md="8">
                        <v-card class="elevation-2 rounded-lg h-100">
                            <!-- Tabela -->

                            <v-data-table
                            :headers="headers"
                            :items="items"
                            class="elevation-0"
                            :items-per-page="10"
                            density="comfortable"
                            @click:row="selectItem"
                            style="border-top: 4px solid var(--neutral-dark)">
                            <template v-slot:item.id="{ item }">
                                <v-chip size="small" color="grey-lighten-1"> 
                                    #{{ item.id }}
                                </v-chip>
                            </template>

                            <template v-for="header in headers.filter(h => h.key != 'id')" v-slot:[`item.${header.key}`]="{ item }" :key="header.key">
                                <div v-if="header.key != 'id'" class="font-weight-medium text-body-1">{{ item[header.key] }}</div>
                            </template>
                            </v-data-table>

                        </v-card>
                    </v-col>

                    <v-col cols="4">
                        <!-- Coluna para detalhes -->
                         <v-card v-if="selectedItem" class="elevation-2 rounded-lg h-100"  style="border-top: 4px solid var(--neutral-dark);">
                            <v-card-title class="bg-green-light pa-4 w-100">
                                <v-icon color="green-dark-md" class="mr-2">mdi-information-outline</v-icon>
                                <span class="text-h6 text-green-primary font-weight-bold">Detalhes</span>
                            </v-card-title>
                            <v-divider></v-divider>

                            <v-card-text class="d-block text-start pa-4">
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

                                    <v-list-item v-if="selectedItem.cnpj" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-domain</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">CNPJ</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.cnpj }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-calendar-plus</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Criado em</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.criadoEm }}
                                        </v-list-item-subtitle>

                                    </v-list-item>

                                    <v-list-item class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-clock-edit-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Ultima Alteração</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.ultimaAlteracao }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-account-key-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Id Terceiro</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.idTerceiro }}
                                        </v-list-item-subtitle>
                                        
                                    </v-list-item>

                                </v-list>
                            </v-card-text>
                         </v-card>

                         <v-card v-else class="elevation-2 rounded-lg h-100 d-flex flex-column justify-center align-center" style="border-top: 4px solid var(--neutral-dark);">
                            <v-card-title class="bg-green-light pa-4 w-100">
                                <v-icon color="green-dark-md" class="mr-2">mdi-information-outline</v-icon>
                                <span class="text-h6 text-green-primary font-weight-bold">Detalhes</span>
                            </v-card-title>
                            <v-divider></v-divider>

                            <v-card-text class="d-flex flex-column justify-center align-center pa-8 text-center text-grey-darken-1">
                                <v-icon size="64" color="grey-lighten-2" class="mb-4">mdi-cursor-default-click</v-icon>
                                <div class="text-body-2">
                                    Selecione uma modalidade para ver os detalhes
                                </div>
                            </v-card-text>
                         </v-card>
               
                    </v-col>
                </v-row>
</template>

<script setup>
import { ref,} from 'vue'

const selectedItem = ref(null);

defineProps({
    carregar: Function,
    importar: Function,
    headers: Array,
    items: Array,
})

function selectItem(event, { item }) {
    selectedItem.value = item
}



</script>