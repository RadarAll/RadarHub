using RSK.Dominio.Entidades;
using RSK.Dominio.IRepositorios;
using RSK.Infraestrutura.Repositorios;

namespace RadarHub.Infraestrutura.Repositorios
{
    public class RepositorioBaseRadarHub<TEntity>
    : RepositorioBaseAssincrono<TEntity, RadarHubDbContext>, IRepositorioBaseAssincrono<TEntity>
    where TEntity : EntidadeBase
    {
        public RepositorioBaseRadarHub(RadarHubDbContext context) : base(context) { }
    }

}
