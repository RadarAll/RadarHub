<template>
  <v-row class="d-flex justify-center pt-15">

    <!-- Sucesso -->
    <div v-if="visivel && tipo === 'success'" class="w-25">
      <v-alert type="success" border="start" variant="tonal" class="d-flex rounded-md elevation-2 justify-center font-weight-bold text-success bg-green-light">
        {{ texto }}
      </v-alert>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="w-25">
      <v-alert type="loading" border="start" variant="tonal" class="d-flex rounded-md elevation-2 justify-center font-weight-bold text-green-primary bg-green-light">
        Aguardando importação...
      </v-alert>
    </div>

    <!-- Erro -->
    <div v-if="visivel && tipo === 'error'" class="w-25">
      <v-alert type="error"border="start" variant="tonal" class="d-flex rounded-md elevation-2 justify-center font-weight-bold text-red-primary bg-red-light">
        {{ texto }}
      </v-alert>
    </div>

  </v-row>
</template>

<script setup>

defineProps({
  visivel: { type: Boolean, default: false },
  tipo: { type: String, default: 'info' }, // success | error | info
  texto: { type: String, default: '' },
  loading: { type: Boolean, default: false }
})

</script>