<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarTipos"
            :importar="importarTipos"
            :headers="headers"
            :items="tipos"
            ></PainelGenerico>

            <Alert :loading="isLoading" :texto="mensagem.texto" :tipo="mensagem.tipo" :visivel="mensagem.visivel"></Alert>
        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed} from 'vue'
import TipoServico from '@/servicos/TipoController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Tipos'
const tipos = ref([]);
const headers = computed(() => {
    if (!tipos.value.length) return []

    const keys = Object.keys(tipos.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarTipos() {
    isLoading.value = true;

    try {
        const response = await TipoServico.importar();
        if (response.status >= 200 && response.status < 300) {
            exibirMensagem('Tipos importados com sucesso!', 'success', 3000);
            await carregarTipos();
        }

    } catch (error) {
        console.error('Erro ao importar tipos:', error);
        exibirMensagem('Erro ao importar tipos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarTipos() {
    isLoading.value = true;

    try {
        const response = await TipoServico.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            tipos.value = response.data || [];
            console.log('Tipos carregados:', tipos.value);
            exibirMensagem('Tipos carregados com sucesso!', 'info', 3000)
        }

    } catch (error) {
        console.error('Erro ao carregar tipos:', error);
        exibirMensagem('Erro ao carregar tipos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarTipos();
})
</script>