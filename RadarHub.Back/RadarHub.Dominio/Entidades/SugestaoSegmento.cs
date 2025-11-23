using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    /// <summary>
    /// Representa uma sugestão de novo segmento baseado em análise de licitações
    /// Permite descobrir automaticamente novos segmentos de mercado
    /// </summary>
    public class SugestaoSegmento : EntidadeBase
    {
        /// <summary>
        /// Nome sugerido para o novo segmento
        /// </summary>
    public string? NomeSugerido { get; set; }

        /// <summary>
        /// Descrição sugerida para o segmento
        /// </summary>
    public string? DescricaoSugerida { get; set; }

        /// <summary>
        /// Palavras-chave sugeridas para o segmento
        /// Armazenadas como JSON array ou string separada por vírgula
        /// </summary>
    public string? PalavrasChaveSugeridas { get; set; }

        /// <summary>
        /// Quantidade de licitações que originaram esta sugestão
        /// </summary>
        public int QuantidadeLicitacoesOriginarias { get; set; }

        /// <summary>
        /// Percentual de confiança da sugestão (0-100)
        /// Quanto mais alto, melhor a sugestão
        /// </summary>
        public decimal ConfiancaPercentual { get; set; }

        /// <summary>
        /// Tipo/origem da sugestão (qual método gerou)
        /// </summary>
        public TipoOrigemSugestao Origem { get; set; }

        /// <summary>
        /// Status da sugestão no fluxo de aprovação
        /// </summary>
        public StatusAprovacaoSugestao Status { get; set; }

        /// <summary>
        /// Data de revisão (quando foi aprovada/rejeitada)
        /// </summary>
        public DateTime? DataRevisao { get; set; }

        /// <summary>
        /// Usuário que fez a revisão
        /// </summary>
    public string? UsuarioRevisao { get; set; }

        /// <summary>
        /// Motivo da rejeição (se aplicável)
        /// </summary>
    public string? MotivoRejeicao { get; set; }

        /// <summary>
        /// ID do segmento criado após aprovação (FK)
        /// </summary>
        public long? SegmentoIdGerado { get; set; }

        /// <summary>
        /// IDs das licitações que originaram a sugestão
        /// Armazenadas como JSON array
        /// </summary>
    public string? LicitacaoIds { get; set; }

        public SugestaoSegmento() { }

        public SugestaoSegmento(
            string nomeSugerido,
            string descricaoSugerida,
            string palavrasChaveSugeridas,
            int quantidadeLicitacoes,
            decimal confiancaPercentual,
            TipoOrigemSugestao origem)
        {
            NomeSugerido = nomeSugerido;
            DescricaoSugerida = descricaoSugerida;
            PalavrasChaveSugeridas = palavrasChaveSugeridas;
            QuantidadeLicitacoesOriginarias = quantidadeLicitacoes;
            ConfiancaPercentual = confiancaPercentual;
            Origem = origem;
            Status = StatusAprovacaoSugestao.Pendente;
        }
    }

    /// <summary>
    /// Tipo/Origem da sugestão de segmento
    /// Indica qual método foi utilizado para gerar
    /// </summary>
    public enum TipoOrigemSugestao
    {
        /// <summary>
        /// Gerado por agrupamento de licitações similares
        /// </summary>
        Clustering = 0,

        /// <summary>
        /// Gerado por análise de palavras-chave frequentes
        /// </summary>
        PalavrasChave = 1,

        /// <summary>
        /// Gerado por análise temática avançada (LDA)
        /// </summary>
        AnaliseLDA = 2,

        /// <summary>
        /// Sugestão manual do usuário
        /// </summary>
        Manual = 3,
        ClassificacaoDiretaLLM= 4
    }

    /// <summary>
    /// Status da sugestão no fluxo de aprovação
    /// </summary>
    public enum StatusAprovacaoSugestao
    {
        /// <summary>
        /// Aguardando revisão do usuário
        /// </summary>
        Pendente = 0,

        /// <summary>
        /// Aprovada e segmento foi criado
        /// </summary>
        Aprovada = 1,

        /// <summary>
        /// Rejeitada pelo usuário
        /// </summary>
        Rejeitada = 2,

        /// <summary>
        /// Descartada automaticamente (duplicata, baixa confiança, etc)
        /// </summary>
        Descartada = 3
    }
}
