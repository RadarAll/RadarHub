using System.Text.Json;
using RSK.Dominio.Servicos;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Interfaces;
using RadarHub.Dominio.Entidades;
using RadarHub.Integracoes.Pnc;
using RSK.Dominio.Notificacoes.Entidades;
using RadarHub.Dominio.DTOs;

namespace RadarHub.Dominio.Servicos
{
    public class UnidadeServico : ServicoImportacaoTerceiro<Unidade>
    {
        private readonly Pncp _pncp;
        private readonly ITransacao _transacao;

        public UnidadeServico(
            IRepositorioImportacaoTerceiro<Unidade> repositorio,
            IServicoMensagem mensagens,
            ITransacao transacao)
            : base(repositorio, mensagens)
        {
            _pncp = new Pncp();
            _transacao = transacao;
        }

        public async Task<bool> ImportarAsync()
        {
            try
            {
                // Obter dados do terceiro
                var json = await _pncp.GetFiltros();
                var filtros = JsonSerializer.Deserialize<FiltrosDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (filtros?.Filters?.Unidades == null || !filtros.Filters.Unidades.Any())
                {
                    _mensagens.Adicionar("Nenhuma unidade encontrada para importação.", TipoMensagem.Aviso);
                    return true; // Sem dados, mas não é erro
                }

                int novas = 0;
                int atualizadas = 0;

                // Processar cada unidade
                foreach (var item in filtros.Filters.Unidades)
                {
                    var existente = await ObterPorIdTerceiroAssincrono(item.Id);

                    if (existente == null)
                    {
                        var nova = new Unidade(item.Id, item.Nome, item.Codigo, item.CodigoNome);
                        await _repositorio.AdicionarAssincrono(nova);
                        novas++;
                    }
                    else if (!string.Equals(existente.Nome, item.Nome, StringComparison.OrdinalIgnoreCase) ||
                             !string.Equals(existente.Codigo, item.Codigo, StringComparison.OrdinalIgnoreCase) ||
                             !string.Equals(existente.CodigoNome, item.CodigoNome, StringComparison.OrdinalIgnoreCase))
                    {
                        existente.Nome = item.Nome;
                        existente.Codigo = item.Codigo;
                        existente.CodigoNome = item.CodigoNome;
                        _repositorio.AtualizarAssincrono(existente);
                        atualizadas++;
                    }
                }

                // Commit da transação
                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Importação concluída. {novas} novas unidades adicionadas e {atualizadas} atualizadas.",
                    TipoMensagem.Sucesso);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback em caso de erro
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao importar unidade: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }
    }
}
