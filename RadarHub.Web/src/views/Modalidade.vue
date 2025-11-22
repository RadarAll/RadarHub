<template>
        <v-main class="bg-grey-lighten-4">
            <v-container class="mt-15 fluid">
                <v-row class="mt-16">
                    <v-col cols="12" md="8" class="d-flex justify-space-between">
                        <v-btn @click="carregarModalidadesModalidades" color="blue-primary">
                            <v-icon class="mdi-spin">mdi-sync</v-icon>
                            Carregar
                        </v-btn>
                        <v-btn @click="importarModalidades" color="green-primary">
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
                            :items="modalidades"
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

                            <template v-slot:item.nome="{ item }">
                                <div class="font-weight-medium text-body-1">{{ item.nome }}</div>
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
                <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading"></Alert>

            </v-container>
        </v-main>

</template>

<script setup>
import { ref, onMounted } from 'vue'
import ModalidadeServico from '@/servicos/ModalidadeController'
import Alert from '@/components/Alert.vue';

const selectedItem = ref(null);
const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});
const modalidades = ref([]);
const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Nome', key: 'nome' },
  {title: 'Ações', key: 'acoes'}
];

function selectItem(event, { item }) {
    selectedItem.value = item
}

function exibirMensagem(texto, tipo = 'info', duracao = 3000) {
    mensagem.value = {
        visivel: true,
        tipo,
        texto
    }

    setTimeout(() => {
    mensagem.value.visivel = false
}, duracao)
}

async function importarModalidades() {
    isLoading.value = true;

    try {
        const response = await ModalidadeServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Modalidades importadas com sucesso!", 'success');
            await carregarModalidades();
        }
    } catch (error) {
        console.error('Erro ao importar modalidades: ', error);
        exibirMensagem('Erro ao importar modalidades.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarModalidades() {
    isLoading.value = true;

    try {
        const response = await ModalidadeServico.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            modalidades.value = response.data || [];
        }
        exibirMensagem('Modalidades importadas com sucesso!', 'success', 3000)

    } catch (error) {
        console.error('Erro ao carregar modalidades:', error);
        exibirMensagem('Erro ao carregar modalidades: ', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarModalidades();
});
</script>