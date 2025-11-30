<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container fluid>
            <PainelGenerico
            :carregar="carregarSegmentos"
            :criar="criarSegmento"
            :headers="headers"
            :items="segmentos"
            ></PainelGenerico>

            <Alert :visivel="mensagem.visivel" :tipo="mensagem.tipo" :texto="mensagem.texto" :loading="isLoading" ></Alert>

            <v-dialog v-model="dialog" max-width="500px" >
                <v-card class="pa-3">
                    <v-card-title><v-icon>mdi-tag-outline</v-icon>Adicionar Segmento</v-card-title>
                    <v-card-text>
                        <v-text-field v-model="novoSegmento.nome"></v-text-field>
                    </v-card-text>
                    <v-card-actions class="d-flex justify-center">
                        <v-btn class="bg-green-primary" @click="salvarSegmento"><v-icon>mdi-plus</v-icon></v-btn>
                        <v-btn class="bg-red-dark-md" color="neutral-light" @click="dialog = false"><v-icon>mdi-close</v-icon></v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>


        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import SegmentoServico from '@/servicos/SegmentoController';
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

const dialog = ref(false)
const novoSegmento = ref({ nome: '' })
const titulo = 'Segmentos'
const segmentos = ref([]);
const headers = computed(() => {
     if (!segmentos.value.length) return []

     const keys = Object.keys(segmentos.value[0]).filter(key => key != 'id')

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

async function salvarSegmento() {
  try {
    const payload = { nome: novoSegmento.value.nome }
    const response = await axios.post(`https://localhost:7203/api/Segmento`, payload,
        {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    }
    )
    

    if (response.status >= 200 && response.status < 300) {
      exibirMensagem('Segmento criado com sucesso!', 'success')
      dialog.value = false
      carregarSegmentos()
    }
  } catch (error) {
    console.error('Erro ao criar segmento:', error)
    exibirMensagem('Erro ao criar segmento.', 'error')
  }
}


async function carregarSegmentos() {
    isLoading.value = true;

    try {
        const response = await SegmentoServico.obterTodos()

        if (response.status >= 200 && response.status < 300) {
            segmentos.value = response.data || [];
            console.log('Segmentos carregados:', segmentos.value);
            exibirMensagem('Segmentos carregados com sucesso!', 'success', 3000)
        }
        

    } catch (error) {
        console.error('Erro ao carregar segmentos:', error);
        exibirMensagem('Erro ao carregar segmentos.', 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

function criarSegmento() {
  dialog.value = true
}

onMounted (() => {
    carregarSegmentos();
})

</script>