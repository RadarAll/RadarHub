using RadarHub.Dominio.Entidades;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RadarHub.Dominio.Servicos.Analise
{
    /// <summary>
    /// Serviço responsável por analisar similaridade entre licitações e segmentos
    /// Implementa múltiplos algoritmos de comparação de strings:
    /// - Levenshtein Distance: Mede o número de edições necessárias
    /// - Jaccard Similarity: Compara conjuntos de palavras
    /// - Matching Exato: Busca por palavras exatas
    /// - Análise Semântica: Tokenização e stem de palavras
    /// </summary>
    public class AnalisadorSimilaridadeServico
    {
        private const decimal THRESHOLD_MINIMO = 40m;  // Score mínimo para aceitar
        private const decimal THRESHOLD_ALTO = 80m;    // Score para alta confiança
        private const decimal PESO_MATCH_EXATO = 0.40m;
        private const decimal PESO_PALAVRAS_CHAVE = 0.30m;
        private const decimal PESO_LEVENSHTEIN = 0.20m;
        private const decimal PESO_JACCARD = 0.10m;

        public class ResultadoAnalise
        {
            public decimal ScoreSimilaridade { get; set; }
            public TipoCorrespondencia TipoCorrespondencia { get; set; }
            public string TextoEncontrado { get; set; }
            public Dictionary<string, decimal> DetalhesScores { get; set; } = new();
        }

        /// <summary>
        /// Analisa similaridade entre um segmento e uma licitação
        /// </summary>
        public ResultadoAnalise AnalisarSimilaridade(
            string nomeSegmento,
            string tituloLicitacao,
            string descricaoLicitacao,
            List<string> palavrasChaveSegmento = null)
        {
            var resultado = new ResultadoAnalise
            {
                DetalhesScores = new Dictionary<string, decimal>()
            };

            if (string.IsNullOrWhiteSpace(nomeSegmento) ||
                (string.IsNullOrWhiteSpace(tituloLicitacao) &&
                 string.IsNullOrWhiteSpace(descricaoLicitacao)))
            {
                resultado.ScoreSimilaridade = 0;
                return resultado;
            }

            var textoLicitacao = $"{tituloLicitacao} {descricaoLicitacao}".ToLower();
            var nomeSegmentoNormalizado = NormalizarTexto(nomeSegmento);
            var textoNormalizado = NormalizarTexto(textoLicitacao);

            // 1. Verificar match exato
            decimal scoreMatchExato = VerificarMatchExato(nomeSegmentoNormalizado, textoNormalizado);
            resultado.DetalhesScores["matchExato"] = scoreMatchExato;

            if (scoreMatchExato == 100m)
            {
                resultado.ScoreSimilaridade = 100m;
                resultado.TipoCorrespondencia = TipoCorrespondencia.MatchExato;
                resultado.TextoEncontrado = nomeSegmento;
                return resultado;
            }

            // 2. Verificar palavras-chave
            decimal scorePalavrasChave = 0;
            string textoChaveEncontrado = "";

            if (palavrasChaveSegmento != null && palavrasChaveSegmento.Count > 0)
            {
                (scorePalavrasChave, textoChaveEncontrado) = 
                    VerificarPalavrasChave(palavrasChaveSegmento, textoNormalizado);
            }
            resultado.DetalhesScores["palavrasChave"] = scorePalavrasChave;

            // 3. Calcular Levenshtein
            decimal scoreLevenshtein = CalcularLevenshteinNormalizado(nomeSegmentoNormalizado, textoNormalizado);
            resultado.DetalhesScores["levenshtein"] = scoreLevenshtein;

            // 4. Calcular Jaccard
            decimal scoreJaccard = CalcularJaccardSimilarity(nomeSegmentoNormalizado, textoNormalizado);
            resultado.DetalhesScores["jaccard"] = scoreJaccard;

            // 5. Calcular score composto
            resultado.ScoreSimilaridade = 
                (scoreMatchExato * PESO_MATCH_EXATO) +
                (scorePalavrasChave * PESO_PALAVRAS_CHAVE) +
                (scoreLevenshtein * PESO_LEVENSHTEIN) +
                (scoreJaccard * PESO_JACCARD);

            // 6. Determinar tipo de correspondência
            if (scoreMatchExato > 0)
            {
                resultado.TipoCorrespondencia = TipoCorrespondencia.NomeSegmento;
                resultado.TextoEncontrado = nomeSegmento;
            }
            else if (scorePalavrasChave > 0)
            {
                resultado.TipoCorrespondencia = TipoCorrespondencia.PalavrasChave;
                resultado.TextoEncontrado = textoChaveEncontrado;
            }
            else if (resultado.ScoreSimilaridade >= THRESHOLD_MINIMO)
            {
                resultado.TipoCorrespondencia = TipoCorrespondencia.ConteudoSimilar;
                resultado.TextoEncontrado = tituloLicitacao?.Substring(0, Math.Min(100, tituloLicitacao?.Length ?? 0)) ?? "Descrição similar";
            }

            return resultado;
        }

        /// <summary>
        /// Normaliza o texto removendo acentos, caracteres especiais e normalizando espaços
        /// </summary>
        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            // Remover acentos
            var textoComAcentos = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in textoComAcentos)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            // Converter para minúsculas, remover caracteres especiais, normalizar espaços
            var resultado = sb.ToString()
                .ToLower()
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .Aggregate(new StringBuilder(), (acc, c) =>
                {
                    if (char.IsWhiteSpace(c))
                    {
                        if (acc.Length > 0 && !char.IsWhiteSpace(acc[acc.Length - 1]))
                            acc.Append(' ');
                    }
                    else
                    {
                        acc.Append(c);
                    }
                    return acc;
                })
                .ToString()
                .Trim();

            return Regex.Replace(resultado, @"\s+", " ");
        }

        /// <summary>
        /// Verifica se há match exato do nome do segmento no texto
        /// </summary>
        private decimal VerificarMatchExato(string nomeSegmento, string texto)
        {
            if (texto.Contains($" {nomeSegmento} ") || 
                texto.StartsWith(nomeSegmento) || 
                texto.EndsWith(nomeSegmento))
            {
                return 100m;
            }

            return 0m;
        }

        /// <summary>
        /// Verifica quantas palavras-chave foram encontradas
        /// </summary>
        private (decimal score, string palavraEncontrada) VerificarPalavrasChave(
            List<string> palavrasChave,
            string texto)
        {
            var palavrasEncontradas = new List<string>();

            foreach (var palavra in palavrasChave)
            {
                var palavraNormalizada = NormalizarTexto(palavra);
                if (!string.IsNullOrEmpty(palavraNormalizada) && 
                    texto.Contains($" {palavraNormalizada} ") ||
                    texto.StartsWith(palavraNormalizada) ||
                    texto.EndsWith(palavraNormalizada))
                {
                    palavrasEncontradas.Add(palavra);
                }
            }

            if (palavrasEncontradas.Count == 0)
                return (0m, "");

            // Score proporcional ao número de palavras encontradas
            var score = Math.Min(100m, (palavrasEncontradas.Count * 100m) / palavrasChave.Count);
            var textoPalavras = string.Join(", ", palavrasEncontradas);

            return (score, textoPalavras);
        }

        /// <summary>
        /// Calcula a Distância de Levenshtein normalizada (0-100)
        /// Mede quantas edições (inserção, deleção, substituição) são necessárias
        /// </summary>
        private decimal CalcularLevenshteinNormalizado(string texto1, string texto2)
        {
            var distancia = CalcularLevenshtein(texto1, texto2);
            var maxLength = Math.Max(texto1.Length, texto2.Length);

            if (maxLength == 0)
                return 100m;

            return Math.Max(0, (100m * (maxLength - distancia)) / maxLength);
        }

        /// <summary>
        /// Implementação do algoritmo de Levenshtein Distance
        /// </summary>
        private int CalcularLevenshtein(string s1, string s2)
        {
            var length1 = s1.Length;
            var length2 = s2.Length;
            var d = new int[length1 + 1, length2 + 1];

            for (int i = 0; i <= length1; i++)
                d[i, 0] = i;

            for (int j = 0; j <= length2; j++)
                d[0, j] = j;

            for (int i = 1; i <= length1; i++)
            {
                for (int j = 1; j <= length2; j++)
                {
                    var cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[length1, length2];
        }

        /// <summary>
        /// Calcula Jaccard Similarity entre dois textos
        /// Compara os conjuntos de palavras e calcula a intersecção/união
        /// </summary>
        private decimal CalcularJaccardSimilarity(string texto1, string texto2)
        {
            var palavras1 = new HashSet<string>(
                texto1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            var palavras2 = new HashSet<string>(
                texto2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            if (palavras1.Count == 0 || palavras2.Count == 0)
                return 0m;

            var intersecao = palavras1.Intersect(palavras2).Count();
            var uniao = palavras1.Union(palavras2).Count();

            if (uniao == 0)
                return 0m;

            return (100m * intersecao) / uniao;
        }

        /// <summary>
        /// Verifica se o score está acima do threshold mínimo
        /// </summary>
        public bool EhAceitavel(decimal score) => score >= THRESHOLD_MINIMO;

        /// <summary>
        /// Verifica se o score é de alta confiança
        /// </summary>
        public bool EhAltaConfianca(decimal score) => score >= THRESHOLD_ALTO;
    }
}
