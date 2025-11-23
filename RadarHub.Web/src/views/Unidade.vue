<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarUnidades"
            :importar="importarUnidades"
            :headers="headers"
            :items="unidades"
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
import UnidadeServico from '@/servicos/UnidadeController';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Unidades'
const unidades = ref([]);
const headers = computed(() => {
    if (!unidades.value.length) return []

    const keys = Object.keys(unidades.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarUnidades() {
    isLoading.value = true;

    try {
        const response = await UnidadeServico.importar()

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Unidades importadas com sucesso!", 'success');
            await carregarUfs();
        }
    } catch (error) {
        console.error("Erro ao importar Unidades:", error);
        exibirMensagem("Erro ao importar Unidades.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarUnidades() {
    isLoading.value = true;

    try {
        const response = await UnidadeServico.obterTodos();

        if (response.status >= 200 && response.status < 300 ) {
            unidades.value = response.data || [];
            console.log("Unidades carregadas com sucesso:", unidades.value);
            exibirMensagem("Unidades carregadas com sucesso!", 'success', 3000);
        }
    } catch (error) {
        console.error("Erro ao carregar Unidades: ", error);
        exibirMensagem("Erro ao carregar Unidades.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarUnidades();
})
</script>