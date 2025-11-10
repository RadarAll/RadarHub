using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadarHub.Integracoes.Pnc
{
    public class Pncp
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://pncp.gov.br";

        public Pncp()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RadarGovBot/1.0");
        }

        /// <summary>
        /// Método interno para fazer requisições GET, tratando gzip automaticamente.
        /// </summary>
        private async Task<string> GetAsync(string endpoint)
        {
            var resposta = await _httpClient.GetAsync(endpoint);
            resposta.EnsureSuccessStatusCode();

            var encodings = resposta.Content.Headers.ContentEncoding;
            if (encodings.Contains("gzip"))
            {
                using var compressedStream = await resposta.Content.ReadAsStreamAsync();
                using var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                using var reader = new StreamReader(decompressedStream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }

            return await resposta.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Retorna todos os filtros disponíveis da API do PNCP para o tipo de documento "edital".
        /// </summary>
        public async Task<string> GetFiltros()
        {
            try
            {
                return await GetAsync("/api/search/filters?tipos_documento=edital");
            }
            catch (Exception ex)
            {
                return $"Erro ao obter filtros: {ex.Message}";
            }
        }

        /// <summary>
        /// Retorna todas as licitações que estão com status "recebendo_proposta".
        /// Faz a paginação automaticamente até carregar todos os resultados.
        /// </summary>
        public async Task<List<LicitacaoResponse>> GetTodasLicitacoesRecebendoProposta(int tamanhoPagina = 1000)
        {
            var todas = new List<LicitacaoResponse>();
            int pagina = 1;
            int totalPaginas = 1;
            long totalItens = 0;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            do
            {
                string parametros = $"?tipos_documento=edital&ordenacao=-data&pagina={pagina}&tam_pagina={tamanhoPagina}&status=recebendo_proposta";
                string endpoint = $"/api/search/{parametros}";

                string json;
                try
                {
                    json = await GetAsync(endpoint);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[ERRO] Falha ao requisitar página {pagina}: {ex.Message}");
                    break;
                }

                var resposta = JsonSerializer.Deserialize<LicitacaoDto>(json, options);

                if (resposta?.Items != null && resposta.Items.Any())
                    todas.AddRange(resposta.Items);

                if (totalItens == 0)
                {
                    totalItens = resposta?.Total ?? 0;
                    totalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoPagina);
                }

                Console.WriteLine($"Página {pagina}/{totalPaginas} carregada ({todas.Count} de {totalItens} itens)");

                if (pagina >= totalPaginas || resposta?.Items?.Count < tamanhoPagina)
                    break;

                pagina++;

                await Task.Delay(500);
            } while (true);

            return todas;
        }
    }
}
