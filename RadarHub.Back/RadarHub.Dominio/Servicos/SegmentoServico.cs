using RadarHub.Dominio.Entidades;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Servicos;

namespace RadarHub.Dominio.Servicos
{
    public class SegmentoServico : ServicoCrudBase<Segmento>
    {
        public SegmentoServico(IRepositorioBaseAssincrono<Segmento> repositorio, IServicoMensagem mensagens, ITransacao transacao) : base(repositorio, mensagens, transacao)
        {
        }
    }
}
