using RadarHub.Dominio.Entidades;

namespace RadarHub.Dominio.DTOs
{
    /// <summary>
    /// DTO que representa uma recomendação de licitação para um segmento
    /// </summary>
    public class RecomendacaoLicitacaoDto
    {
        /// <summary>
        /// ID único da licitação
        /// </summary>
        public long LicitacaoId { get; set; }

        /// <summary>
        /// ID único do segmento
        /// </summary>
        public long SegmentoId { get; set; }

        /// <summary>
        /// Nome do segmento
        /// </summary>
        public string SegmentoNome { get; set; }

        /// <summary>
        /// Título da licitação
        /// </summary>
        public string LicitacaoTitulo { get; set; }

        /// <summary>
        /// Score de similaridade entre 0 e 100
        /// Indica o grau de confiança da recomendação
        /// </summary>
        public decimal ScoreSimilaridade { get; set; }

        /// <summary>
        /// Tipo de correspondência encontrada
        /// </summary>
        public TipoCorrespondencia TipoCorrespondencia { get; set; }

        /// <summary>
        /// Texto específico que gerou a correspondência
        /// Pode ser o nome do segmento, palavra-chave, ou parte do título
        /// </summary>
        public string TextoEncontrado { get; set; }

        /// <summary>
        /// Data de publicação da licitação no PNCP
        /// </summary>
        public DateTime? DataPublicacao { get; set; }

        /// <summary>
        /// Nome do órgão responsável pela licitação
        /// </summary>
        public string OrgaoNome { get; set; }

        /// <summary>
        /// Valor global da licitação
        /// </summary>
        public decimal? ValorGlobal { get; set; }

        /// <summary>
        /// Indica se é uma recomendação de alta confiança (score >= 80)
        /// </summary>
        public bool EhAltaConfianca => ScoreSimilaridade >= 80m;

        /// <summary>
        /// Retorna uma descrição legível do tipo de correspondência
        /// </summary>
        public string DescricaoTipo => TipoCorrespondencia switch
        {
            TipoCorrespondencia.MatchExato => "Correspondência Exata",
            TipoCorrespondencia.NomeSegmento => "Nome do Segmento Encontrado",
            TipoCorrespondencia.PalavrasChave => "Palavras-chave Encontradas",
            TipoCorrespondencia.ConteudoSimilar => "Conteúdo Similar",
            _ => "Desconhecido"
        };

        /// <summary>
        /// Retorna uma cor para visualização (verde para alta confiança, amarelo para média, etc)
        /// </summary>
        public string CorConfianca => ScoreSimilaridade switch
        {
            >= 80 => "success",      // Verde
            >= 60 => "warning",      // Amarelo
            >= 40 => "info",         // Azul
            _ => "secondary"         // Cinza
        };
    }

    /// <summary>
    /// Wrapper para retornar paginação de recomendações
    /// </summary>
    public class ListaRecomendacoesDto
    {
        public List<RecomendacaoLicitacaoDto> Recomendacoes { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanhosPagina { get; set; }
        public decimal ScoreMedioSegmentos { get; set; }
        public int TotalAltaConfianca { get; set; }
    }

    /// <summary>
    /// DTO para filtrar recomendações
    /// </summary>
    public class FiltroRecomendacaoDto
    {
        /// <summary>
        /// IDs dos segmentos para filtrar
        /// </summary>
        public List<string> SegmentoIds { get; set; } = new();

        /// <summary>
        /// Score mínimo desejado
        /// </summary>
        public decimal? ScoreMinimo { get; set; }

        /// <summary>
        /// Tipos de correspondência para filtrar
        /// </summary>
        public List<TipoCorrespondencia> TiposCorrespondencia { get; set; } = new();

        /// <summary>
        /// Apenas recomendações de alta confiança
        /// </summary>
        public bool ApenasAltaConfianca { get; set; }

        /// <summary>
        /// Ordenação desejada
        /// </summary>
        public OrdenacaoRecomendacao Ordenacao { get; set; } = OrdenacaoRecomendacao.ScoreMaisAlto;

        /// <summary>
        /// Número máximo de resultados
        /// </summary>
        public int Top { get; set; } = 50;
    }

    /// <summary>
    /// Opções de ordenação para recomendações
    /// </summary>
    public enum OrdenacaoRecomendacao
    {
        ScoreMaisAlto = 0,
        ScoreMaisBaixo = 1,
        MaisRecente = 2,
        MaisAntiga = 3,
        MaiorValor = 4,
        MenorValor = 5
    }

    /// <summary>
    /// Resumo de recomendações para um segmento
    /// </summary>
    public class ResumoRecomendacaoDto
    {
        public string SegmentoId { get; set; }
        public string SegmentoNome { get; set; }
        public int TotalRecomendacoes { get; set; }
        public int TotalAltaConfianca { get; set; }
        public decimal ScoreMedio { get; set; }
        public decimal? ValorTotalRecomendado { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
    }
}
