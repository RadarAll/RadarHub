using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos;
using RSK.Dominio.Interfaces;
using RSK.Dominio.Notificacoes.Interfaces;

namespace RSK.API.Controllers
{
    public class PlanoController : ApiCrudControllerBase<Plano>
    {

        public PlanoController(
            IServicoConsultaBase<Plano> servicoConsulta,
            PlanoServico servicoUsuario,
            INotificador notificador
        ) : base(servicoConsulta, servicoUsuario, notificador)
        {
        }

    }
}
