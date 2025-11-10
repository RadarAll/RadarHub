using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    public class Licitacao : EntidadeBaseImportacaoTerceiro
    {
        
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string ItemUrl { get; set; }
        public decimal? ValorGlobal { get; set; }
        public bool TemResultado { get; set; }


        public DateTime? DataPublicacaoPncp { get; set; }
        public DateTime? DataAtualizacaoPncp { get; set; }
        public DateTime? DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }



        public string Ano { get; set; }
        public string? NumeroSequencial { get; set; }
        public string? NumeroSequencialCompraAta { get; set; }
        public string? NumeroControlePncp { get; set; }

        public string? OrgaoIdTerceiro { get; set; }
        public string? MunicipioIdTerceiro { get; set; }
        public string? ModalidadeIdTerceiro { get; set; }
        public string? TipoIdTerceiro { get; set; }
        public string? UfIdTerceiro { get; set; }
        public string? FonteOrcamentariaIdTerceiro { get; set; }
        public string? TipoMargemPreferenciaIdTerceiro { get; set; }
        public string? UnidadeIdTerceiro { get; set; }
        public string? PoderIdTerceiro { get; set; }
        public string? EsferaIdTerceiro { get; set; }


        public string? OrgaoSubrogadoId { get; set; }
        public string? OrgaoSubrogadoNome { get; set; }

        public bool ExigenciaConteudoNacional { get; set; }


        
        public Licitacao() { }
    }
}