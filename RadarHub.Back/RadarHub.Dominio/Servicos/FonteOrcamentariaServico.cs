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
    public class FonteOrcamentariaServico : ServicoImportacaoTerceiro<FonteOrcamentaria>
    {
        private readonly Pncp _pncp;
        private readonly ITransacao _transacao;

        public FonteOrcamentariaServico(
            IRepositorioImportacaoTerceiro<FonteOrcamentaria> repositorio,
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

                if (filtros?.Filters?.FontesOrcamentarias == null || !filtros.Filters.FontesOrcamentarias.Any())
                {
                    _mensagens.Adicionar("Nenhuma fonte orçamentária encontrado para importação.", TipoMensagem.Aviso);
                    return true;
                }

                int novas = 0;
                int atualizadas = 0;

                foreach (var item in filtros.Filters.FontesOrcamentarias)
                {
                    var existente = await ObterPorIdTerceiroAssincrono(item.Id);

                    if (existente == null)
                    {
                        var nova = new FonteOrcamentaria(item.Id, item.Nome);
                        await _repositorio.AdicionarAssincrono(nova);
                        novas++;
                    }
                    else if (!string.Equals(existente.Nome, item.Nome, StringComparison.OrdinalIgnoreCase))
                    {
                        existente.Nome = item.Nome;
                        _repositorio.AtualizarAssincrono(existente);
                        atualizadas++;
                    }
                }

                // Commit da transação
                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Importação concluída. {novas} novas fontes orçamentárias adicionadas e {atualizadas} atualizadas.",
                    TipoMensagem.Sucesso);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback em caso de erro
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao importar fontes orçamentárias: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }
    }
}
