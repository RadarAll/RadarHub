<template>
    <v-main class="bg-grey-lighten-4">
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarTiposMargemPreferencia"
            :importar="importarTiposMargemPreferencia"
            :headers="headers"
            :items="tiposMargemPreferencia"
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
import TipoMargemPreferenciaServico from '@/servicos/TipoMargemPreferenciaController'; 
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const tiposMargemPreferencia = ref([]);
const headers = computed(() => {
    if (!tiposMargemPreferencia.value.length) return []

    const keys = Object.keys(tiposMargemPreferencia.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarTiposMargemPreferencia() {
    isLoading.value = true;

    try {
        const response = await TipoMargemPreferenciaServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem('Tipos Margem Preferência importadas com sucesso!', 'success');
            await carregarTiposMargemPreferencia();
        }
    } catch (error) {
        console.error('Erro ao importar Tipos Margem Preferência:', error);
        exibirMensagem('Erro ao importar Tipos Margem Preferência.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarTiposMargemPreferencia() {
    isLoading.value = true;

    try {
        const response = await TipoMargemPreferenciaServico.obterTodos();

        if (response.status >= 200 && response.status < 300 ) {
            tiposMargemPreferencia.value = response.data || [];
            console.log('Tipos Margem Preferência carregados:', tiposMargemPreferencia.value);
            exibirMensagem('Tipos Margem Preferência carregados com sucesso!', 'success', 3000);
        }
    } catch (error) {
        console.error('Erro ao carregar Tipos Margem Preferência:', error);
        exibirMensagem('Erro ao carregar Tipos Margem Preferência', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarTiposMargemPreferencia();
})
</script>