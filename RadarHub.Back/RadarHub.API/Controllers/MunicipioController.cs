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
    public class MunicipioController : ApiConsultaControllerBase<Municipio>
    {
        private readonly MunicipioServico _municipioServico;
        private readonly INotificador _mensagens;

        public MunicipioController(IServicoConsultaBase<Municipio> servicoConsulta, MunicipioServico municipioServico, INotificador mensagens) : base(servicoConsulta)
        {
            _municipioServico = municipioServico;
            _mensagens = mensagens;
        }


        [HttpPost("Importar")]
        public async Task<IActionResult> Importar()
        {
            var sucesso = await _municipioServico.ImportarAsync();
            var resultado = _mensagens.ObterMensagens();

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
