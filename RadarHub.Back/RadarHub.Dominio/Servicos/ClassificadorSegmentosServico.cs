using RadarHub.Integracoes.Gemini.Analise;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadarHub.Dominio.Servicos.Analise
{
    /// <summary>
    /// DTO/Record usado para padronizar o retorno dos resultados da análise semântica em lote.
    /// É o contrato de saída do Classificador para o SugestorSegmentosServico.
    /// </summary>
    public record ResultadoAnaliseSemanticaLote(
        long LicitacaoId,
        string NomeSegmentoPrincipal,
        decimal Confianca,
        Dictionary<string, double> TermosRelevantes
    );

    // -------------------------------------------------------------------------

    /// <summary>
    /// Serviço intermediário responsável por orquestrar a classificação semântica de um cluster
    /// de licitações usando o Large Language Model (LLM) do Gemini.
    /// </summary>
    public class ClassificadorSegmentosServico
    {
        private readonly IAnalisadorSemanticoExterno _analisadorGemini;
        private readonly NormalizadorTextoServico _normalizador;

        // Limite de caracteres para evitar estourar o contexto da API do LLM
        private const int MAX_CARACTERES_PROMPT = 25000;

        // O NormalizadorTextoServico precisa ser injetado para extrair palavras-chave do resultado do Gemini
        public ClassificadorSegmentosServico(
            IAnalisadorSemanticoExterno analisadorGemini,
            NormalizadorTextoServico normalizador)
        {
            _analisadorGemini = analisadorGemini;
            _normalizador = normalizador;
        }

        /// <summary>
        /// Agrega títulos e descrições do cluster e chama o Gemini para classificação semântica.
        /// (Usado para a estratégia de Clustering)
        /// </summary>
        /// <returns>Tupla contendo o nome sugerido, a confiança do LLM e termos-chave.</returns>
        public async Task<(string NomeSugestao, decimal ConfiancaPercentual, Dictionary<string, double> TermosRelevantes)> ExtrairNomeDoClusterAsync(
            List<string> titulos,
            List<string> descricoes)
        {
            // 1. Consolida a informação do cluster para criar um prompt robusto
            var textoConsolidado = string.Join(". ", titulos.Concat(descricoes)
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct());

            // Garante que o prompt não exceda o limite de contexto
            if (textoConsolidado.Length > MAX_CARACTERES_PROMPT)
            {
                textoConsolidado = textoConsolidado.Substring(0, MAX_CARACTERES_PROMPT) + " [TEXTO TRUNCADO]";
            }

            // Usa o título mais representativo do cluster (mantido)
            var tituloPrincipal = titulos.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t)) ?? "Licitação Agrupada";


            // 2. 🎯 CHAMA A INTEGRAÇÃO DO GEMINI (Chamada única por cluster)
            // ✅ CORREÇÃO: Empacota o único texto consolidado em uma lista para usar o AnalisarDescricoesEmLoteAsync.
            var dadosParaAnalise = new List<(long LicitacaoId, string Descricao)>
            {
                // Usa 0L como um ID placeholder, pois é um cluster e não uma única licitação.
                (0L, textoConsolidado)
            };

            var resultadosLote = await _analisadorGemini.AnalisarDescricoesEmLoteAsync(
                dadosParaAnalise);

            // Verifica se obteve um resultado.
            if (!resultadosLote.Any())
            {
                // Retorna valores padrão em caso de falha na análise do LLM
                return ("Erro de Classificação", 0m, new Dictionary<string, double>());
            }

            // Extrai o único resultado que esperamos do processamento do cluster
            // (A estrutura ResultadoAnaliseSemantica do Integracoes.Gemini deve ser compatível)
            var resultado = resultadosLote.First();


            // 3. Extrai palavras-chave do segmento sugerido (para persistência no BD)
            var termosBrutos = _normalizador.TokenizarTextoSimples(resultado.NomeSegmentoPrincipal);

            // Mapeia para Dictionary<string, double> para compatibilidade com o SugestorSegmentosServico
            // O valor de 1.0 é um placeholder, pois a relevância real veio do LLM.
            var termosRelevantes = termosBrutos.ToDictionary(
                key => key,
                value => 1.0);

            // 4. Retorna o resultado com a confiança do LLM
            return (resultado.NomeSegmentoPrincipal, resultado.Confianca, termosRelevantes);
        }

        /// <summary>
        /// Realiza a classificação semântica de múltiplas licitações em uma única chamada de lote (Batch)
        /// ao LLM.
        /// (Usado para a estratégia de Classificação Direta, corrigindo o erro 429).
        /// </summary>
        /// <param name="dadosParaAnalise">Lista de tuplas contendo ID e a Descrição da licitação.</param>
        /// <returns>Lista de resultados padronizados (ResultadoAnaliseSemanticaLote).</returns>
        public async Task<List<ResultadoAnaliseSemanticaLote>> ClassificarDiretamenteEmLoteAsync(
            List<(long Id, string Descricao)> dadosParaAnalise)
        {
            if (!dadosParaAnalise.Any())
            {
                return new List<ResultadoAnaliseSemanticaLote>();
            }

            // 1. CHAMA A INTEGRAÇÃO EM LOTE DO GEMINI
            var licitacoesParaLote = dadosParaAnalise.Select(d => (d.Id, d.Descricao)).ToList();
            var resultadosExternos = await _analisadorGemini.AnalisarDescricoesEmLoteAsync(licitacoesParaLote);

            var resultadosFinais = new List<ResultadoAnaliseSemanticaLote>();

            // 2. Processa os resultados do lote, extraindo palavras-chave
            foreach (var resultado in resultadosExternos)
            {
                // Extrai palavras-chave do segmento sugerido
                var termosBrutos = _normalizador.TokenizarTextoSimples(resultado.NomeSegmentoPrincipal);

                // Mapeia para Dictionary<string, double> (relevância é 1.0, pois é determinada pelo LLM)
                var termosRelevantes = termosBrutos.ToDictionary(
                    key => key,
                    value => 1.0);

                // 3. Mapeia para o DTO de retorno
                resultadosFinais.Add(new ResultadoAnaliseSemanticaLote(
                    resultado.LicitacaoId,
                    resultado.NomeSegmentoPrincipal,
                    resultado.Confianca,
                    termosRelevantes
                ));
            }

            return resultadosFinais;
        }
    }
}