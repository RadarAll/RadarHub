<template>
  <div class="p-8 bg-neutral-100 min-h-screen">
    <!-- Cabeçalho -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-neutral-800 tracking-tight text-green-primary">
        Lista de Licitações
      </h1>
    </div>

    <!-- Barra de pesquisa e botão -->
    <div class="flex gap-2 mb-3 justify-between items-center">
      <div class="relative w-80">
        <input
          v-model="filtro"
          type="text"
          placeholder="Buscar por objeto, órgão..."
          class="w-full pl-4 pr-4 py-2 rounded-lg border border-neutral-300 text-neutral-800 placeholder-neutral-400
          focus:outline-none focus:border-neutral-500 focus:ring-1 focus:ring-neutral-400 transition"
        />
      </div>

      <button 
        @click="importarDados" 
        :disabled="carregando"
        class="bg-green-primary text-white hover:bg-green-dark disabled:opacity-50 px-4 py-2 rounded-lg transition"
      >
        {{ carregando ? 'Importando...' : 'Importar' }}
      </button>
    </div>

    <!-- Tabela -->
    <div class="bg-white border border-neutral-200 rounded-2xl shadow-sm overflow-hidden">
      <div class="flex-1 overflow-x-auto">
        <table class="w-full text-left">
          <thead class="bg-neutral-50 text-neutral-700 border-b border-neutral-200">
            <tr>
              <th class="px-[16px] py-[12px] font-medium">ID</th>
              <th class="px-[16px] py-[12px] font-medium">Objeto</th>
              <th class="px-[16px] py-[12px] font-medium">Órgão</th>
              <th class="px-[16px] py-[12px] font-medium">Modalidade</th>
              <th class="px-[16px] py-[12px] font-medium">Data</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="item in licitacoesFiltradasPaginadas"
              :key="item.id"
              class="hover:bg-neutral-50 transition-colors border-b border-neutral-100"
            >
              <td class="px-[16px] py-[12px] text-neutral-800 text-sm">
                {{ item.id || '-' }}
              </td>
              <td class="px-[16px] py-[12px] text-neutral-800 text-sm truncate max-w-xs">
                {{ item.objeto || item.nome || '-' }}
              </td>
              <td class="px-[16px] py-[12px] text-neutral-800 text-sm">
                {{ item.orgaoNome || item.orgao || '-' }}
              </td>
              <td class="px-[16px] py-[12px] text-neutral-800 text-sm">
                {{ item.modalidadeLicitacaoNome || '-' }}
              </td>
              <td class="px-[16px] py-[12px] text-neutral-800 text-sm">
                {{ formatarData(item.dataSessao) }}
              </td>
            </tr>
            <tr v-if="licitacoes.length === 0">
              <td colspan="5" class="px-[16px] py-[12px] text-center text-neutral-600">
                Nenhuma licitação encontrada
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Paginação -->
      <div class="flex justify-center items-center gap-3 px-6 py-3 bg-neutral-50 border-t border-neutral-200">
        <button
          :disabled="currentPage === 1"
          @click="previousPage"
          class="px-3 py-1 border border-neutral-300 rounded text-neutral-700 hover:border-neutral-500 disabled:opacity-50"
        >
          ← Anterior
        </button>

        <span class="text-sm text-neutral-700 font-medium">
          Página {{ currentPage }} de {{ totalPages }}
        </span>

        <button
          :disabled="currentPage === totalPages"
          @click="nextPage"
          class="px-3 py-1 border border-neutral-300 rounded text-neutral-700 hover:border-neutral-500 disabled:opacity-50"
        >
          Próxima →
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import LicitacaoServico from '../servicos/LicitacaoServico'

const licitacoes = ref([])
const filtro = ref('')
const currentPage = ref(1)
const itemsPerPage = 15
const carregando = ref(false)
const { toast } = useToast()

const carregar = async () => {
  try {
    carregando.value = true
    const data = await LicitacaoServico.obterOData()
    licitacoes.value = Array.isArray(data) ? data : data?.value || []
    currentPage.value = 1
  } catch (e) {
    toast({
      component: 'div',
      content: 'Erro ao carregar licitações: ' + (e.message || 'Desconhecido'),
      options: { type: 'error' }
    })
  } finally {
    carregando.value = false
  }
}

const importarDados = async () => {
  try {
    carregando.value = true
    const res = await LicitacaoServico.importar()
    toast({
      component: 'div',
      content: 'Importação concluída com sucesso!',
      options: { type: 'success' }
    })
    console.info('Resultado importação:', res)
    await carregar()
  } catch (err) {
    toast({
      component: 'div',
      content: err.message || 'Erro na importação',
      options: { type: 'error' }
    })
    console.error(err)
  } finally {
    carregando.value = false
  }
}

const totalPages = computed(() => Math.max(1, Math.ceil(licitacoes.value.length / itemsPerPage)))
const nextPage = () => { if (currentPage.value < totalPages.value) currentPage.value++ }
const previousPage = () => { if (currentPage.value > 1) currentPage.value-- }

const licitacoesFiltradas = computed(() => {
  if (!filtro.value.trim()) return licitacoes.value
  const termo = filtro.value.toLowerCase()
  return licitacoes.value.filter(l =>
    (l.objeto || l.nome || '').toString().toLowerCase().includes(termo) ||
    (l.orgaoNome || '').toLowerCase().includes(termo)
  )
})

const licitacoesFiltradasPaginadas = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return licitacoesFiltradas.value.slice(start, start + itemsPerPage)
})

const formatarData = (data) => {
  if (!data) return '-'
  return new Date(data).toLocaleDateString('pt-BR')
}

onMounted(carregar)
</script>
