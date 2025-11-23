using Microsoft.AspNetCore.Mvc;
using RadarHub.Dominio.DTOs; // Assume-se que este DTO existe
using RadarHub.Dominio.Servicos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RadarHub.API.Controllers
{
    [ApiController]
    [Route("api/recomendacoes")]
    public class RecomendacaoLicitacaoController : ControllerBase
    {
        private readonly RecomendacaoLicitacaoServico _servico;
        // Injeção de IServicoMensagem pode ser adicionada aqui se necessário para recuperar mensagens de erro específicas.

        public RecomendacaoLicitacaoController(RecomendacaoLicitacaoServico servico)
        {
            _servico = servico;
        }

        // =====================================================================
        // 1. ANÁLISE COMPLETA (GATILHO)
        // =====================================================================

        /// <summary>
        /// Inicia a análise de similaridade de todas as licitações com os segmentos.
        /// </summary>
        [HttpPost("analisar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AnalisarSimilaruidades(
            [FromQuery] decimal scoreMinimo = 40m)
        {
            try
            {
                // A flag 'true' ou 'false' indica sucesso na execução da rotina.
                var sucesso = await _servico.AnalisarSimilaruidadesAsync(scoreMinimo);

                if (sucesso)
                {
                    return Ok(new { mensagem = "Análise de similaridades iniciada/concluída com sucesso." });
                }

                // Se a rotina falhou (e.g., erro no banco de dados ou lógica interna)
                return StatusCode(500, new { erro = "Falha ao executar a análise de similaridades." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro interno do servidor: {ex.Message}" });
            }
        }

        // =====================================================================
        // 2. OBTENÇÃO DE RECOMENDAÇÕES
        // =====================================================================

        /// <summary>
        /// Obtém recomendações de licitações para um segmento específico.
        /// </summary>
        [HttpGet("segmento/{segmentoId}")]
        [ProducesResponseType(typeof(List<RecomendacaoLicitacaoDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObterPorSegmento(
            long segmentoId,
            [FromQuery] decimal scoreMinimo = 40m,
            [FromQuery] int top = 50)
        {
            if (segmentoId == null)
            {
                return BadRequest(new { erro = "O ID do segmento é obrigatório." });
            }

            try
            {
                var resultado = await _servico.ObterRecomendacoesPorSegmentoAsync(
                    segmentoId,
                    scoreMinimo,
                    top);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro ao buscar recomendações: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém recomendações de licitações para múltiplos segmentos.
        /// Os IDs dos segmentos devem ser passados na query string (ex: ?segmentoIds=id1&segmentoIds=id2).
        /// </summary>
        [HttpGet("segmentos")]
        [ProducesResponseType(typeof(List<RecomendacaoLicitacaoDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObterPorSegmentos(
            [FromQuery] List<long> segmentoIds,
            [FromQuery] decimal scoreMinimo = 40m,
            [FromQuery] int top = 100)
        {
            if (segmentoIds == null || !segmentoIds.Any())
            {
                return BadRequest(new { erro = "É necessário fornecer pelo menos um ID de segmento." });
            }

            try
            {
                var resultado = await _servico.ObterRecomendacoesPorSegmentosAsync(
                    segmentoIds,
                    scoreMinimo,
                    top);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro ao buscar recomendações: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém recomendações de alta confiança (scoreMinimo: 80) para um segmento específico.
        /// </summary>
        [HttpGet("segmento/{segmentoId}/alta-confianca")]
        [ProducesResponseType(typeof(List<RecomendacaoLicitacaoDto>), 200)]
        public async Task<IActionResult> ObterRecomendacoesAltaConfianca(long segmentoId)
        {
            // Reutiliza a lógica do endpoint ObterPorSegmento, delegando a regra de score para o serviço
            if (segmentoId == null)
            {
                return BadRequest(new { erro = "O ID do segmento é obrigatório." });
            }

            try
            {
                var resultado = await _servico.ObterRecomendacoesAltaConfiancaAsync(segmentoId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro ao buscar recomendações de alta confiança: {ex.Message}" });
            }
        }

        // =====================================================================
        // 3. REMOÇÃO E REANÁLISE
        // =====================================================================

        /// <summary>
        /// Remove todas as recomendações associadas a uma licitação.
        /// </summary>
        [HttpDelete("licitacao/{licitacaoId}")]
        [ProducesResponseType(204)] // No Content - sucesso na remoção
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RemoverRecomendacoesPorLicitacao(long licitacaoId)
        {
            if (licitacaoId == null)
            {
                return BadRequest(new { erro = "O ID da licitação é obrigatório." });
            }

            try
            {
                var sucesso = await _servico.RemoverRecomendacoesPorLicitacaoAsync(licitacaoId);

                if (sucesso)
                {
                    return NoContent(); // Retorna 204
                }

                return StatusCode(500, new { erro = "Falha ao remover recomendações. Verifique os logs." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro interno do servidor: {ex.Message}" });
            }
        }

        /// <summary>
        /// Reanalisa o par específico de Licitação x Segmento.
        /// </summary>
        [HttpPost("reanalisar/{licitacaoId}/{segmentoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReAnalisarPorLicitacaoSegmento(long licitacaoId, long segmentoId)
        {
            if (licitacaoId == null || segmentoId == null)
            {
                return BadRequest(new { erro = "Os IDs de licitação e segmento são obrigatórios." });
            }

            try
            {
                var sucesso = await _servico.ReAnalisarPorLicitacaoSegmentoAsync(licitacaoId, segmentoId);

                if (sucesso)
                {
                    return Ok(new { mensagem = $"Reanálise para Licitação '{licitacaoId}' e Segmento '{segmentoId}' concluída." });
                }

                // A mensagem de erro específica já é adicionada dentro do serviço.
                return StatusCode(400, new { erro = "Falha ao reanalisar. Licitação ou Segmento não encontrados, ou score abaixo do mínimo para salvar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro interno do servidor: {ex.Message}" });
            }
        }
    }
}