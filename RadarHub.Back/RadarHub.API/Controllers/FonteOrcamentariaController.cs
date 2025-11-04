using Microsoft.AspNetCore.Mvc;
using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos;
using RSK.API.Controllers;
using RSK.Dominio.Interfaces;
using RSK.Dominio.Notificacoes.Interfaces;

namespace RadarHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FonteOrcamentariaController : ApiConsultaControllerBase<FonteOrcamentaria>
    {
        private readonly FonteOrcamentariaServico _tipoServico;
        private readonly INotificador _mensagens;

        public FonteOrcamentariaController(IServicoConsultaBase<FonteOrcamentaria> servicoConsulta, FonteOrcamentariaServico tipoServico, INotificador mensagens) : base(servicoConsulta)
        {
            _tipoServico = tipoServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> Importar()
        {
            var sucesso = await _tipoServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
