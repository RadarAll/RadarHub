<template>
    <v-main class="bg-grey-lighten-4">
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarFontesOrcamentarias"
            :importar="importarFontesOrcamentarias"
            :headers="headers"
            :items="fontesOrcamentarias"
            />

            <Alert
            :loading="isLoading"
            :texto="mensagem.texto"
            :tipo="mensagem.tipo"
            :visivel="mensagem.visivel"
            />
        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import FonteOrcamentariaServico from '@/servicos/FonteOrcamentariaController';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const fontesOrcamentarias = ref([]);
const headers = computed(() => {
    if (!fontesOrcamentarias.value.length) return []

    const keys = Object.keys(fontesOrcamentarias.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarFontesOrcamentarias() {
    isLoading.value = true;

    try {
        const response = await FonteOrcamentariaServico.importar()

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Fontes Orçamentárias importadas com sucesso!", 'success');
            await carregarFontesOrcamentarias();
        }
    } catch (error) {
        console.error("Erro ao importar Fontes Orçamentárias:", error);
        exibirMensagem("Erro ao importar Fontes Orçamentárias.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarFontesOrcamentarias() {
    isLoading.value = true;

    try {
        const response = await FonteOrcamentariaServico.obterTodos();

        if (response.status >= 200 && response.status < 300 ) {
            fontesOrcamentarias.value = response.data || [];
            console.log("Fontes Orçamentárias carregadas com sucesso:", fontesOrcamentarias.value);
            exibirMensagem("Fontes Orçamentárias carregadas com sucesso!", 'success', 3000);
        }
    } catch (error) {
        console.error("Erro ao carregar Fontes Orçamentárias: ", error);
        exibirMensagem("Erro ao carregar Fontes Orçamentárias.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarFontesOrcamentarias();
})
</script>