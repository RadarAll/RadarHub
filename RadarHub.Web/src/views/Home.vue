<template>
  <div class="container-home">
    <header class="home-header">
      <h1>Gestão de Modalidades</h1>
      <p class="subtitle">Importe e gerencie modalidades de licitação</p>
    </header>

    <section class="actions-section">
      <button 
        @click="importarModalidades" 
        :disabled="isLoading"
        class="btn btn-primary"
        :aria-busy="isLoading"
      >
        <span v-if="!isLoading">📥 Importar Modalidades</span>
        <span v-else>⏳ Importando...</span>
      </button>

      <button 
        @click="carregarModalidades" 
        :disabled="isLoading"
        class="btn btn-secondary"
      >
        🔄 Carregar Modalidades
      </button>
    </section>

    <!-- Alertas de Status -->
    <div v-if="mensagem.visivel" :class="['alert', `alert-${mensagem.tipo}`]" role="alert">
      <p>{{ mensagem.texto }}</p>
      <button @click="fecharMensagem" class="btn-fechar" aria-label="Fechar alerta">×</button>
    </div>

    <!-- Tabela de Modalidades -->
    <section class="modalidades-section" v-if="modalidades.length > 0">
      <h2>Modalidades Cadastradas</h2>
      <div class="table-responsive">
        <table class="modalidades-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nome</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="modalidade in modalidades" :key="modalidade.id">
              <td>{{ modalidade.id }}</td>
              <td>{{ modalidade.nome }}</td>
              <td class="actions-cell">
                <button @click="editarModalidade(modalidade)" class="btn btn-sm btn-info">
                  ✏️ Editar
                </button>
                <button @click="deletarModalidade(modalidade.id)" class="btn btn-sm btn-danger">
                  🗑️ Deletar
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <!-- Estado Vazio -->
    <section v-else-if="!isLoading" class="empty-state">
      <p>Nenhuma modalidade cadastrada.</p>
      <p class="text-muted">Clique em "Importar Modalidades" para começar.</p>
    </section>

    <!-- Estado de Carregamento -->
    <section v-if="isLoading" class="loading-state">
      <div class="spinner"></div>
      <p>Processando...</p>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import ModalidadeServico from '@/servicos/ModalidadeServico';

// ============ ESTADOS REATIVAS ============
const modalidades = ref([]);
const isLoading = ref(false);
const mensagem = ref({
  visivel: false,
  tipo: 'info', // 'success', 'error', 'warning', 'info'
  texto: ''
});

// ============ CONSTANTES ============
const TEMPO_ALERTA = 5000; // 5 segundos

// ============ MÉTODOS ============

/**
 * Exibe uma mensagem de alerta
 * @param {string} texto - Texto da mensagem
 * @param {string} tipo - Tipo de alerta (success, error, warning, info)
 */
function exibirMensagem(texto, tipo = 'info') {
  mensagem.value = {
    visivel: true,
    tipo,
    texto
  };

  setTimeout(() => {
    fecharMensagem();
  }, TEMPO_ALERTA);
}

/**
 * Fecha a mensagem de alerta
 */
function fecharMensagem() {
  mensagem.value.visivel = false;
}

/**
 * Importa modalidades do servidor
 */
async function importarModalidades() {
  isLoading.value = true;
  try {
    const res = await ModalidadeServico.importar();
    
    if (res.status >= 200 && res.status < 300) {
      exibirMensagem('Modalidades importadas com sucesso!', 'success');
      await carregarModalidades();
    }
  } catch (error) {
    console.error('Erro ao importar modalidades:', error);
    const mensagemErro = error.response?.data?.message || 'Erro ao importar modalidades';
    exibirMensagem(mensagemErro, 'error');
  } finally {
    isLoading.value = false;
  }
}

/**
 * Carrega todas as modalidades
 */
async function carregarModalidades() {
  isLoading.value = true;
  try {
    const res = await ModalidadeServico.obterTodos();
    
    if (res.status >= 200 && res.status < 300) {
      modalidades.value = res.data || [];
    }
  } catch (error) {
    console.error('Erro ao carregar modalidades:', error);
    exibirMensagem('Erro ao carregar modalidades', 'error');
  } finally {
    isLoading.value = false;
  }
}

/**
 * Edita uma modalidade (placeholder para implementação futura)
 * @param {Object} modalidade - Modalidade a editar
 */
function editarModalidade(modalidade) {
  console.log('Editar modalidade:', modalidade);
  exibirMensagem('Funcionalidade de edição em desenvolvimento', 'info');
  // TODO: Implementar navegação para página de edição ou modal
}

/**
 * Deleta uma modalidade (placeholder para implementação futura)
 * @param {number} id - ID da modalidade a deletar
 */
