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
    public class UfsServico : ServicoImportacaoTerceiro<Ufs>
    {
        private readonly Pncp _pncp;
        private readonly ITransacao _transacao;

        public UfsServico(
            IRepositorioImportacaoTerceiro<Ufs> repositorio,
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

                if (filtros?.Filters?.Ufs == null || !filtros.Filters.Ufs.Any())
                {
                    _mensagens.Adicionar("Nenhum UF encontrado para importação.", TipoMensagem.Aviso);
                    return true; // Sem dados, mas não é erro
                }

                int novas = 0;
                int atualizadas = 0;

                // Processar cada UF
                foreach (var item in filtros.Filters.Ufs)
                {
                    var existente = await ObterPorIdTerceiroAssincrono(item.Id);

                    if (existente == null)
                    {
                        var nova = new Ufs(item.Id);
                        await _repositorio.AdicionarAssincrono(nova);
                        novas++;
                    }
                    else if (!string.Equals(existente.IdTerceiro, item.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        existente.IdTerceiro = item.Id;
                        _repositorio.AtualizarAssincrono(existente);
                        atualizadas++;
                    }
                }

                // Commit da transação
                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Importação concluída. {novas} novas UFs adicionadas e {atualizadas} atualizadas.",
                    TipoMensagem.Sucesso);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback em caso de erro
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao importar UFs: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }
    }
}
