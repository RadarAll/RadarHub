<template>
  <v-app-bar
    class="elevation-2" >
    <v-toolbar-title class="text-h6 font-weight-bold text-green-primary">
      {{ titulo }}
    </v-toolbar-title>
    <v-spacer />

    <v-btn icon color="green-primary">
      <v-icon>mdi-bell</v-icon>
    </v-btn>

    <v-btn to="/perfil" icon color="green-primary">
      <v-icon>mdi-account-circle</v-icon>
    </v-btn>
  </v-app-bar>

</template>

<script setup>
defineProps({
  titulo: {
    type: String,
    required: true
  }
})
</script>