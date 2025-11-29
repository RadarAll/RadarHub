<template>
  <v-main class="bg-grey-lighten-4">
    <AppBar :titulo="titulo"></AppBar>
    <v-container fluid>
      <PainelGenerico
      :carregar="carregarLicitacoes"
      :importar="importarLicitacoes"
      :headers="headers"
      :items="licitacoes"
      ></PainelGenerico>

      <Alert :loading="isLoading" :texto="mensagem.texto" :tipo="mensagem.tipo" :visivel="mensagem.visivel"></Alert>
    </v-container>
  </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import LicitacaoServico from '@/servicos/LicitacaoController';
import Alert from '@/components/Alert.vue';
import PainelGenerico from '@/components/PainelGenerico.vue';
import AppBar from '@/components/AppBar.vue';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Licitações'
const licitacoes = ref([]);
const headers = computed(() => {
     if (!licitacoes.value.length) return []

     const keys = Object.keys(licitacoes.value[0]).filter(key => key != 'id' && key!= 'criadoEm' && key != 'ultimaAlteracao')

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

async function importarLicitacoes() {
    isLoading.value = true;

    try {
        const response = await LicitacaoServico.importar();

        if (response.status >= 200 && response.status < 300) {
            exibirMensagem("Licitações importados com sucesso!", 'success');
            await carregarLicitacoes();
        }

    } catch (error) {
        console.error('Erro ao importar licitações:', error);
        exibirMensagem('Erro ao importar licitações.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

async function carregarLicitacoes() {
    isLoading.value = true;

    try {
        const response = await LicitacaoServico.obterTodos();

        if (response.status >= 200 && response.status < 300) {
            licitacoes.value = response.data || [];
            console.log('Licitações carregados:', licitacoes.value);
            exibirMensagem('Licitações carregados com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar licitações:', error);
        exibirMensagem('Erro ao carregar licitações.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted (() => {
    carregarLicitacoes();
})

</script>