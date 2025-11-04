using RadarHub.Dominio.DTOs;
using RadarHub.Dominio.Entidades;
using RadarHub.Integracoes.Pnc;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Entidades;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Servicos;
using System.Text.Json;

namespace RadarHub.Dominio.Servicos
{
    public class EsferaServico : ServicoImportacaoTerceiro<Esfera>
    {
        private readonly Pncp _pncp;
        private readonly ITransacao _transacao;

        public EsferaServico(
            IRepositorioImportacaoTerceiro<Esfera> esferaRepositorio,
            IServicoMensagem mensagens,
            ITransacao transacao)
            : base(esferaRepositorio, mensagens)
        {
            _pncp = new Pncp();
            _transacao = transacao;
        }

        public async Task<bool> ImportarAsync()
        {
            try
            {
                // Obter os dados do terceiro
                var json = await _pncp.GetFiltros();
                var filtros = JsonSerializer.Deserialize<FiltrosDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (filtros?.Filters.Esferas == null || !filtros.Filters.Esferas.Any())
                {
                    _mensagens.Adicionar("Nenhuma esfera encontrada para importação.", TipoMensagem.Aviso);
                    return true; // Não há nada a importar, mas não é erro
                }

                int novas = 0;
                int atualizadas = 0;

                // Processar cada esfera
                foreach (var item in filtros.Filters.Esferas)
                {
                    var existente = await _repositorio.ObterPorIdTerceiroAssincrono(item.Id);

                    if (existente == null)
                    {
                        var nova = new Esfera(item.Id, item.Nome);
                        await _repositorio.AdicionarAssincrono(nova);
                        novas++;
                    }
                    else if (!string.Equals(existente.Nome, item.Nome, StringComparison.OrdinalIgnoreCase))
                    {
                        existente.Nome = item.Nome;
                        await _repositorio.AtualizarAssincrono(existente);
                        atualizadas++;
                    }
                }

                // Commit da transação
                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Importação concluída. {novas} novas esferas adicionadas e {atualizadas} atualizadas.",
                    TipoMensagem.Sucesso);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback em caso de erro
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao importar esferas: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }
    }
}
