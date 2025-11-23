using RadarHub.Dominio.DTOs;
using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos.Analise;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Entidades;
using RSK.Dominio.Notificacoes.Interfaces;
using System; // Adicionado para DateTime e Exception
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadarHub.Dominio.Servicos
{
    /// <summary>
    /// Serviço responsável por gerenciar recomendações de licitações para segmentos
    /// Realiza análise de similaridade entre licitações e segmentos cadastrados
    /// </summary>
    public class RecomendacaoLicitacaoServico
    {
        private readonly IRepositorioBaseAssincrono<LicitacaoSegmento> _repositorioRecomendacao;
        private readonly IRepositorioBaseAssincrono<Licitacao> _repositorioLicitacao;
        private readonly IRepositorioBaseAssincrono<Segmento> _repositorioSegmento;
        private readonly AnalisadorSimilaridadeServico _analisadorSimilaridade;
        private readonly IServicoMensagem _mensagens;
        private readonly ITransacao _transacao;

        public RecomendacaoLicitacaoServico(
            IRepositorioBaseAssincrono<LicitacaoSegmento> repositorioRecomendacao,
            IRepositorioBaseAssincrono<Licitacao> repositorioLicitacao,
            IRepositorioBaseAssincrono<Segmento> repositorioSegmento,
            IServicoMensagem mensagens,
            ITransacao transacao)
        {
            _repositorioRecomendacao = repositorioRecomendacao;
            _repositorioLicitacao = repositorioLicitacao;
            _repositorioSegmento = repositorioSegmento;
            _analisadorSimilaridade = new AnalisadorSimilaridadeServico();
            _mensagens = mensagens;
            _transacao = transacao;
        }

        /// <summary>
        /// Executa análise completa de similaridade para todas as licitações
        /// Cria recomendações baseadas em segmentos cadastrados
        /// </summary>
        public async Task<bool> AnalisarSimilaruidadesAsync(decimal scoreMinimo = 40m)
        {
            try
            {
                // Obter segmentos e licitações
                var segmentos = await _repositorioSegmento.ObterTodosAssincrono();
                var licitacoes = await _repositorioLicitacao.ObterTodosAssincrono();

                if (!segmentos.Any() || !licitacoes.Any())
                {
                    _mensagens.Adicionar("Nenhum segmento ou licitação encontrado para análise");
                    // CORREÇÃO: Usando CommitAssincrono para consistência com o uso final.
                    // Caso ITransacao tenha apenas CommitAsync, rever a implementação do ITransacao.
                    await _transacao.CommitAssincrono();
                    return true;
                }

                int recomendacoesAdicionadas = 0;
                int recomendacoesAtualizadas = 0;

                // Analisar cada combinação de licitação x segmento
                foreach (var licitacao in licitacoes)
                {
                    foreach (var segmento in segmentos)
                    {
                        var resultado = _analisadorSimilaridade.AnalisarSimilaridade(
                            segmento.Nome,
                            licitacao.Titulo,
                            licitacao.Descricao,
                            null);

                        // Aceitar apenas se score acima do mínimo
                        if (!_analisadorSimilaridade.EhAceitavel(resultado.ScoreSimilaridade))
                            continue;

                        // Verificar se já existe recomendação
                        // CORREÇÃO: ObterPrimeiroAsync não existe. Usa-se BuscarAssincrono e depois FirstOrDefault().
                        var recomendacaoExistente = (await _repositorioRecomendacao
                            .BuscarAssincrono(x =>
                                x.LicitacaoId == licitacao.Id &&
                                x.SegmentoId == segmento.Id))
                            .FirstOrDefault();

                        if (recomendacaoExistente != null)
                        {
                            // Atualizar se o novo score é melhor
                            if (resultado.ScoreSimilaridade > recomendacaoExistente.ScoreSimilaridade)
                            {
                                recomendacaoExistente.ScoreSimilaridade = resultado.ScoreSimilaridade;
                                recomendacaoExistente.TipoCorrespondencia = resultado.TipoCorrespondencia;
                                recomendacaoExistente.TextoEncontrado = resultado.TextoEncontrado;
                                recomendacaoExistente.DataAnalise = DateTime.Now;
                                recomendacaoExistente.DetalhesAnalise = SerializarDetalhes(resultado.DetalhesScores);

                                // CORREÇÃO: AtualizarAssincrono é void. Não usar await.
                                _repositorioRecomendacao.AtualizarAssincrono(recomendacaoExistente);
                                recomendacoesAtualizadas++;
                            }
                        }
                        else
                        {
                            // Criar nova recomendação
                            var novaRecomendacao = new LicitacaoSegmento(
                                licitacao.Id,
                                segmento.Id,
                                resultado.ScoreSimilaridade,
                                resultado.TipoCorrespondencia,
                                resultado.TextoEncontrado)
                            {
                                DetalhesAnalise = SerializarDetalhes(resultado.DetalhesScores)
                            };

                            // CORREÇÃO: AdicionarAsync não existe. Usar AdicionarAssincrono.
                            await _repositorioRecomendacao.AdicionarAssincrono(novaRecomendacao);
                            recomendacoesAdicionadas++;
                        }
                    }
                }

                // CORREÇÃO: Usando CommitAssincrono para consistência.
                await _transacao.CommitAssincrono();

                _mensagens.Adicionar(
                    $"Análise concluída! " +
                    $"Recomendações adicionadas: {recomendacoesAdicionadas}, " +
                    $"atualizadas: {recomendacoesAtualizadas}");

                return true;
            }
            catch (Exception ex)
            {
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao analisar similaridades: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }

        /// <summary>
        /// Obtém recomendações para um segmento específico
        /// </summary>
        public async Task<List<RecomendacaoLicitacaoDto>> ObterRecomendacoesPorSegmentoAsync(
            long segmentoId,
            decimal scoreMinimo = 40m,
            int top = 50)
        {
            // CORREÇÃO: ObterAsync não existe. Usar BuscarAssincrono.
            var recomendacoes = await _repositorioRecomendacao
                .BuscarAssincrono(x =>
                    x.SegmentoId == segmentoId &&
                    x.ScoreSimilaridade >= scoreMinimo);

            var recomendacoesOrdenadas = recomendacoes
                .OrderByDescending(x => x.ScoreSimilaridade)
                .Take(top)
                .ToList();

            // CORREÇÃO: ObterPorIdAsync não existe. Usar ObterPorIdAssincrono.
            var segmento = await _repositorioSegmento.ObterPorIdAssincrono(segmentoId);

            var resultado = new List<RecomendacaoLicitacaoDto>();

            foreach (var recomendacao in recomendacoesOrdenadas)
            {
                // CORREÇÃO: ObterPorIdAsync não existe. Usar ObterPorIdAssincrono.
                var licitacao = await _repositorioLicitacao.ObterPorIdAssincrono(recomendacao.LicitacaoId);

                if (licitacao != null)
                {
                    resultado.Add(new RecomendacaoLicitacaoDto
                    {
                        LicitacaoId = licitacao.Id,
                        SegmentoId = segmento.Id,
                        SegmentoNome = segmento.Nome,
                        LicitacaoTitulo = licitacao.Titulo,
                        ScoreSimilaridade = recomendacao.ScoreSimilaridade,
                        TipoCorrespondencia = recomendacao.TipoCorrespondencia,
                        TextoEncontrado = recomendacao.TextoEncontrado,
                        DataPublicacao = licitacao.DataPublicacaoPncp,
                        OrgaoNome = licitacao.OrgaoIdTerceiro,
                        ValorGlobal = licitacao.ValorGlobal
                    });
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtém recomendações para múltiplos segmentos
        /// </summary>
        public async Task<List<RecomendacaoLicitacaoDto>> ObterRecomendacoesPorSegmentosAsync(
            List<long> segmentoIds,
            decimal scoreMinimo = 40m,
            int top = 100)
        {
            var resultado = new List<RecomendacaoLicitacaoDto>();

            foreach (var segmentoId in segmentoIds)
            {
                // Não precisa de correção aqui, pois chama o método corrigido ObterRecomendacoesPorSegmentoAsync
                var recomendacoes = await ObterRecomendacoesPorSegmentoAsync(segmentoId, scoreMinimo, top);
                resultado.AddRange(recomendacoes);
            }

            // Remover duplicatas e ordenar por score
            return resultado
                .GroupBy(x => x.LicitacaoId)
                .Select(g => g.OrderByDescending(x => x.ScoreSimilaridade).First())
                .OrderByDescending(x => x.ScoreSimilaridade)
                .Take(top)
                .ToList();
        }

        /// <summary>
        /// Obtém recomendações de alta confiança para um segmento
        /// </summary>
        public async Task<List<RecomendacaoLicitacaoDto>> ObterRecomendacoesAltaConfiancaAsync(
            long segmentoId)
        {
            // Não precisa de correção aqui, pois chama o método corrigido ObterRecomendacoesPorSegmentoAsync
            return await ObterRecomendacoesPorSegmentoAsync(segmentoId, scoreMinimo: 80m);
        }

        /// <summary>
        /// Remove recomendações de uma licitação (quando ela é deletada)
        /// </summary>
        public async Task<bool> RemoverRecomendacoesPorLicitacaoAsync(long licitacaoId)
        {
            try
            {

                // CORREÇÃO: ObterAsync não existe. Usar BuscarAssincrono.
                var recomendacoes = await _repositorioRecomendacao
                    .BuscarAssincrono(x => x.LicitacaoId == licitacaoId);

                foreach (var recomendacao in recomendacoes)
                {
                    // CORREÇÃO: RemoverAsync não existe. Usar DeletarAssincrono com o ID.
                    await _repositorioRecomendacao.DeletarAssincrono(recomendacao.Id);
                }

                // CORREÇÃO: Usando CommitAssincrono para consistência.
                await _transacao.CommitAssincrono();
                return true;
            }
            catch (Exception ex)
            {
                await _transacao.RollbackAssincrono();
                _mensagens.Adicionar($"Erro ao remover recomendações: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }

        /// <summary>
        /// Serializa detalhes dos scores para armazenamento
        /// </summary>
        private string SerializarDetalhes(Dictionary<string, decimal> detalhes)
        {
            // Implementação pode usar JSON ou outro formato
            return string.Join("|",
                detalhes.Select(kvp => $"{kvp.Key}:{kvp.Value:F2}"));
        }

        /// <summary>
        /// Reanalisa um par específico de licitação e segmento
        /// </summary>
        public async Task<bool> ReAnalisarPorLicitacaoSegmentoAsync(
            long licitacaoId,
            long segmentoId)
        {
            try
            {
                // CORREÇÃO: ObterPorIdAsync não existe. Usar ObterPorIdAssincrono.
                var licitacao = await _repositorioLicitacao.ObterPorIdAssincrono(licitacaoId);
                // CORREÇÃO: ObterPorIdAsync não existe. Usar ObterPorIdAssincrono.
                var segmento = await _repositorioSegmento.ObterPorIdAssincrono(segmentoId);

                if (licitacao == null || segmento == null)
                {
                    _mensagens.Adicionar("Licitação ou Segmento não encontrado", TipoMensagem.Erro);
                    return false;
                }

                var resultado = _analisadorSimilaridade.AnalisarSimilaridade(
                    segmento.Nome,
                    licitacao.Titulo,
                    licitacao.Descricao,
                    null);

                if (!_analisadorSimilaridade.EhAceitavel(resultado.ScoreSimilaridade))
                {
                    // Remover recomendação se score ficou abaixo do mínimo
                    // CORREÇÃO: ObterPrimeiroAsync não existe. Usa-se BuscarAssincrono e depois FirstOrDefault().
                    var recomendacao = (await _repositorioRecomendacao
                        .BuscarAssincrono(x =>
                            x.LicitacaoId == licitacaoId &&
                            x.SegmentoId == segmentoId))
                        .FirstOrDefault();

                    if (recomendacao != null)
                    {
                        // CORREÇÃO: RemoverAsync não existe. Usar DeletarAssincrono com o ID.
                        await _repositorioRecomendacao.DeletarAssincrono(recomendacao.Id);
                    }
                }
                else
                {
                    // CORREÇÃO: ObterPrimeiroAsync não existe. Usa-se BuscarAssincrono e depois FirstOrDefault().
                    var recomendacaoExistente = (await _repositorioRecomendacao
                        .BuscarAssincrono(x =>
                            x.LicitacaoId == licitacaoId &&
                            x.SegmentoId == segmentoId))
                        .FirstOrDefault();

                    if (recomendacaoExistente != null)
                    {
                        recomendacaoExistente.ScoreSimilaridade = resultado.ScoreSimilaridade;
                        recomendacaoExistente.TipoCorrespondencia = resultado.TipoCorrespondencia;
                        recomendacaoExistente.TextoEncontrado = resultado.TextoEncontrado;
                        recomendacaoExistente.DataAnalise = DateTime.Now;
                        recomendacaoExistente.DetalhesAnalise = SerializarDetalhes(resultado.DetalhesScores);

                        // CORREÇÃO: AtualizarAsync não existe. Usar AtualizarAssincrono (que é void).
                        _repositorioRecomendacao.AtualizarAssincrono(recomendacaoExistente);
                    }
                    else
                    {
                        var novaRecomendacao = new LicitacaoSegmento(
                            licitacaoId,
                            segmentoId,
                            resultado.ScoreSimilaridade,
                            resultado.TipoCorrespondencia,
                            resultado.TextoEncontrado)
                        {
                            DetalhesAnalise = SerializarDetalhes(resultado.DetalhesScores)
                        };

                        // CORREÇÃO: AdicionarAsync não existe. Usar AdicionarAssincrono.
                        await _repositorioRecomendacao.AdicionarAssincrono(novaRecomendacao);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _mensagens.Adicionar($"Erro ao reanalizar: {ex.Message}", TipoMensagem.Erro);
                return false;
            }
        }
    }
}