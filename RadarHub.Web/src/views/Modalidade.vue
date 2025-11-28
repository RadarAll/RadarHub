<template>
        <v-main class="bg-grey-lighten-4">
            <AppBar :titulo="titulo"></AppBar>
            <v-container fluid>
                <PainelGenerico
                :carregar="carregarModalidades"
                :importar="importarModalidades"
                :headers="headers"
                :items="modalidades"
                >

                </PainelGenerico>
                <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading"></Alert>

            </v-container>
        </v-main>

</template>

<script setup>
import { ref, onMounted } from 'vue'
import ModalidadeServico from '@/servicos/ModalidadeController'
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Modalidades'
const modalidades = ref([]);
const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Nome', key: 'nome' },
  {title: 'Ações', key: 'acoes'}
];
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
            console.log("Modalidades carregadas:", modalidades.value);
            exibirMensagem('Modalidades carregadas com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar modalidades:', error);
        exibirMensagem('Erro ao carregar modalidades.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarModalidades();
    
});
</script>