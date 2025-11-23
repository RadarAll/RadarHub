<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarPoderes"
            :importar="importarPoderes"
            :headers="headers"
            :items="poderes"
            ></PainelGenerico>

            <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading" ></Alert>

        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import PoderServico from '@/servicos/PoderController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Poderes'
const poderes = ref([]);
const headers = computed(() => {
     if (!poderes.value.length) return []

     const keys = Object.keys(poderes.value[0]).filter(key => key != 'id')

    return [
        {
            title: 'ID', key: 'id'
        },
        ...keys.map(key => ({
            title: key.charAt(0).toUpperCase() + key.slice(1), key: key
        }))
    ]
})

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

async function importarPoderes() {
    isLoading.value = true;

    try {
        const response = await PoderServico.importar();

        if (response.status >= 200 && response.status < 3000) {
            exibirMensagem("Poderes importados com sucesso!", 'success');
            await carregarPoderes();
        }

    } catch (error) {
        console.error('Erro ao improtar poderes:', error);
        exibirMensagem('Erro ao importar poderes.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarPoderes() {
    isLoading.value = true;

    try {
        const response = await PoderServico.obterTodos();

        if (response.status >= 200 && response.status < 3000) {
            poderes.value = response.data || [];
            console.log('Poderes carregados:', poderes.value);
        }
        exibirMensagem('Poderes importados com sucesso!', 'success', 3000)

    } catch (error) {
        console.error('Erro ao carregar poderes:', error);
        exibirMensagem('Erro ao carregar poderes.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarPoderes();
})

</script>