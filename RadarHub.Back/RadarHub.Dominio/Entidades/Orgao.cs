using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Orgao : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }
        public string Cnpj { get; set; }

        public Orgao(string idTerceiro, string nome, string cnpj) 
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
            this.Cnpj = cnpj;
        }

        private Orgao()
        {

        }
    }
}
