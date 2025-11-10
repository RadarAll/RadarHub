using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RSK.Dominio.Entidades;

namespace RadarHub.Dominio.Entidades
{
    /// <summary>
    /// Representa o plano de assinatura da empresa, definindo os limites de uso da plataforma.
    /// </summary>
    public class Plano : EntidadeBase
    {
        /// <summary>
        /// Nome do plano (Ex: Básico, Intermediário, Premium).
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Descrição detalhada do plano e seus benefícios.
        /// </summary>
        [StringLength(500)]
        public string Descricao { get; set; }

        /// <summary>
        /// O limite máximo de licitações recomendadas que o serviço pode retornar.
        /// Este valor é usado no método Take() do serviço de recomendação.
        /// </summary>
        [Required]
        public int LimiteRecomendacoes { get; set; }

        /// <summary>
        /// O valor da mensalidade do plano (Ex: 99.90).
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorMensal { get; set; }

        /// <summary>
        /// Indica se o plano está ativo e disponível para contratação.
        /// </summary>
        public bool Ativo { get; set; } = true;

        // ==========================================================
        // CONSTRUTORES
        // ==========================================================

        public Plano(string nome, string descricao, int limiteRecomendacoes, decimal valorMensal)
        {
            this.Nome = nome;
            this.Descricao = descricao;
            this.LimiteRecomendacoes = limiteRecomendacoes;
            this.ValorMensal = valorMensal;
        }

        private Plano()
        {
        }
    }
}