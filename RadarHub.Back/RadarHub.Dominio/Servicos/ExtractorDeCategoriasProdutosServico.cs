using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RadarHub.Dominio.Servicos.Analise
{
    /// <summary>
    /// Extrai categorias de produtos e serviços reais de licitações
    /// Descarta automaticamente termos administrativos/procedurais puros
    /// </summary>
    public class ExtractorDeCategoriasProdutosServico
    {
        // Palavras que indicam "categorias de produtos/serviços" quando aparecem
        private static readonly HashSet<string> INDICADORES_CATEGORIA = new HashSet<string> 
        { 
            "material", "medicamento", "equipamento", "serviço", "servico", "bem", "bens",
            "alimento", "alimentos", "produto", "produtos", "insumo", "insumos", "peça", "peças",
            "peça", "item", "itens", "fornecimento", "aquisição", "aquisicao"
        };

        // Palavras puramente administrativas/procedurais (não indicam categoria)
        private static readonly HashSet<string> PURAMENTE_ADMINISTRATIVOS = new HashSet<string>
        {
            "edital", "aviso", "comunicado", "licitação", "licitacoes", "contratação", "contratacao",
            "compra", "compras", "processo", "procedimento", "necessidade", "necessidades",
            "demanda", "demandas", "oferta", "oficio", "despacho", "portaria", "termo",
            "referencia", "lei", "decreto", "resolução", "resolucao", "norma", "regulamento",
            "portal", "pncp", "sistema", "plataforma", "acesso", "login", "cadastro",
            "aprovação", "aprovacao", "rejeição", "rejeicao", "cancelamento", "cancelamento",
            "empresa", "empresas", "fornecedor", "fornecedores", "órgão", "orgao",
            "municipio", "municipios", "estado", "estados", "governo", "público", "publico",
            "2025", "2024", "2023", "janeiro", "fevereiro", "março", "marco", "abril",
            "maio", "junho", "julho", "agosto", "setembro", "outubro", "novembro", "dezembro",
            "futura", "eventual", "urgente", "imediata"
        };

        // Categorias comerciais conhecidas (usadas para validação, não exclusão)
        private static readonly HashSet<string> PALAVRAS_CATEGORIAS_COMUNS = new HashSet<string>
        {
            "medicamento", "medicamentos", "farmaco", "farmacos", "vacina", "vacinas",
            "alimento", "alimentos", "comida", "refeição", "refeicao",
            "papel", "papelaria", "caneta", "caderno", "livro", "livros",
            "equipamento", "equipamentos", "máquina", "maquina", "motor",
            "construção", "construcao", "material construção", "cimento", "tijolo",
            "transporte", "veiculo", "carro", "caminhão", "caminhao",
            "eletricidade", "energia", "agua", "agua", "gás", "gas",
            "tecnologia", "informatica", "computador", "software", "hardware",
            "consultoria", "assessoria", "auditoria", "analise", "analise",
            "limpeza", "higiene", "desinfetante", "sabonete", "sabão", "sabao",
            "combustível", "combustivel", "gasolina", "diesel"
        };

    /// <summary>
    /// Extrai termos que provavelmente representam categorias de produtos/serviços
    /// Retorna lista ordenada por "probabilidade de ser categoria real"
    /// ESTRATÉGIA: Privilegia bigramas e palavras substantivadas, penaliza severamente preposições/conjunções
    /// </summary>
    public List<string> ExtrairCategoriasProvaveis(
        IEnumerable<string> titulos, 
        IEnumerable<string> descricoes,
        int topResultados = 20)
    {
        var normalizador = new NormalizadorTextoServico();
        var scorePorTermo = new Dictionary<string, double>();

        var todoTexto = titulos.Concat(descricoes ?? new List<string>()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

        foreach (var texto in todoTexto)
        {
            try
            {
                var normalizado = normalizador.Normalizar(texto);
                var palavras = normalizado.Split(new[] { ' ', '-', '_' }, System.StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                // Bigramas e Trigramas: PRIORIDADE máxima (palavras compostas são melhores categorias)
                for (int i = 0; i < palavras.Count - 1; i++)
                {
                    var p1Norm = normalizador.Normalizar(palavras[i]);
                    var p2Norm = normalizador.Normalizar(palavras[i + 1]);

                    // Ignorar se qualquer parte é puramente administrativo
                    if (PURAMENTE_ADMINISTRATIVOS.Contains(p1Norm) || PURAMENTE_ADMINISTRATIVOS.Contains(p2Norm))
                        continue;

                    // Ignorar bigramas com preposições genéricas (para, com, de, em, etc)
                    var preposicoes = new HashSet<string> { "para", "com", "de", "em", "no", "na", "nos", "nas", "do", "da", "dos", "das", "a", "ao" };
                    if (preposicoes.Contains(p1Norm) || preposicoes.Contains(p2Norm))
                        continue;

                    var bigram = $"{palavras[i]} {palavras[i + 1]}";
                    double score = 10.0; // Bigramas começam com score MUITO alto

                    // MEGA BOOST: Se contém palavra de categoria conhecida
                    if (PALAVRAS_CATEGORIAS_COMUNS.Contains(p1Norm) || PALAVRAS_CATEGORIAS_COMUNS.Contains(p2Norm))
                        score += 20.0;

                    // BOOST: Se contém indicador de categoria
                    if (INDICADORES_CATEGORIA.Contains(p1Norm) || INDICADORES_CATEGORIA.Contains(p2Norm))
                        score += 10.0;

                    if (!scorePorTermo.ContainsKey(bigram))
                        scorePorTermo[bigram] = 0;
                    scorePorTermo[bigram] += score;
                }

                // Unigramas: score muito MAIS BAIXO, apenas se forem muito específicas
                for (int i = 0; i < palavras.Count; i++)
                {
                    var palavra = palavras[i];
                    var palavraNorm = normalizador.Normalizar(palavra);

                    // PENALTY SEVERA: Se é preposição, conjunção, artigo (muito curto genérico)
                    if (palavraNorm.Length <= 3 || PURAMENTE_ADMINISTRATIVOS.Contains(palavraNorm))
                        continue;

                    // Se é palavra de categoria conhecida, aceitar com score moderado
                    if (PALAVRAS_CATEGORIAS_COMUNS.Contains(palavraNorm))
                    {
                        var score = 3.0; // Apenas 3.0 vs 10+ dos bigramas
                        if (!scorePorTermo.ContainsKey(palavra))
                            scorePorTermo[palavra] = 0;
                        scorePorTermo[palavra] += score;
                    }
                    // Se tem indicador de categoria perto, aceitar também
                    else if ((i > 0 && INDICADORES_CATEGORIA.Contains(normalizador.Normalizar(palavras[i - 1]))) ||
                             (i < palavras.Count - 1 && INDICADORES_CATEGORIA.Contains(normalizador.Normalizar(palavras[i + 1]))))
                    {
                        var score = 2.0;
                        if (!scorePorTermo.ContainsKey(palavra))
                            scorePorTermo[palavra] = 0;
                        scorePorTermo[palavra] += score;
                    }
                }
            }
            catch { }
        }

        // Retornar termos ordenados por score (mais prováveis primeiro)
        // FILTRO: Apenas aceitar termos com score mínimo (bigramas naturalmente terão mais)
        return scorePorTermo
            .Where(kv => kv.Value >= 2.5) // Threshold mais alto para excluir unigramas genéricas
            .OrderByDescending(kv => kv.Value)
            .Take(topResultados)
            .Select(kv => kv.Key)
            .ToList();
    }        /// <summary>
        /// Identifica se um termo é provavelmente uma categoria de produto/serviço (não administrativo)
        /// </summary>
        public bool EhProvavelmenteCategoriaReal(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return false;

            var normalizador = new NormalizadorTextoServico();
            var termoNorm = normalizador.Normalizar(termo);

            // Se está na lista de puramente administrativos, não é categoria
            if (PURAMENTE_ADMINISTRATIVOS.Contains(termoNorm))
                return false;

            // Se está na lista de categorias conhecidas, é categoria
            if (PALAVRAS_CATEGORIAS_COMUNS.Contains(termoNorm))
                return true;

            // Se tem indicador de categoria, é provável categoria
            var palavras = termo.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (palavras.Any(p => INDICADORES_CATEGORIA.Contains(normalizador.Normalizar(p))))
                return true;

            // Heurística: Se tem 2+ palavras e nenhuma é administrativo, é mais provável ser categoria
            if (palavras.Length >= 2 && !palavras.Any(p => PURAMENTE_ADMINISTRATIVOS.Contains(normalizador.Normalizar(p))))
                return true;

            return false;
        }
    }
}
