using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Poder : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }

        public Poder(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private Poder()
        {

        }
    }
}
