<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container fluid>
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
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';
import axios from 'axios';

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

    const keys = Object.keys(sugestoesSegmentos.value[0]).filter(key => key != 'id' && key != 'descricaoSugerida' && key != 'origem' && key != 'criadoEm' && key != 'ultimaAlteracao' && key != 'licitacaoIds' && key != 'segmentoIdGerado' && key != 'motivoRejeicao' && key != 'dataRevisao' && key != 'usuarioRevisao')

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
        const response = await axios.get('https://localhost:7203/api/SugestoesSegmento/pendentes')

        if (response.status >= 200 && response.status < 300) {
            sugestoesSegmentos.value = response.data.sugestoes || [];
            console.log('Sugestões de Segmentos carregadas:', sugestoesSegmentos.value);
            exibirMensagem('Sugestões de Segmentos carregadas com sucesso!', 'success', 3000)
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