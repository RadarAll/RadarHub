<template>
    <v-main class="bg-grey-lighten-4">
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarUfs"
            :importar="importarUfs"
            :headers="headers"
            :items="ufs"
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
import UfServico from '@/servicos/UfController';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const ufs = ref([]);
const headers = computed(() => {
    if (!ufs.value.length) return []

    const keys = Object.keys(ufs.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarUfs() {
    isLoading.value = true;

    try {
        const response = await UfServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Uf's com sucesso!", 'success');
            await carregarUfs();
        }
    } catch (error) {
        console.error("Erro ao importar Uf's:", error);
        exibirMensagem("Erro ao importar Uf's.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarUfs() {
    isLoading.value = true;

    try {
        const response = await UfServico.obterTodos();

        if (response.status >= 200 && response.status < 300 ) {
            ufs.value = response.data || [];
            console.log("Uf's carregadas com sucesso:", ufs.value);
            exibirMensagem("Uf's carregadas com sucesso!", 'success', 3000);
        }
    } catch (error) {
        console.error("Erro ao carregar Uf's: ", error);
        exibirMensagem("Erro ao carregar Uf's.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarUfs();
})
</script>