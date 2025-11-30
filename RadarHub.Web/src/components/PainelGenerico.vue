<template>
                <v-row class="mt-16">
                    <v-col cols="12" md="8" class="d-flex justify-space-between">
                        <v-btn @click="carregar" color="green-light-md" class="text-neutral-light">
                            <v-icon>mdi-sync</v-icon>
                            Carregar
                        </v-btn>
                        <v-btn v-if="importar" @click="importar" color="green-primary">
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
                            v-if="items.length > 0"
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

                            <template v-slot:item.ativo="{ item }">
                                <v-chip :color="item.ativo ? 'green-primary' : 'red-primary'"> 
                                    <v-icon>
                                        {{ item.ativo ? 'mdi-check-bold' : 'mdi-close-thick' }}
                                    </v-icon>
                                </v-chip>
                            </template>

                            <template v-for="header in headers.filter(h => h.key != 'id' && h.key != 'ativo')" v-slot:[`item.${header.key}`]="{ item }" :key="header.key">
                                <div v-if="header.key != 'id'" class="font-weight-medium text-body-1">{{ item[header.key] }}</div>
                            </template>
                            
                            </v-data-table>

                            <v-data-table v-else
                            class="elevation-0 pa-8"
                            >
                                <v-card-text class="d-flex flex-column justify-center align-center pa-16 text-center text-grey-darken-1">
                                <v-icon size="70" color="grey-lighten-2" class="mb-4">mdi-database-off</v-icon>
                                <div class="text-body-2">
                                    Não há registro no banco de dados.
                                </div>
                            </v-card-text>
                            </v-data-table>

                        </v-card>
                    </v-col>

                    <v-col cols="4">
                        <!-- Coluna para detalhes -->
                         <v-card v-if="selectedItem" class="elevation-2 rounded-lg"  style="border-top: 4px solid var(--neutral-dark);">
                            <v-card-title class="bg-green-light pa-4 w-100">
                                <v-icon color="green-dark-md" class="mr-2">mdi-information-outline</v-icon>
                                <span class="text-h6 text-green-primary font-weight-bold">Detalhes</span>
                            </v-card-title>
                            <v-divider></v-divider>

                            <v-card-text class="d-block text-start pa-4 py-2">

                                <div v-if="selectedItem.nomeCompleto" class="mb-4">
                                    <v-chip color="green-primary" label class="mb-3">
                                        <v-icon start>mdi-account</v-icon>
                                        {{ selectedItem.nomeCompleto }}
                                    </v-chip>
                                </div>

                                <div v-if="selectedItem.nome" class="mb-4">
                                    <v-chip color="green-primary" label class="mb-3">
                                        <v-icon start>mdi-gavel</v-icon>
                                        {{ selectedItem.nome }}
                                    </v-chip>
                                </div>

                                <div v-if="selectedItem.nomeSugerido" class="mb-4">
                                    <v-chip color="green-primary" label class="mb-3">
                                        <v-icon start>mdi-gavel</v-icon>
                                        {{ selectedItem.nomeSugerido }}
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

                                    <v-list-item v-if="selectedItem.nome" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-information-box-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Nome</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.nome }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.confiancaPercentual" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-percent</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Percentual</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.confiancaPercentual }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.nomeCompleto" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-information-box-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Nome</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.nomeCompleto }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.email" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-email</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Nome</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.email }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.descricaoSugerida" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-text-box-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Descrição Sugerida</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.descricaoSugerida }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.licitacaoIds" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-identifier</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Licitações Id's</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            #{{ selectedItem.licitacaoIds }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.quantidadeLicitacoesOriginarias" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-gavel</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Licitações Originárias</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            #{{ selectedItem.quantidadeLicitacoesOriginarias }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.palavrasChaveSugeridas" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-key-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Palavras Chaves Sugeridas</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            #{{ selectedItem.palavrasChaveSugeridas }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.senhaHash" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-lock</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Senha Hash</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                                {{ selectedItem.senhaHash }}
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

                                    <v-list-item v-if="selectedItem.idTerceiro" class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-account-key-outline</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Id Terceiro</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.idTerceiro }}
                                        </v-list-item-subtitle>
                                    </v-list-item>

                                    <v-list-item v-if="selectedItem.dataDesativacao " class="px-0">
                                        <template v-slot:prepend>
                                            <v-icon color="grey-darken-1">mdi-timer-off</v-icon>
                                        </template>
                                        <v-list-item-title class="text-caption text-grey-darken-1">Data desativação</v-list-item-title>
                                        <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                            {{ selectedItem.dataDesativacao }}
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
                                    Selecione um registro para ver os detalhes
                                </div>
                            </v-card-text>
                         </v-card>
               
                    </v-col>
                </v-row>

                <v-row v-if="selectedItem && route.path === '/sugestoes' ">
                    <v-col cols="8"  class="d-flex align-center justify-center">
                        <v-btn @click="aprovar" color="success" class="mr-5 text-neutral-light"><v-icon>mdi-check</v-icon> Aprovar</v-btn>
                        <v-btn color="red-dark-md"><v-icon>mdi-close</v-icon> Rejeitar</v-btn>
                    </v-col>
                </v-row>

</template>

<script setup>
import { ref,} from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'

const route = useRoute()

const selectedItem = ref(null);

defineProps({
    carregar: {
        type: Function,
        required: true
    },
    importar: {
        type: Function,
        required: false
    },
    headers: {
        type: Array,
        required: true
    },
    items: {
        type: Array,
        required: true
    }
})

async function aprovar() {
    try {
        const response = await axios.post(`https://localhost:7203/api/SugestoesSegmento/${selectedItem.value.id}/aprovar`,
            { usuarioRevisao: localStorage.getItem('emailUsuario') }
        )

        if (response.status >= 200 && response.status < 300) {
            console.log('Sugestão aprovada com sucesso!');
            window.location.reload()

        }
    } catch (error) {
        console.log('Erro ao aprovar Sugestão:', error);
    }
}

function selectItem(event, { item }) {
    selectedItem.value = item
}



</script>