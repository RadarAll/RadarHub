using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// Serviço para normalização de texto, remoção de stopwords e extração de termos significativos.
/// </summary>
public class NormalizadorTextoServico
{
    private static readonly HashSet<string> StopwordsPT = new(StringComparer.OrdinalIgnoreCase)
    {
        "o","a","os","as","um","uma","de","do","da","dos","das","em","no","na","nos","nas",
        "e","ou","mas","por","para","com","sem","como","quando","onde","porém","pois","logo",
        "edital","editais","licitacao","licitacoes","contratacao","contratacoes","aquisicao","aquisicoes",
        "registro", "registros", "aquisição", "contratação",
        "servico","servicos","produto","produtos","empresa","empresas","aviso","portal","municipio","evento",
        "equipamento", "equipamentos",
        "material", "materiais",
        "obra", "obras",
        "insumo", "insumos",
        "bens",
        "geral",
        "prestacao"
    };

    /// <summary>
    /// Remove stopwords e palavras curtas, normalizando para comparação.
    /// </summary>
    public List<string> RemoverStopwords(List<string> termos, int minTamanhoPalavra = 3)
    {
        return termos
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => t.Length >= minTamanhoPalavra && !StopwordsPT.Contains(t))
            .ToList();
    }

    /// <summary>
    /// Tokeniza um texto, remove pontuações, normaliza e aplica stop-words.
    /// </summary>
    public List<string> TokenizarTextoSimples(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return new List<string>();

        var textoNormalizado = RemoverPontuacao(RemoverAcentos(texto));
        var termosBrutos = textoNormalizado.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        return RemoverStopwords(termosBrutos);
    }

    #region Métodos de Limpeza Auxiliares

    /// <summary>
    /// Normaliza o texto removendo acentos, pontuação e convertendo para minúsculas.
    /// </summary>
    public string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

        // Sequência: Remover Acentos -> Remover Pontuação -> ToLowerInvariant
        return RemoverPontuacao(RemoverAcentos(texto))
            .ToLowerInvariant().Trim();
    }

    /// <summary>
    /// Remove acentos de uma string.
    /// </summary>
    public string RemoverAcentos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;

        var normalizedString = texto.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Remove pontuação, deixando apenas letras, números e espaços.
    /// </summary>
    public string RemoverPontuacao(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        // Substitui caracteres que não são letras, números ou espaços por um espaço
        return Regex.Replace(texto, @"[^\w\s]", " ");
    }

    /// <summary>
    /// Remove termos genéricos de licitações (já incluídos no StopwordsPT, mas útil para limpeza extra).
    /// </summary>
    public string RemoverTermosComuns(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        var termosComunsRegex = new Regex(string.Join("|", StopwordsPT), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return termosComunsRegex.Replace(texto, " ");
    }

    /// <summary>
    /// Agrupa termos similares para contagem.
    /// </summary>
    public List<string> RemoverSimilares(List<string> termos, int distanciaMaxima = 1)
    {
        // ... (Implementação de Levenshtein - Mantida como está)
        var grupos = new List<string>();
        var processados = new HashSet<string>();

        foreach (var termo in termos.OrderByDescending(t => t.Length))
        {
            if (processados.Contains(termo))
                continue;

            grupos.Add(termo);
            processados.Add(termo);

            foreach (var outro in termos)
            {
                if (processados.Contains(outro))
                    continue;

                if (DistanciaLevenshtein(termo, outro) <= distanciaMaxima)
                    processados.Add(outro);
            }
        }

        return grupos;
    }

    /// <summary>
    /// Cálculo de distância de Levenshtein.
    /// </summary>
    private int DistanciaLevenshtein(string s1, string s2)
    {
        s1 ??= string.Empty;
        s2 ??= string.Empty;

        int n = s1.Length;
        int m = s2.Length;

        if (n == 0) return m;
        if (m == 0) return n;

        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
    #endregion
}