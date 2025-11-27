<template>
    <v-main class="bg-grey-lighten-4">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="fluid">
            <PainelGenerico
            :carregar="carregarUsuarios"
            :headers="headers"
            :items="usuarios"
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
import UsuarioServico from '@/servicos/UsuarioController';
import AppBar from '@/components/AppBar.vue';
import axios from 'axios';

const isLoading = ref(false);
const mensagem = ref({
    visivel: false,
    tipo: 'info',
    texto: ''
});

const titulo = 'Usuarios'
const usuarios = ref([]);
const headers = computed(() => {
    if (!usuarios.value.length) return []

    const keys = Object.keys(usuarios.value[0]).filter(key => key != 'id' && key != 'criadoEm' && key != 'ultimaAlteracao' && key!= 'senhaHash' && key !='dataDesativacao')

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

async function carregarUsuarios() {
    isLoading.value = true;

    try {
        const response = await UsuarioServico.obterTodos();

        if (response.status >= 200 && response.status < 300 ) {
            usuarios.value = response.data || [];
            console.log("Usuarios carregadas com sucesso:", usuarios.value);
            exibirMensagem("Usuarios carregadas com sucesso!", 'success', 3000);
        }
    } catch (error) {
        console.error("Erro ao carregar Usuarios: ", error);
        exibirMensagem("Erro ao carregar Usuarios.", 'error', 3000);
    } finally {
        isLoading.value = false;
    }
}

onMounted(() => {
    carregarUsuarios();
})
</script>