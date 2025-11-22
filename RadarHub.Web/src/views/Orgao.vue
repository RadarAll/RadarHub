<template>
    <v-main class="bg-grey-lighten-4">
        <v-container class="mt-15 fluid">
            <PainelGenerico
            :carregar="carregarOrgaos"
            :importar="importarOrgaos"
            :headers="headers"
            :items="orgaos"
            ></PainelGenerico>

            <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading" ></Alert>

        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import OrgaoServico from '@/servicos/OrgaoController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});
const orgaos = ref([]);
const headers = computed(() => {
     if (!orgaos.value.length) return []

     const keys = Object.keys(orgaos.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarOrgaos() {
    isLoading.value = true;

    try {
        const response = await OrgaoServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Orgãos importados com sucesso!", 'success');
            await carregarOrgaos();
        }

    } catch (error) {
        console.error('Erro ao importar orgãos:', error);
        exibirMensagem('Erro ao importar orgãos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarOrgaos() {
    isLoading.value = true;

    try {
        const response = await OrgaoServico.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            orgaos.value = response.data || [];
            console.log('Orgãos carregados:', orgaos.value);
            exibirMensagem('Orgãos importados com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar orgãos:', error);
        exibirMensagem('Erro ao carregar orgãos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarOrgaos();
})

</script>