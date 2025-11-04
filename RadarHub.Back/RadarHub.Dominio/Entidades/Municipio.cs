using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Municipio : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }

        public Municipio(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private Municipio()
        {
        }

    }
}
