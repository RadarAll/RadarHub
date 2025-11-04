using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class FonteOrcamentaria : EntidadeBaseImportacaoTerceiro
    {
        public string Nome { get; set; }

        public FonteOrcamentaria(string idTerceiro, string nome)
        {
            this.IdTerceiro = idTerceiro;
            this.Nome = nome;
        }

        private FonteOrcamentaria()
        {

        }
    }

}
