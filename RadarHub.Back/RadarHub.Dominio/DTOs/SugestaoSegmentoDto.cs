using RadarHub.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace RadarHub.Dominio.DTOs
{
    /// <summary>
    /// DTO para retornar sugestão de segmento
    /// </summary>
    public class SugestaoSegmentoDto
    {
        public string Id { get; set; }

        public string NomeSugerido { get; set; }

        public string DescricaoSugerida { get; set; }

        public List<string> PalavrasChaveSugeridas { get; set; } = new();

        public int QuantidadeLicitacoesOriginarias { get; set; }

        public decimal ConfiancaPercentual { get; set; }

        public string Origem { get; set; }

        public string Status { get; set; }

        public DateTime DataSugestao { get; set; }

        public DateTime? DataRevisao { get; set; }

        public string UsuarioRevisao { get; set; }

        public string MotivoRejeicao { get; set; }

        public List<string> LicitacaoIds { get; set; } = new();

        /// <summary>
        /// Retorna nível de confiança em texto
        /// </summary>
        public string NivelConfianca => ConfiancaPercentual switch
        {
            >= 80 => "Muito Alta",
            >= 70 => "Alta",
            >= 60 => "Média",
            >= 50 => "Baixa",
            _ => "Muito Baixa"
        };

        /// <summary>
        /// Retorna cor para exibição no frontend
        /// </summary>
        public string CorConfianca => ConfiancaPercentual switch
        {
            >= 80 => "success",      // Verde
            >= 70 => "info",         // Azul
            >= 60 => "warning",      // Amarelo
            _ => "danger"            // Vermelho
        };

        /// <summary>
        /// Descrição legível do tipo de origem
        /// </summary>
        public string DescricaoOrigem => Origem switch
        {
            "Clustering" => "Agrupamento de Licitações",
            "PalavrasChave" => "Palavras-chave Frequentes",
            "AnaliseLDA" => "Análise Temática",
            "Manual" => "Sugestão Manual",
            _ => "Desconhecido"
        };

        /// <summary>
        /// Descrição legível do status
        /// </summary>
        public string DescricaoStatus => Status switch
        {
            "Pendente" => "Aguardando Revisão",
            "Aprovada" => "Segmento Criado",
            "Rejeitada" => "Rejeitada",
            "Descartada" => "Descartada",
            _ => "Desconhecido"
        };
    }

    /// <summary>
    /// DTO para requisição de aprovação de sugestão
    /// </summary>
    public class AprovarSugestaoDto
    {
        public string SugestaoId { get; set; }
        public string UsuarioRevisao { get; set; }
        public string NomeFinal { get; set; } // Permite editar antes de aprovar
    }

    /// <summary>
    /// DTO para requisição de rejeição de sugestão
    /// </summary>
    public class RejeitarSugestaoDto
    {
        public string SugestaoId { get; set; }
        public string UsuarioRevisao { get; set; }
        public string Motivo { get; set; }
    }

    /// <summary>
    /// DTO para filtrar sugestões
    /// </summary>
    public class FiltroSugestaoSegmentoDto
    {
        public StatusAprovacaoSugestao? Status { get; set; }
        public decimal? ConfiancaMinimaPercentual { get; set; }
        public TipoOrigemSugestao? Origem { get; set; }
        public int Top { get; set; } = 50;
    }

    /// <summary>
    /// DTO para resumo de sugestões
    /// </summary>
    public class ResumoSugestoesDto
    {
        public int TotalSugestoes { get; set; }
        public int TotalPendentes { get; set; }
        public int TotalAprovadas { get; set; }
        public int TotalRejeitadas { get; set; }
        public decimal ConfiancaMedia { get; set; }
        public decimal PercentualPendentes { get; set; }
        public List<SugestaoSegmentoDto> SugestoesPendentes { get; set; } = new();
    }
}
