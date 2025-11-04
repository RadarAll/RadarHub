using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Modalidade : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }

        public Modalidade(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private Modalidade()
        {
        }

    }
}
