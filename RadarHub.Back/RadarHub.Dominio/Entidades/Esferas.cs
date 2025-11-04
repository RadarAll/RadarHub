using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Esfera : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set;}

        public Esfera(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private Esfera() { 
        }
    }
}
