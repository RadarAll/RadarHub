using RadarHub.Dominio.Entidades;
using RadarHub.Integracoes.Pnc;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Entidades;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Servicos;

namespace RadarHub.Dominio.Servicos
{
    public class LicitacaoServico : ServicoImportacaoTerceiro<Licitacao>
    {
        private readonly Pncp _pncp;
        private readonly ITransacao _transacao;

        private readonly OrgaoServico _orgaoServico;
        private readonly MunicipioServico _municipioServico;
        private readonly ModalidadeServico _modalidadeServico;
        private readonly TipoServico _tipoServico;
        private readonly FonteOrcamentariaServico _fonteOrcamentariaServico;
        private readonly UfsServico _ufsServico;

        public LicitacaoServico(
            IRepositorioImportacaoTerceiro<Licitacao> repositorio,
            ITransacao transacao,
            IServicoMensagem mensagens,
            OrgaoServico orgaoServico,
            MunicipioServico municipioServico,
            ModalidadeServico modalidadeServico,
            TipoServico tipoServico,
            FonteOrcamentariaServico fonteOrcamentariaServico,
            UfsServico ufsServico
        ) : base(repositorio, mensagens)
        {
            _pncp = new Pncp();
            _transacao = transacao;
            _orgaoServico = orgaoServico;
            _municipioServico = municipioServico;
            _modalidadeServico = modalidadeServico;
            _tipoServico = tipoServico;
            _fonteOrcamentariaServico = fonteOrcamentariaServico;
            _ufsServico = ufsServico;
        }

        private async Task<Licitacao?> MapearLicitacaoAsync(LicitacaoResponse item, Licitacao? existente)
        {
            var licitacao = existente ?? new Licitacao { IdTerceiro = item.Id };

            licitacao.OrgaoIdTerceiro = item.OrgaoId;
            licitacao.MunicipioIdTerceiro = item.MunicipioId;
            licitacao.UfIdTerceiro = item.Uf;
            licitacao.ModalidadeIdTerceiro = item.ModalidadeLicitacaoId;
            licitacao.TipoIdTerceiro = item.TipoId;
            licitacao.FonteOrcamentariaIdTerceiro = item.FonteOrcamentariaId.ToString();
            licitacao.EsferaIdTerceiro = item.EsferaId;

            licitacao.UnidadeIdTerceiro = item.UnidadeId;
            licitacao.Titulo = item.Title;
            licitacao.Descricao = item.Description;
            licitacao.ItemUrl = item.ItemUrl;
            licitacao.Ano = item.Ano;
            licitacao.UltimaAlteracao = DateTime.Now;
            licitacao.DataPublicacaoPncp = item.DataPublicacaoPncp;
            licitacao.DataAtualizacaoPncp = item.DataAtualizacaoPncp;
            licitacao.DataInicioVigencia = item.DataInicioVigencia;
            licitacao.DataFimVigencia = item.DataFimVigencia;
            licitacao.ValorGlobal = item.ValorGlobal;
            licitacao.TemResultado = item.TemResultado;
            licitacao.OrgaoSubrogadoId = item.OrgaoSubrogadoId;
            licitacao.OrgaoSubrogadoNome = item.OrgaoSubrogadoNome;
            licitacao.NumeroSequencial = item.NumeroSequencial;
            licitacao.NumeroControlePncp = item.NumeroControlePncp;
            licitacao.ExigenciaConteudoNacional = item.ExigenciaConteudoNacional;
            licitacao.NumeroSequencialCompraAta = item.NumeroSequencialCompraAta;

            return licitacao;
        }

        public async Task<bool> ImportarAsync()
        {
            try
            {
                var licitacoesResponse = await _pncp.GetTodasLicitacoesRecebendoProposta();

                if (licitacoesResponse == null || !licitacoesResponse.Any())
                {
                    _mensagens.Adicionar("Nenhuma licitação encontrada para importação.", TipoMensagem.Aviso);
                    return true;
                }

                var novasLicitacoes = new List<Licitacao>();
                var atualizadasLicitacoes = new List<Licitacao>();
                int ignoradas = 0;

                foreach (var item in licitacoesResponse)
                {
                    _mensagens.Limpar();
                    try
                    {
                        var existente = await ObterPorIdTerceiroAssincronoParaImportacao(item.Id);
                        var licitacaoFinal = await MapearLicitacaoAsync(item, existente);

                        if (licitacaoFinal == null)
                        {
                            ignoradas++;
                            continue;
                        }

                        if (existente == null)
                        {
                            novasLicitacoes.Add(licitacaoFinal);
                        }
                        else if (existente.DataAtualizacaoPncp != item.DataAtualizacaoPncp)
                        {
                            atualizadasLicitacoes.Add(licitacaoFinal);
                        }
                    }
                    catch (Exception ex)
                    {
                        ignoradas++;
                        _mensagens.AdicionarErro($"Erro ao importar licitação {item.Id}: {ex.Message}");
                    }
                }

                if (novasLicitacoes.Any())
                    await _repositorio.BulkAdicionarAssincrono(novasLicitacoes);

                if (atualizadasLicitacoes.Any())
                    await _repositorio.BulkAtualizarAssincrono(atualizadasLicitacoes);

                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Importação concluída. {novasLicitacoes.Count} novas, {atualizadasLicitacoes.Count} atualizadas e {ignoradas} ignoradas.",
                    TipoMensagem.Sucesso
                );

                return true;
            }
            catch (Exception ex)
            {
                await _transacao.RollbackAssincrono();
                _mensagens.AdicionarErro($"Erro grave na importação: {ex.Message}");
                return false;
            }
        }
    }
}
