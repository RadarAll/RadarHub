using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Ufs: EntidadeBaseImportacaoTerceiro
    {
        public Ufs(string idTerceiro)
        {
            this.IdTerceiro = idTerceiro;
        }

        private Ufs()
        {
        }
    }
}
