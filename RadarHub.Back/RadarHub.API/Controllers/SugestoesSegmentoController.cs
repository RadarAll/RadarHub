using Microsoft.AspNetCore.Mvc;
using RadarHub.Dominio.DTOs;
using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RadarHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SugestoesSegmentoController : ControllerBase
    {
        private readonly SugestorSegmentosServico _sugestorServico;

        public SugestoesSegmentoController(SugestorSegmentosServico sugestorServico)
        {
            _sugestorServico = sugestorServico;
        }

        // =====================================================================
        // GERAR SUGESTÕES
        // =====================================================================

        [HttpGet("sugerir")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SugerirSegmentos(
            [FromQuery] decimal confiancaMinima = 60)
        {
            try
            {
                if (confiancaMinima < 0 || confiancaMinima > 100)
                    return BadRequest(new { erro = "Confiança mínima deve estar entre 0 e 100." });

                // A chamada ao serviço agora considera apenas a confiança mínima (o filtro de pesquisa 'pesquisa'
                // e o parâmetro 'licitações' foram removidos/simplificados com base na análise prévia).
                var resultado = await _sugestorServico.SugerirSegmentosAsync(null, confiancaMinima);

                return Ok(new
                {
                    sucesso = true,
                    total = resultado.Count,
                    sugestoes = resultado,
                    mensagens = _sugestorServico.ObterMensagens()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // =====================================================================
        // SUGESTÕES PENDENTES
        // =====================================================================

        [HttpGet("pendentes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterSugestoesPendentes(
            [FromQuery] decimal? confiancaMinima = null)
        {
            try
            {
                if (confiancaMinima.HasValue && (confiancaMinima.Value < 0 || confiancaMinima.Value > 100))
                    return BadRequest(new { erro = "Confiança mínima deve estar entre 0 e 100." });

                var sugestoes = await _sugestorServico.ObterSugestoesPendentesAssincrono();

                if (confiancaMinima.HasValue)
                    sugestoes = sugestoes
                        .Where(x => x.ConfiancaPercentual >= confiancaMinima.Value)
                        .ToList();

                return Ok(new
                {
                    sucesso = true,
                    total = sugestoes.Count,
                    sugestoes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // =====================================================================
        // APROVAR SUGESTÃO
        // =====================================================================

        [HttpPost("{id:long}/aprovar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AprovarSugestao(long id, [FromBody] AprovarSugestaoSegmentoDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { erro = "ID inválido." });

                if (dto == null || string.IsNullOrWhiteSpace(dto.UsuarioRevisao))
                    return BadRequest(new { erro = "Usuário revisor é obrigatório." });

                var sugestao = await _sugestorServico.ObterSugestaoAsync(id);
                if (sugestao == null)
                    return NotFound(new { erro = "Sugestão não encontrada." });

                var segmento = await _sugestorServico.AprovarSugestaoAsync(sugestao, dto.UsuarioRevisao);

                return Ok(new
                {
                    sucesso = true,
                    mensagem = "Sugestão aprovada com sucesso.",
                    segmento,
                    mensagens = _sugestorServico.ObterMensagens()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // =====================================================================
        // REJEITAR SUGESTÃO
        // =====================================================================

        [HttpPost("{id:long}/rejeitar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RejeitarSugestao(long id, [FromBody] RejeitarSugestaoSegmentoDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { erro = "ID inválido." });

                if (dto == null)
                    return BadRequest(new { erro = "Dados de rejeição são obrigatórios." });

                if (string.IsNullOrWhiteSpace(dto.Motivo))
                    return BadRequest(new { erro = "Motivo é obrigatório." });

                if (string.IsNullOrWhiteSpace(dto.UsuarioRevisao))
                    return BadRequest(new { erro = "Usuário revisor é obrigatório." });

                var sugestao = await _sugestorServico.ObterSugestaoAsync(id);
                if (sugestao == null)
                    return NotFound(new { erro = "Sugestão não encontrada." });

                var resultado = await _sugestorServico.RejeitarSugestaoAsync(sugestao, dto.Motivo, dto.UsuarioRevisao);

                return Ok(new
                {
                    sucesso = resultado,
                    mensagem = "Sugestão rejeitada com sucesso.",
                    mensagens = _sugestorServico.ObterMensagens()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // =====================================================================
        // ESTATÍSTICAS
        // =====================================================================

        [HttpGet("estatisticas")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ObterEstatisticas()
        {
            try
            {
                var (total, pendentes, aprovadas, rejeitadas, confiancaMedia,
                     confiancaMinima, confiancaMaxima) = await _sugestorServico.ObterEstatisticasAsync();

                return Ok(new
                {
                    sucesso = true,
                    total,
                    pendentes,
                    aprovadas,
                    rejeitadas,
                    confiancaMedia = Math.Round(confiancaMedia, 2),
                    confiancaMinima = Math.Round(confiancaMinima, 2),
                    confiancaMaxima = Math.Round(confiancaMaxima, 2)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // =====================================================================
        // DETALHES
        // =====================================================================

        [HttpGet("{id:long}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterDetalhes(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { erro = "ID inválido." });

                var sugestao = await _sugestorServico.ObterSugestaoAsync(id);
                if (sugestao == null)
                    return NotFound(new { erro = "Sugestão não encontrada." });

                return Ok(new
                {
                    sucesso = true,
                    sugestao
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}