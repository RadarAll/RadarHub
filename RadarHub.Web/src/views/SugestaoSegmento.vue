<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="fluid">
            <PainelGenerico
            :carregar="carregarSugestoes"
            :headers="headers"
            :items="sugestoesSegmentos"
            >

            <Alert :loading="isLoading" :texto="mensagem.texto" :tipo="mensagem.tipo" :visivel="mensagem.visivel"></Alert>

            </PainelGenerico>
        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed} from 'vue'
import SugestaoSegmentoService from '@/servicos/SugestaoSegmentoController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Sugestão de Segmentos'
const sugestoesSegmentos = ref([]);
const headers = computed(() => {
    if (!sugestoesSegmentos.value.length) return []

    const keys = Object.keys(sugestoesSegmentos.value[0]).filter(key => key != 'id')

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

async function carregarSugestoes() {
    isLoading.value = true;

    try {
        const response = await SugestaoSegmentoService.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            sugestoesSegmentos.value = response.data || [];
            console.log('Sugestões de Segmentos carregadas:', tipos.value);
            exibirMensagem('Sugestões de Segmentos carregadas com sucesso!', 'info', 3000)
        }

    } catch (error) {
        console.error('Erro ao carregar Sugestões de Segmentos:', error);
        exibirMensagem('Erro ao carregar Sugestões de Segmentos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarSugestoes();
})
</script>