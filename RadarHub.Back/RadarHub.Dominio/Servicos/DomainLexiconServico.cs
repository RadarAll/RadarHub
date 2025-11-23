using System.Collections.Generic;
using System.Linq;

namespace RadarHub.Dominio.Servicos
{
    /// <summary>
    /// Serviço simples de léxico mapeando palavras-chave para segmentos de mercado
    /// Usado para transformar termos técnicos em segmentos comerciais mais úteis
    /// </summary>
    public class DomainLexiconServico
    {
        private readonly Dictionary<string, List<string>> _lexicon;

        public DomainLexiconServico()
        {
            _lexicon = new Dictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase)
            {
                // Papelaria e Material de Escritório
                { "Papelaria", new List<string>{ 
                    "papel", "papelaria", "caneta", "lapis", "lapi s", "caderno", "borracha", "estojo", 
                    "impressao", "impressora", "toner", "cartucho", "folha", "bloco", "livro registro",
                    "pasta", "arquivo", "grampo", "clipe", "fita adesiva", "envelope"
                } },

                // Medicamentos e Insumos Farmacêuticos
                { "Medicamentos", new List<string>{ 
                    "medicamento", "remedio", "remedios", "vacina", "farmaceutico", "farmacia", 
                    "antibiotico", "analgesico", "medicacao", "farmacos", "droga", "insumo farmaceutico"
                } },

                // Material Escolar
                { "Material Escolar", new List<string>{ 
                    "caderno", "mochila", "estojo", "lapis", "lapiseira", "caneta", "colar",
                    "material escolar", "escolar", "uniforme", "acessorio escolar"
                } },

                // Alimentação e Merenda Escolar
                { "Alimentação Escolar", new List<string>{ 
                    "alimentacao", "merenda", "comida", "alimento", "refeicao", "restaurante", 
                    "fornecimento de alimentos", "merenda escolar", "escolar", "comestivel",
                    "alimentar", "alimentos", "refeicoes"
                } },

                // Higiene e Limpeza
                { "Higiene e Limpeza", new List<string>{ 
                    "sabonete", "desinfetante", "detergente", "papel higienico", "alcool", "higiene", 
                    "limpeza", "sanitizante", "higienico", "toalha papel", "sabao", "desinfeccao"
                } },

                // Equipamentos de Informática
                { "Equipamentos de Informática", new List<string>{ 
                    "computador", "notebook", "servidor", "monitor", "teclado", "mouse", "impressora", 
                    "hardware", "ti", "software", "pc", "desktop", "equipamento informatica",
                    "dispositivo", "periferico"
                } },

                // Transporte e Logística
                { "Transporte e Logística", new List<string>{ 
                    "transporte", "logistica", "frete", "caminhao", "veiculo", "entrega",
                    "veiculo automotor", "automovel", "motorista", "carregamento"
                } },

                // Construção e Obras
                { "Construção e Obras", new List<string>{ 
                    "construcao", "obra", "reforma", "materiais de construcao", "cimento", "tijolo", 
                    "serralheria", "obra civil", "construir", "edificacao"
                } },

                // Serviços de Consultoria
                { "Consultoria e Assessoria", new List<string>{ 
                    "consultoria", "assessoria", "auditoria", "analise tecnica", "consultor",
                    "consultivo", "assessor", "consulente"
                } },

                // Saúde
                { "Saúde", new List<string>{ 
                    "hospital", "clinica", "saude", "equipamento medico", "medico", "enfermagem",
                    "hospitalar", "assistencia saude", "servico saude", "cuidado saude"
                } }
            };

            // Normalize lexicon keywords (remove acentos) - keep both accented and unaccented forms
            foreach (var key in _lexicon.Keys.ToList())
            {
                var list = _lexicon[key];
                var normalized = list.Select(x => Normalizar(x)).Distinct().ToList();
                _lexicon[key] = normalized;
            }
        }

        private string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            var nfd = texto.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in nfd)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Tenta mapear um termo (palavra ou n-gram) para um segmento do léxico
        /// Retorna null se não houver correspondência
        /// Usa estratégia de matching: exato primeiro, depois contains, depois palavras individuais
        /// </summary>
        public string MapearParaSegmento(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo)) return null;
            var t = Normalizar(termo);

            // Estratégia 1: Procurar correspondência exata de palavras-chave
            foreach (var kv in _lexicon)
            {
                foreach (var keyword in kv.Value)
                {
                    // Comparação mais rigorosa: a palavra-chave deve estar como palavra inteira no termo
                    // Não apenas como substring para evitar "festividades" → "festival"
                    var palavrasDoTermo = t.Split(new[] { ' ', '-', '_' }, System.StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var palavra in palavrasDoTermo)
                    {
                        // Correspondência exata
                        if (palavra == keyword)
                            return kv.Key;
                        
                        // Correspondência parcial apenas se forem palavras-chave menos comuns (> 4 caracteres)
                        if (keyword.Length > 4 && palavra.Contains(keyword))
                            return kv.Key;
                    }
                }
            }

            return null;
        }
    }
}
