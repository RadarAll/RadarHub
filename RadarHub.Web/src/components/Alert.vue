<template>
  <v-row class="d-flex justify-end pt-16 pb-5 footer-fixed">

    <!-- Sucesso -->
    <div v-if="visivel && tipo === 'success'" class="w-25">
      <v-alert type="success" border="start" variant="tonal" class="d-flex rounded-md elevation-2 justify-center font-weight-bold text-success bg-green-light">
        {{ texto }}
      </v-alert>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="w-25">
      <v-alert border="start" variant="tonal" class="d-flex rounded-md elevation-2 justify-center font-weight-bold text-green-primary bg-green-light">
        <v-icon class="mdi-spin mr-3" size="large">mdi-sync</v-icon>
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

<style scoped>
.footer-fixed {
  position: fixed;
  bottom: 0;
  left: 0;
  width: 100%;
  z-index: 1000;
}
</style>
