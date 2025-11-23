using Microsoft.Extensions.Options;
using RadarHub.Integracoes.Gemini.Analise;
using RadarHub.Integracoes.Gemini.Configuracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RadarHub.Integracoes.Gemini
{
    // ... (Definição do DTO ResultadoAnaliseSemantica no namespace Analise)

    public class GeminiAnalisadorServico : IAnalisadorSemanticoExterno
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string GEMINI_MODEL = "gemini-2.5-flash";

        // Limite de itens por requisição de lote para evitar timeout/token de saída
        private const int CHUNK_SIZE = 50;

        // As propriedades de DTO permanecem
        private const string NOME_PROPRIEDADE_SEGMENTO = "NomeSegmentoPrincipal";
        private const string NOME_PROPRIEDADE_CONFIANCA = "Confianca";
        private const string NOME_PROPRIEDADE_JUSTIFICATIVA = "Justificativa";

        public GeminiAnalisadorServico(
            HttpClient httpClient,
            IOptions<GeminiSettings> geminiOptions)
        {
            _httpClient = httpClient;

            if (string.IsNullOrEmpty(geminiOptions.Value.ApiKey))
            {
                throw new InvalidOperationException("A Gemini API Key (Gemini:ApiKey) não está configurada.");
            }
            _apiKey = geminiOptions.Value.ApiKey;

            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ======================================================================
        // 🎯 MÉTODO PRINCIPAL (COM CHUNKING)
        // ======================================================================

        /// <summary>
        /// Analisa múltiplas licitações em lotes menores para garantir robustez e evitar limites de token.
        /// </summary>
        /// <param name="licitacoes">Lista de tuplas (Id, Descrição) a serem analisadas.</param>
        public async Task<List<ResultadoAnaliseSemantica>> AnalisarDescricoesEmLoteAsync(
            List<(long LicitacaoId, string Descricao)> licitacoes)
        {
            if (licitacoes == null || !licitacoes.Any())
            {
                return new List<ResultadoAnaliseSemantica>();
            }

            var resultadosAcumulados = new List<ResultadoAnaliseSemantica>();

            // 1. Quebra a lista em chunks menores (50 itens por requisição)
            var chunks = licitacoes
                .Select((l, i) => new { Item = l, Index = i })
                .GroupBy(x => x.Index / CHUNK_SIZE)
                .Select(g => g.Select(x => x.Item).ToList())
                .ToList();

            // 2. Processa cada chunk sequencialmente
            foreach (var chunk in chunks)
            {
                // Chama o método interno que lida com a requisição real
                var resultadosChunk = await ProcessarChunkInternoAsync(chunk);
                resultadosAcumulados.AddRange(resultadosChunk);
            }

            // O mapeamento de IDs de licitação é feito dentro do ProcessarChunkInternoAsync.
            return resultadosAcumulados;
        }

        // ======================================================================
        // ⚙️ MÉTODO INTERNO (Requisição real)
        // ======================================================================

        /// <summary>
        /// Executa a requisição única para um chunk específico de licitações.
        /// </summary>
        private async Task<List<ResultadoAnaliseSemantica>> ProcessarChunkInternoAsync(
            List<(long LicitacaoId, string Descricao)> licitacoes)
        {
            // 1. Criar a lista de conteúdos (prompts)
            var contents = licitacoes.Select(l =>
                new { parts = new[] { new { text = CriarPromptClassificacao(l.Descricao) } } }
            ).ToArray();

            // O esquema de resposta deve englobar um ARRAY JSON
            var responseSchema = new
            {
                type = "ARRAY",
                items = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        NomeSegmentoPrincipal = new { type = "STRING", description = "Nome do segmento limpo, sem palavras processuais." },
                        Confianca = new { type = "NUMBER", description = "Confiança percentual (0-100) na classificação." },
                        Justificativa = new { type = "STRING", description = "Breve explicação da classificação." }
                    }
                }
            };

            // 2. Montar o payload
            var requestBody = new
            {
                contents = contents,
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    responseSchema = responseSchema,
                    temperature = 0.1
                }
            };

            var url = $"v1beta/models/{GEMINI_MODEL}:generateContent?key={_apiKey}";

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            List<ResultadoAnaliseSemantica> resultadosLLM = new List<ResultadoAnaliseSemantica>();
            string responseJson = null;

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                responseJson = await response.Content.ReadAsStringAsync();

                // 3. Extrai o bloco JSON (ARRAY de resultados)
                var jsonDocument = JsonDocument.Parse(responseJson);

                var textPart = jsonDocument.RootElement.GetProperty("candidates")[0]
                                                     .GetProperty("content").GetProperty("parts")[0]
                                                     .GetProperty("text").GetString();

                // 4. Deserializa o array de resultados (robustez contra arrays nulos)
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                resultadosLLM = JsonSerializer.Deserialize<List<ResultadoAnaliseSemantica>>(textPart, options) ?? new List<ResultadoAnaliseSemantica>();

                // 5. Mapeia de volta o ID da licitação, respeitando a ordem
                for (int i = 0; i < licitacoes.Count; i++)
                {
                    if (i < resultadosLLM.Count)
                    {
                        // Usa o Id da licitação original para identificar o resultado
                        resultadosLLM[i].LicitacaoId = licitacoes[i].LicitacaoId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de erro: Se o chunk falhar (parsing ou HTTP), retorna lista vazia
                // e o processo principal continua com o próximo chunk.
                Console.WriteLine($"ERRO CRÍTICO no processamento de lote do Gemini: {ex.Message}. Response JSON: {responseJson}");
                return new List<ResultadoAnaliseSemantica>();
            }

            return resultadosLLM;
        }


        // ======================================================================
        // CÓDIGO AUXILIAR (Mantido)
        // ======================================================================

        private string CriarPromptClassificacao(string descricao)
        {
            return $@"Você é um especialista em classificação de licitações públicas.
Sua única tarefa é analisar a Descrição de uma licitação e retornar o NOME DO SEGMENTO (o OBJETO) mais específico e comercialmente relevante.

REGRAS:
1. O nome do segmento NÃO DEVE ser genérico ou processual (Ex: ""Contratação"", ""Aquisição"", ""Direta"", ""Aviso"").
2. O nome deve ser um substantivo descritivo. **Prefira termos que agrupem subitens, como a categoria principal do item (Ex: ""Material de Escritório"", ""Componentes Elétricos"").**
3. O nome deve ser capitalizado.
4. Retorne a Confiança percentual (de 0 a 100).
5. Responda APENAS com um objeto JSON válido (ou um ARRAY de objetos JSON se for em lote).

Descrição da Licitação: {descricao}";
        }
    }
}