<template>
    <v-main class="bg-grey-lighten-4">
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarMunicipios"
            :importar="importarMunicipios"
            :headers="headers"
            :items="municipios"
            ></PainelGenerico>

            <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading" ></Alert>

        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import MunicipioServico from '@/servicos/MunicipioController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});
const municipios = ref([]);
const headers = computed(() => {
     if (!municipios.value.length) return []

     const keys = Object.keys(municipios.value[0]).filter(key => key != 'id' && key!= 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarMunicipios() {
    isLoading.value = true;

    try {
        const response = await MunicipioServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Municipios importados com sucesso!", 'success');
            await carregarMunicipios();
        }

    } catch (error) {
        console.error('Erro ao importar municipios:', error);
        exibirMensagem('Erro ao importar municipios.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarMunicipios() {
    isLoading.value = true;

    try {
        const response = await MunicipioServico.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            municipios.value = response.data || [];
            console.log('Municipios carregados:', municipios.value);
            exibirMensagem('Municipios importados com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar municipios:', error);
        exibirMensagem('Erro ao carregar municipios.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarMunicipios();
})

</script>