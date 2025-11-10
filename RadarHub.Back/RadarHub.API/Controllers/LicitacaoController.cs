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
    public class LicitacaoController : ApiConsultaControllerBase<Licitacao>
    {
        private readonly LicitacaoServico _licitacaoServico;
        private readonly INotificador _mensagens;

        public LicitacaoController(IServicoConsultaBase<Licitacao> servicoConsulta, LicitacaoServico licitacaoServico, INotificador mensagens): base(servicoConsulta)
        {
            _licitacaoServico = licitacaoServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> Importar()
        {
            var sucesso = await _licitacaoServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
