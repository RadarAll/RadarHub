using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Segmento : EntidadeBase
    {
        public string Nome { get; set; }

        public Segmento(string nome)
        {
            Nome = nome;
        }

        private Segmento() { }

    }
}
