using Microsoft.AspNetCore.Mvc;
using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos;
using RSK.API.Controllers;
using RSK.Dominio.Interfaces;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Servicos;

namespace RadarHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadeController : ApiConsultaControllerBase<Unidade>
    {
        private readonly UnidadeServico _unidadeServico;
        private readonly INotificador _mensagens;

        public UnidadeController(IServicoConsultaBase<Unidade> servicoConsulta, INotificador mensagens, UnidadeServico unidadeServico) : base(servicoConsulta)
        {
            _unidadeServico = unidadeServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> Importar()
        {
            var sucesso = await _unidadeServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
