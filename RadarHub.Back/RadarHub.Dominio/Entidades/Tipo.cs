using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Tipo : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }    

        public Tipo(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private Tipo() 
        {
        }
    }
}
