using RadarHub.Dominio.Entidades;
using RSK.Dominio.IRepositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadarHub.Infraestrutura.Repositorios
{
    /// <summary>
    /// Interface do repositório para sugestões de segmentos
    /// </summary>
    public interface IRepositorioSugestaoSegmento : IRepositorioBaseAssincrono<SugestaoSegmento>
    {
        /// <summary>
        /// Obtém sugestões pendentes de revisão
        /// </summary>
        Task<List<SugestaoSegmento>> ObterSugestoesPendentesAsync();

        /// <summary>
        /// Obtém sugestões aprovadas
        /// </summary>
        Task<List<SugestaoSegmento>> ObterSugestoesAprovadosAsync();

        /// <summary>
        /// Obtém sugestões rejeitadas
        /// </summary>
        Task<List<SugestaoSegmento>> ObterSugestoesRejeitadosAsync();

        /// <summary>
        /// Obtém sugestões por confiança mínima
        /// </summary>
        Task<List<SugestaoSegmento>> ObterPorConfiancaAsync(decimal confiancaMinima);

        /// <summary>
        /// Obtém sugestões por tipo de origem
        /// </summary>
        Task<List<SugestaoSegmento>> ObterPorOrigemAsync(TipoOrigemSugestao origem);

        /// <summary>
        /// Verifica se já existe sugestão com nome similar
        /// </summary>
        Task<bool> ExisteSugestaoComNomeAsync(string nome);

        /// <summary>
        /// Obtém estatísticas de sugestões
        /// </summary>
        Task<(int Total, int Pendentes, int Aprovadas, decimal ConfiancaMedia)> ObterEstatisticasAsync();
    }
}