function deletarModalidade(id) {
  const confirmacao = confirm('Tem certeza que deseja deletar esta modalidade?');
  if (confirmacao) {
    console.log('Deletar modalidade:', id);
    exibirMensagem('Funcionalidade de deleção em desenvolvimento', 'info');
    // TODO: Implementar chamada de serviço para deletar
  }
}

// ============ CICLO DE VIDA ============

/**
 * Carrega as modalidades ao montar o componente
 */
onMounted(() => {
  carregarModalidades();
});
</script>

<style scoped>
/* ============ VARIÁVEIS ============ */
:root {
  --primary-color: #007bff;
  --secondary-color: #6c757d;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --warning-color: #ffc107;
  --info-color: #17a2b8;
  --light-color: #f8f9fa;
  --dark-color: #343a40;
  --border-radius: 0.375rem;
  --transition: all 0.3s ease;
}

/* ============ LAYOUT GERAL ============ */
.container-home {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  color: var(--dark-color);
}

/* ============ HEADER ============ */
.home-header {
  margin-bottom: 2rem;
  border-bottom: 2px solid var(--primary-color);
  padding-bottom: 1rem;
}

.home-header h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  font-weight: 700;
  color: var(--dark-color);
}

.subtitle {
  margin: 0;
  font-size: 1rem;
  color: var(--secondary-color);
}

/* ============ AÇÕES ============ */
.actions-section {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

/* ============ BOTÕES ============ */
.btn {
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  font-weight: 600;
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: var(--transition);
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
}

.btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.btn-primary {
  background-color: var(--primary-color);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
  box-shadow: 0 2px 8px rgba(0, 123, 255, 0.25);
}

.btn-secondary {
  background-color: var(--secondary-color);
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #5a6268;
}

.btn-info {
  background-color: var(--info-color);
  color: white;
}

.btn-info:hover:not(:disabled) {
  background-color: #138496;
}

.btn-danger {
  background-color: var(--danger-color);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background-color: #c82333;
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
}

.btn-fechar {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: inherit;
  padding: 0;
  line-height: 1;
}

.btn-fechar:hover {
  opacity: 0.7;
}

/* ============ ALERTAS ============ */
.alert {
  padding: 1rem;
  margin-bottom: 1.5rem;
  border-radius: var(--border-radius);
  display: flex;
  justify-content: space-between;
  align-items: center;
  animation: slideIn 0.3s ease-out;
}

.alert p {
  margin: 0;
}

.alert-success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.alert-error {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.alert-warning {
  background-color: #fff3cd;
  color: #856404;
  border: 1px solid #ffeeba;
}

.alert-info {
  background-color: #d1ecf1;
  color: #0c5460;
  border: 1px solid #bee5eb;
}

/* ============ TABELA ============ */
.modalidades-section {
  margin-top: 2rem;
}

.modalidades-section h2 {
  margin-bottom: 1rem;
  font-size: 1.5rem;
  color: var(--dark-color);
}

.table-responsive {
  overflow-x: auto;
}

.modalidades-table {
  width: 100%;
  border-collapse: collapse;
  background-color: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border-radius: var(--border-radius);
  overflow: hidden;
}

.modalidades-table thead {
  background-color: var(--light-color);
  font-weight: 700;
}

.modalidades-table th,
.modalidades-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #dee2e6;
}

.modalidades-table tbody tr:hover {
  background-color: #f5f5f5;
}

.modalidades-table tbody tr:last-child td {
  border-bottom: none;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

/* ============ ESTADO VAZIO ============ */
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  background-color: var(--light-color);
  border-radius: var(--border-radius);
  margin-top: 2rem;
}

.empty-state p {
  margin: 0.5rem 0;
  font-size: 1.1rem;
}

.text-muted {
  color: var(--secondary-color);
  font-size: 0.95rem;
}

/* ============ CARREGAMENTO ============ */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  gap: 1rem;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--light-color);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

/* ============ ANIMAÇÕES ============ */
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* ============ RESPONSIVIDADE ============ */
@media (max-width: 768px) {
  .container-home {
    padding: 1.5rem 0.5rem;
  }

  .home-header h1 {
    font-size: 1.5rem;
  }

  .actions-section {
    flex-direction: column;
  }

  .btn {
    width: 100%;
    justify-content: center;
  }

  .actions-cell {
    flex-direction: column;
  }

  .actions-cell .btn {
    width: 100%;
  }

  .modalidades-table th,
  .modalidades-table td {
    padding: 0.75rem 0.5rem;
    font-size: 0.875rem;
  }
}

@media (max-width: 480px) {
  .home-header h1 {
    font-size: 1.25rem;
  }

  .subtitle {
    font-size: 0.9rem;
  }

  .btn {
    padding: 0.625rem 1rem;
    font-size: 0.875rem;
  }
}
</style>
