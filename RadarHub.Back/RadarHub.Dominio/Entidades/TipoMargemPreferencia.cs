using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class TipoMargemPreferencia : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }

        public TipoMargemPreferencia(string idTerceiro, string name)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = name;
        }

        private TipoMargemPreferencia()
        {

        }
    }
}
