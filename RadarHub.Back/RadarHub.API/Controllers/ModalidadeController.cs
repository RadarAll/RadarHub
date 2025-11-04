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
    public class ModalidadeController : ApiConsultaControllerBase<Modalidade>
    {
        private readonly ModalidadeServico _modalidadeServico;
        private readonly INotificador _mensagens;

        public ModalidadeController(IServicoConsultaBase<Modalidade> servicoConsulta, ModalidadeServico modalidadeServico, INotificador mensagens): base(servicoConsulta)
        {
            _modalidadeServico = modalidadeServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> ImportarModalidades()
        {
            var sucesso = await _modalidadeServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
