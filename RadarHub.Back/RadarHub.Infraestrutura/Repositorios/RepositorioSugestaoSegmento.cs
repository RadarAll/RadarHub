using RadarHub.Dominio.Entidades;
using RSK.Dominio.IRepositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadarHub.Infraestrutura.Repositorios
{
    /// <summary>
    /// Repositório para gerenciar sugestões de segmentos
    /// </summary>
    public class RepositorioSugestaoSegmento : RepositorioBaseRadarHub<SugestaoSegmento>
    {
        public RepositorioSugestaoSegmento(RadarHubDbContext contexto) 
            : base(contexto)
        {
        }

        /// <summary>
        /// Obtém sugestões pendentes de revisão
        /// </summary>
        public async Task<List<SugestaoSegmento>> ObterSugestoesPendentesAsync()
        {
            var resultado = await BuscarAssincrono(x => x.Status == StatusAprovacaoSugestao.Pendente);
            return resultado.ToList();
        }

        /// <summary>
        /// Obtém sugestões aprovadas
        /// </summary>
        public async Task<List<SugestaoSegmento>> ObterSugestoesAprovadosAsync()
        {
            var resultado = await BuscarAssincrono(x => x.Status == StatusAprovacaoSugestao.Aprovada);
            return resultado.ToList();
        }

        /// <summary>
        /// Obtém sugestões rejeitadas
        /// </summary>
        public async Task<List<SugestaoSegmento>> ObterSugestoesRejeitadosAsync()
        {
            var resultado = await BuscarAssincrono(x => x.Status == StatusAprovacaoSugestao.Rejeitada);
            return resultado.ToList();
        }

        /// <summary>
        /// Obtém sugestões por confiança mínima
        /// </summary>
        public async Task<List<SugestaoSegmento>> ObterPorConfiancaAsync(decimal confiancaMinima)
        {
            var resultado = await BuscarAssincrono(x => x.ConfiancaPercentual >= confiancaMinima);
            return resultado.ToList();
        }

        /// <summary>
        /// Obtém sugestões por tipo de origem
        /// </summary>
        public async Task<List<SugestaoSegmento>> ObterPorOrigemAsync(TipoOrigemSugestao origem)
        {
            var resultado = await BuscarAssincrono(x => x.Origem == origem);
            return resultado.ToList();
        }

        /// <summary>
        /// Verifica se já existe sugestão com nome similar
        /// </summary>
        public async Task<bool> ExisteSugestaoComNomeAsync(string nome)
        {
            var todas = await ObterTodosAssincrono();
            var todasLista = todas.ToList();
            return todasLista.Any(x => 
                x.NomeSugerido.Equals(nome, StringComparison.OrdinalIgnoreCase) &&
                x.Status == StatusAprovacaoSugestao.Pendente);
        }

        /// <summary>
        /// Obtém estatísticas de sugestões
        /// </summary>
        public async Task<(int Total, int Pendentes, int Aprovadas, decimal ConfiancaMedia)> ObterEstatisticasAsync()
        {
            var todas = await ObterTodosAssincrono();
            var todasLista = todas.ToList();
            
            var total = todasLista.Count;
            var pendentes = todasLista.Count(x => x.Status == StatusAprovacaoSugestao.Pendente);
            var aprovadas = todasLista.Count(x => x.Status == StatusAprovacaoSugestao.Aprovada);
            var confiancaMedia = todasLista.Any() ? (decimal)todasLista.Average(x => x.ConfiancaPercentual) : 0;

            return (total, pendentes, aprovadas, confiancaMedia);
        }
    }
}
