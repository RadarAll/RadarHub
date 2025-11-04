//using Microsoft.AspNetCore.Mvc;
//using RadarGov.Integracoes.Gemini;
//using RadarGov.Integracoes.DTOs;

//namespace RadarGov.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SegmentoController : ControllerBase
//    {
//        private readonly SegmentoClassifierService _classifierService;

//        public SegmentoController(SegmentoClassifierService classifierService)
//        {
//            _classifierService = classifierService;
//        }


//        [HttpPost("classificar-teste")]
//        public async Task<IActionResult> ClassificarSegmentoTeste([FromBody] ClassificacaoRequest request)
//        {
//            if (string.IsNullOrWhiteSpace(request.Titulo) && string.IsNullOrWhiteSpace(request.Descricao))
//            {
//                return BadRequest("O Título ou a Descrição devem ser fornecidos para classificação.");
//            }

//            string? segmento = await _classifierService.ClassificarSegmentoAsync(
//                request.Titulo ?? string.Empty,
//                request.Descricao ?? string.Empty
//            );

//            if (segmento == null)
//            {
//                return StatusCode(500, "Falha ao classificar o segmento. Verifique logs do serviço GeminiApiClient.");
//            }

//            if (segmento == "SERVIÇOS DIVERSOS")
//            {
//                return Ok($"Classificado como: {segmento} (Segmento de fallback)");
//            }

//            return Ok($"Segmento Classificado com Sucesso: {segmento}");
//        }
//    }

//}