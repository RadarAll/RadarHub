namespace RadarHub.Integracoes.Gemini.Analise
{
    /// <summary>
    /// Objeto de dados (Data Transfer Object) para a resposta do modelo Gemini.
    /// </summary>
    public class ResultadoAnaliseSemantica
    {
        // O nome limpo e comercial do segmento (Ex: "Material de Escritório")
        public string NomeSegmentoPrincipal { get; set; }
        // Confiança do modelo na classificação, de 0 a 100
        public decimal Confianca { get; set; }
        // Justificativa para auditoria
        public long LicitacaoId { get; set; }
        public string Justificativa { get; set; }
    }

    /// <summary>
    /// Interface para serviços de PLN avançados (utilizando APIs de terceiros como Gemini).
    /// </summary>
    public interface IAnalisadorSemanticoExterno
    {
        /// <summary>
        /// Analisa o texto de uma licitação e retorna o objeto limpo/semântico.
        /// </summary>
        Task<List<ResultadoAnaliseSemantica>> AnalisarDescricoesEmLoteAsync(List<(long LicitacaoId, string Descricao)> licitacoes);
    }
}