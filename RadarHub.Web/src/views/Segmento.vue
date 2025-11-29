<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container fluid>
            <PainelGenerico
            :carregar="carregarSegmentos"
            :headers="headers"
            :items="segmentos"
            ></PainelGenerico>

            <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading" ></Alert>

        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import SegmentoServico from '@/servicos/SegmentoController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Segmentos'
const segmentos = ref([]);
const headers = computed(() => {
     if (!segmentos.value.length) return []

     const keys = Object.keys(segmentos.value[0]).filter(key => key != 'id')

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

async function carregarSegmentos() {
    isLoading.value = true;

    try {
        const response = await SegmentoServico.obterTodos()

        if (response.status >= 200 && response.status < 300) {
            segmentos.value = response.data || [];
            console.log('Segmentos carregados:', segmentos.value);
            exibirMensagem('Segmentos carregados com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar segmentos:', error);
        exibirMensagem('Erro ao carregar segmentos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarSegmentos();
})

</script>