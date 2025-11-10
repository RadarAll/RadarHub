using RadarHub.Dominio.Entidades;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Servicos;

namespace RadarHub.Dominio.Servicos
{
    public class PlanoServico : ServicoCrudBase<Plano>
    {
        public PlanoServico(IRepositorioBaseAssincrono<Plano> repositorio, IServicoMensagem mensagens, ITransacao transacao) : base(repositorio, mensagens, transacao)
        {
        }
    }
}
