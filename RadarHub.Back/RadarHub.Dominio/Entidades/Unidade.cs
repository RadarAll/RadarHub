using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Unidade : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }    
        public string? Codigo { get; set; }
        public string? CodigoNome { get; set; }

        public Unidade(string idTerceiro, string nome, string codigo, string codigoNome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
            this.Codigo = codigo;
            this.CodigoNome = codigoNome;
        }

        private Unidade()
        {

        }
    }
}
