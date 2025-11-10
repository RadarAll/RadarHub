using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadarHub.Integracoes.Pnc
{
    public class LicitacaoDto
    {
        [JsonPropertyName("items")]
        public List<LicitacaoResponse> Items { get; set; }
        [JsonPropertyName("total")]
        public long Total { get; set; }
    }

    public class LicitacaoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("index")]
        public string Index { get; set; }

        [JsonPropertyName("doc_type")]
        public string DocType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("item_url")]
        public string ItemUrl { get; set; }

        [JsonPropertyName("document_type")]
        public string DocumentType { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("ano")]
        public string Ano { get; set; }

        [JsonPropertyName("numero_sequencial")]
        public string NumeroSequencial { get; set; }

        [JsonPropertyName("numero_sequencial_compra_ata")]
        public string NumeroSequencialCompraAta { get; set; }

        [JsonPropertyName("numero_controle_pncp")]
        public string NumeroControlePncp { get; set; }

        [JsonPropertyName("orgao_id")]
        public string OrgaoId { get; set; }

        [JsonPropertyName("orgao_cnpj")]
        public string OrgaoCnpj { get; set; }

        [JsonPropertyName("orgao_nome")]
        public string OrgaoNome { get; set; }

        [JsonPropertyName("orgao_subrogado_id")]
        public string OrgaoSubrogadoId { get; set; }

        [JsonPropertyName("orgao_subrogado_nome")]
        public string OrgaoSubrogadoNome { get; set; }

        [JsonPropertyName("unidade_id")]
        public string UnidadeId { get; set; }

        [JsonPropertyName("unidade_codigo")]
        public string UnidadeCodigo { get; set; }

        [JsonPropertyName("unidade_nome")]
        public string UnidadeNome { get; set; }

        [JsonPropertyName("esfera_id")]
        public string EsferaId { get; set; }

        [JsonPropertyName("esfera_nome")]
        public string EsferaNome { get; set; }

        [JsonPropertyName("poder_id")]
        public string PoderId { get; set; }

        [JsonPropertyName("poder_nome")]
        public string PoderNome { get; set; }

        [JsonPropertyName("municipio_id")]
        public string MunicipioId { get; set; }

        [JsonPropertyName("municipio_nome")]
        public string MunicipioNome { get; set; }

        [JsonPropertyName("uf")]
        public string Uf { get; set; }

        [JsonPropertyName("modalidade_licitacao_id")]
        public string ModalidadeLicitacaoId { get; set; }

        [JsonPropertyName("modalidade_licitacao_nome")]
        public string ModalidadeLicitacaoNome { get; set; }

        [JsonPropertyName("situacao_id")]
        public string SituacaoId { get; set; }

        [JsonPropertyName("situacao_nome")]
        public string SituacaoNome { get; set; }

        [JsonPropertyName("data_publicacao_pncp")]
        public DateTime? DataPublicacaoPncp { get; set; }

        [JsonPropertyName("data_atualizacao_pncp")]
        public DateTime? DataAtualizacaoPncp { get; set; }

        [JsonPropertyName("data_assinatura")]
        public DateTime? DataAssinatura { get; set; }

        [JsonPropertyName("data_inicio_vigencia")]
        public DateTime? DataInicioVigencia { get; set; }

        [JsonPropertyName("data_fim_vigencia")]
        public DateTime? DataFimVigencia { get; set; }

        [JsonPropertyName("cancelado")]
        public bool Cancelado { get; set; }

        [JsonPropertyName("valor_global")]
        public decimal? ValorGlobal { get; set; }

        [JsonPropertyName("tem_resultado")]
        public bool TemResultado { get; set; }

        [JsonPropertyName("tipo_id")]
        public string TipoId { get; set; }

        [JsonPropertyName("tipo_nome")]
        public string TipoNome { get; set; }

        [JsonPropertyName("tipo_contrato_id")]
        public string TipoContratoId { get; set; }

        [JsonPropertyName("tipo_contrato_nome")]
        public string TipoContratoNome { get; set; }

        [JsonPropertyName("fonte_orcamentaria")]
        public JsonElement? FonteOrcamentaria { get; set; }

        [JsonPropertyName("fonte_orcamentaria_id")]
        public JsonElement? FonteOrcamentariaId { get; set; }

        [JsonPropertyName("fonte_orcamentaria_nome")]
        public JsonElement? FonteOrcamentariaNome { get; set; }


        [JsonPropertyName("exigencia_conteudo_nacional")]
        public bool ExigenciaConteudoNacional { get; set; }

        [JsonPropertyName("tipo_margem_preferencia")]
        public JsonElement? TipoMargemPreferencia { get; set; }

        [JsonPropertyName("tipo_margem_preferencia_id")]
        public JsonElement? TipoMargemPreferenciaId { get; set; }

        [JsonPropertyName("tipo_margem_preferencia_nome")]
        public JsonElement? TipoMargemPreferenciaNome { get; set; }

    }

}
