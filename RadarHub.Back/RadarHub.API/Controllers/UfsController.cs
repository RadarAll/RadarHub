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
    public class UfsController : ApiConsultaControllerBase<Ufs>
    {
        private readonly UfsServico _ufsServico;
        private readonly INotificador _mensagens;

        public UfsController(IServicoConsultaBase<Ufs> servicoConsulta, UfsServico ufsServico, INotificador mensagens) : base(servicoConsulta)
        {
            _ufsServico = ufsServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> Importar()
        {
            var sucesso = await _ufsServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
