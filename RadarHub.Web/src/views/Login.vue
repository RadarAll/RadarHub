<template>
  <v-container fluid class="bg bg-image fill-height d-flex justify-center align-center">
    <v-card class="elevation-2 rounded-lg pa-0 overflow-hidden" max-width="1400" style="width: 100%; height: 700px;">
      <v-row no-gutters class="h-100">
        <v-col cols="12" md="6" class="d-flex flex-column justify-center align-center gradient-bg text-white pa-8">
          <div class="text-h4 font-weight-bold mb-4">Bem vindo ao RadarHub <v-icon>mdi-hub</v-icon></div>
          <div class="text-body-1 text-white text-center">
            Centro para consulta, transparência e acompanhamento das informações do Plano Nacional de Contratações Públicas.
          </div>
        </v-col>

        <v-col cols="12" md="6" class="d-flex justify-center align-center pa-8">
          <v-card flat class="w-100" max-width="320">
            <v-card-title class="text-green-light-md justify-center text-h5 font-weight-bold mb-6">Login de Usuário</v-card-title>

            <v-text-field v-model="email" prepend-inner-icon="mdi-account" label="Email" variant="outlined" density="comfortable" class="mb-4"/>

            <v-text-field v-model="senha" prepend-inner-icon="mdi-lock" label="Password" type="senha" variant="outlined" density="comfortable" class="mb-4"/>

            <v-btn @click="login" color="green-light-md" class="text-neutral-light" block>
              <v-icon start>mdi-login</v-icon>
              Entrar
            </v-btn>
          </v-card>
        </v-col>
      </v-row>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const email = ref('')
const senha = ref('')

const login = async () => {

    try {
        const response = await axios.post('https://localhost:7203/api/Autenticacao/login', {
            email: email.value,
            senha: senha.value
        })

        const token = response.data.token

        localStorage.setItem('token', token)
        localStorage.setItem('emailUsuario', email.value)
        
        window.location.href = '/home'
    } catch (error) {
        console.log('Erro no Login:', error);
    }
}

</script>

<style scoped>
.gradient-bg {
  background: linear-gradient(135deg, #99BA73, #144003);
}
.bg {
    background: #0b2502;
}

.bg-image {
  background-image: url('/fundo.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}


</style>
