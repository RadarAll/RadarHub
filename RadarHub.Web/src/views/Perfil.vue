<template>
    <v-main class="bg-grey-lighten-4 d-flex flex-row justify-center align-center">
        <AppBar :titulo="titulo"></AppBar>
        <v-container class="fluid">
            <v-row class="d-flex flex-row justify-center">
                <v-col cols="7">
                    <v-card class="d-flex flex-column justify-center elevation-2 rounded-lg h-100 d-flex flex-column align-center pa-6  border-top-green-primary" style="border-top: 4px solid var(--green-light)">
                        <v-avatar size="150" class="mb-4">
                            <v-img :src="User" />
                        </v-avatar>
                        <div class="text-h6 font-weight-bold text-green-primary">{{ usuarioLogado.nomeCompleto }}</div>
                        <div class="text-body-2 text-grey-darken-1">{{cargo }}</div>
                        <div class="text-caption text-grey-darken-1 mt-2">{{ empresa }}</div>
                    </v-card>
                </v-col>

                <v-col cols="7" class="d-flex flex-column">
                    <v-card class="elevation-2 rounded-lg" style="border-top: 4px solid var(--green-primary)">
                        <v-card-title class="bg-green-light pa-4 w-100 d-flex justify-center">
                            <v-icon color="green-dark-md" class="mr-2">mdi-account</v-icon>
                            <span class="text-h6 text-green-primary font-weight-bold">Perfil</span>
                        </v-card-title>
                        <v-divider></v-divider>

                        <v-card-text class="pa-6">
                            <v-list density="compact">
                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-account</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Nome completo</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ usuarioLogado.nomeCompleto }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-email</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Email</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ usuarioLogado.email }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-lock</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Senha</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    ********
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-text</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Sobre você</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ bio }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-earth</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">País</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ pais }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-account-badge-outline</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Cargo</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ cargo }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                                <v-divider />

                                <v-list-item>
                                    <template v-slot:prepend>
                                    <v-icon color="grey-darken-1">mdi-domain</v-icon>
                                    </template>
                                    <v-list-item-title class="text-caption text-grey-darken-1">Empresa</v-list-item-title>
                                    <v-list-item-subtitle class="text-body-2 font-weight-medium">
                                    {{ empresa }}
                                    </v-list-item-subtitle>
                                </v-list-item>
                            </v-list>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-main>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import AppBar from '@/components/AppBar.vue'
import User from '@/assets/user.png'
import axios from 'axios'

const titulo = 'Meu perfil'
const mostrarSenha = ref(false)
const bio = 'Faço parte da equipe da RadarAll, o seu sistema de licitações'
const pais = 'Brasil'
const cargo = 'SuperUser'
const empresa = 'RadarAll'
const usuarioLogado = ref({})
const emailUsuario = localStorage.getItem('emailUsuario')

async function getPerfil() {
    try {
        const token = localStorage.getItem('token')
        const response = await axios.get(`https://localhost:7203/api/Usuario/email/${encodeURIComponent(emailUsuario)}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            }
        })

        usuarioLogado.value = response.data.dados

    } catch (error) {
        console.log('Não foi possível buscar os dados do Perfil:', error);
    }
}

onMounted(() => {
    getPerfil() 
})

</script>
