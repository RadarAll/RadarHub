using System;
using System.Collections.Generic;
using System.Linq;

namespace RadarHub.Dominio.Servicos
{
    /// <summary>
    /// Serviço de cálculo TF-IDF para extrair termos mais discriminativos/relevantes em um corpus.
    /// Utiliza NormalizadorTextoServico para garantir consistência no pré-processamento.
    /// </summary>
    public class AnalisadorTFIDFServico
    {
        private readonly NormalizadorTextoServico _normalizador;

        public AnalisadorTFIDFServico(NormalizadorTextoServico normalizador)
        {
            _normalizador = normalizador;
        }

        /// <summary>
        /// Calcula TF-IDF para um conjunto de documentos (corpus).
        /// Retorna uma lista de dicionários, onde cada dicionário representa um documento
        /// e contém o score TF-IDF de cada termo naquele documento.
        /// Este formato permite que o serviço consumidor agregue os scores por termo.
        /// </summary>
        /// <param name="documentos">Lista de strings, onde cada string é um documento/licitação.</param>
        /// <returns>Lista de Dictionaries, onde cada Dictionary é: Key=termo (string), Value=TFIDF Score (double).</returns>
        public List<Dictionary<string, double>> CalcularTFIDFParaCorpus(List<string> documentos, int minTamanhoPalavra = 3)
        {
            if (documentos == null || documentos.Count == 0)
                return new List<Dictionary<string, double>>();

            // 1. Tokenizar todos os documentos usando o Normalizador para consistência
            var tokenizados = documentos.Select(d => TokenizarComFiltro(d, minTamanhoPalavra)).ToList();

            // 2. Calcular DF (Document Frequency) - em quantos docs cada termo aparece
            var df = new Dictionary<string, int>();
            foreach (var doc in tokenizados)
            {
                var termosPorDoc = doc.Distinct().ToList();
                foreach (var termo in termosPorDoc)
                {
                    if (!df.ContainsKey(termo))
                        df[termo] = 0;
                    df[termo]++;
                }
            }

            // 3. Calcular IDF (Inverse Document Frequency)
            var idf = new Dictionary<string, double>();
            var totalDocs = documentos.Count;

            foreach (var kvp in df)
            {
                // Fórmula IDF: log(N / df)
                idf[kvp.Key] = Math.Log(totalDocs / (double)kvp.Value);
            }

            // 4. Calcular TF-IDF por documento
            var resultadosPorDocumento = new List<Dictionary<string, double>>();

            foreach (var docTokens in tokenizados)
            {
                var resultadoDoc = new Dictionary<string, double>();
                var totalTermosDoc = docTokens.Count;

                if (totalTermosDoc == 0)
                {
                    resultadosPorDocumento.Add(resultadoDoc);
                    continue;
                }

                // Calcular Term Frequency (TF)
                var tfPorTermo = docTokens
                    .GroupBy(t => t)
                    .ToDictionary(g => g.Key, g => g.Count() / (double)totalTermosDoc);

                // Calcular TF-IDF = TF * IDF
                foreach (var tfKvp in tfPorTermo)
                {
                    var termo = tfKvp.Key;
                    var tf = tfKvp.Value;
                    // O termo deve existir em IDF, mas verifica-se por segurança.
                    var idfScore = idf.ContainsKey(termo) ? idf[termo] : 0;

                    resultadoDoc[termo] = tf * idfScore;
                }

                resultadosPorDocumento.Add(resultadoDoc);
            }

            return resultadosPorDocumento;
        }

        /// <summary>
        /// Realiza tokenização, normalização, remoção de stopwords e filtragem de tamanho,
        /// garantindo que a lista de termos esteja pronta para o cálculo TF-IDF.
        /// </summary>
        private List<string> TokenizarComFiltro(string texto, int minTamanhoPalavra)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<string>();

            // 1. Tokenização bruta (baseada na estrutura do NormalizadorTextoServico)
            var tokens = texto
                .ToLowerInvariant()
                .Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '-', '(', ')', '[', ']', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            // 2. Utiliza o Normalizador para remover stopwords e filtrar palavras curtas,
            // garantindo a consistência com o resto da aplicação.
            return _normalizador.RemoverStopwords(tokens, minTamanhoPalavra);
        }

        // Os métodos CalcularTFIDF (agregado), CalcularTFIDFPorDocumento (individual) e CalcularSimilaridade 
        // da versão anterior foram removidos, pois não são utilizados no novo fluxo otimizado.
    }
}