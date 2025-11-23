using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    /// <summary>
    /// Representa a relação entre uma Licitação e um Segmento,
    /// indicando a similaridade e recomendação.
    /// </summary>
    public class LicitacaoSegmento : EntidadeBase
    {
        /// <summary>
        /// ID da licitação relacionada
        /// </summary>
        public long LicitacaoId { get; set; }

        /// <summary>
        /// ID do segmento relacionado
        /// </summary>
        public long SegmentoId { get; set; }

        /// <summary>
        /// Score de similaridade entre 0 e 100
        /// Quanto mais alto, maior a chance de recomendação ser relevante
        /// </summary>
        public decimal ScoreSimilaridade { get; set; }

        /// <summary>
        /// Tipo de correspondência encontrada
        /// </summary>
        public TipoCorrespondencia TipoCorrespondencia { get; set; }

        /// <summary>
        /// Data em que a análise foi realizada
        /// </summary>
        public DateTime DataAnalise { get; set; }

        /// <summary>
        /// Texto específico que gerou a correspondência
        /// Útil para auditoria e compreensão da recomendação
        /// </summary>
        public string TextoEncontrado { get; set; }

        /// <summary>
        /// Campo interno para armazenar detalhes da análise em JSON
        /// Ex: {"metodos": ["levenshtein", "jaccard"], "scores": {...}}
        /// </summary>
        public string DetalhesAnalise { get; set; }

        public LicitacaoSegmento() { }

        public LicitacaoSegmento(
            long licitacaoId,
            long segmentoId,
            decimal scoreSimilaridade,
            TipoCorrespondencia tipoCorrespondencia,
            string textoEncontrado)
        {
            LicitacaoId = licitacaoId;
            SegmentoId = segmentoId;
            ScoreSimilaridade = scoreSimilaridade;
            TipoCorrespondencia = tipoCorrespondencia;
            TextoEncontrado = textoEncontrado;
            DataAnalise = DateTime.Now;
        }
    }

    /// <summary>
    /// Tipos de correspondência encontrados entre Licitação e Segmento
    /// </summary>
    public enum TipoCorrespondencia
    {
        /// <summary>
        /// Nome do segmento encontrado no título ou descrição
        /// </summary>
        NomeSegmento = 0,

        /// <summary>
        /// Palavras-chave do segmento encontradas
        /// </summary>
        PalavrasChave = 1,

        /// <summary>
        /// Análise semântica de similaridade entre textos
        /// </summary>
        ConteudoSimilar = 2,

        /// <summary>
        /// Match exato com alta confiança
        /// </summary>
        MatchExato = 3,

        /// <summary>
        /// Relacionamento criado automaticamente ao aprovar sugestão de segmento
        /// </summary>
        SugestaoAprovada = 4
    }
}
