<template>

</template>

<script>
import ModalidadeServico from '@/servicos/ModalidadeServico';
import { onMounted } from 'vue';

async function teste() {
  await ModalidadeServico.importar();
}

onMounted(teste);
</script>