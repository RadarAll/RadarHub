using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos.Analise;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace RadarHub.Dominio.Servicos
{
    /// <summary>
    /// Serviço responsável por sugerir novos segmentos baseado em análise de licitações.
    /// Utiliza TF-IDF, Classificação Direta (LLM), Clustering e PLN Avançado (Gemini) para maior relevância.
    /// </summary>
    public class SugestorSegmentosServico
    {
        private readonly IRepositorioBaseAssincrono<SugestaoSegmento> _repositorioSugestoes;
        private readonly IRepositorioBaseAssincrono<Licitacao> _repositorioLicitacao;
        private readonly IRepositorioBaseAssincrono<Segmento> _repositorioSegmentos;
        private readonly IRepositorioBaseAssincrono<LicitacaoSegmento> _repositorioLicitacaoSegmento;

        private readonly AnalisadorSimilaridadeServico _analisador;
        private readonly IServicoMensagem _mensagens;
        private readonly NormalizadorTextoServico _normalizador;
        private readonly AnalisadorTFIDFServico _tfidfServico;
        private readonly ClassificadorSegmentosServico _classificador;
        private readonly ExtractorDeCategoriasProdutosServico _extractorCategorias;
        private readonly List<string> _mensagensDebug = new List<string>();

        private const decimal CONFIANCA_MINIMA_PADRAO = 60m;
        private const int FREQUENCIA_MINIMA_DOCS = 3;
        private const int TAMANHO_MINIMO_GRUPO = 3;
        private const int TOP_SUGESTOES_TFIDF = 5;
        private const decimal CONFIANCA_MINIMA_LLM_DIRETO = 80m;

        public SugestorSegmentosServico(
            IRepositorioBaseAssincrono<SugestaoSegmento> repositorioSugestoes,
            IRepositorioBaseAssincrono<Licitacao> repositorioLicitacao,
            IRepositorioBaseAssincrono<Segmento> repositorioSegmentos,
            IRepositorioBaseAssincrono<LicitacaoSegmento> repositorioLicitacaoSegmento,
            IServicoMensagem mensagens,
            AnalisadorSimilaridadeServico analisador,
            NormalizadorTextoServico normalizador,
            AnalisadorTFIDFServico tfidfServico,
            ClassificadorSegmentosServico classificador,
            ExtractorDeCategoriasProdutosServico extractorCategorias)
        {
            _repositorioSugestoes = repositorioSugestoes;
            _repositorioLicitacao = repositorioLicitacao;
            _repositorioSegmentos = repositorioSegmentos;
            _repositorioLicitacaoSegmento = repositorioLicitacaoSegmento;

            // Atribuições injetadas
            _mensagens = mensagens;
            _analisador = analisador;
            _normalizador = normalizador;
            _tfidfServico = tfidfServico;
            _classificador = classificador;
            _extractorCategorias = extractorCategorias;
        }

        #region Helpers de logging
        private void AdicionarMensagemDebug(string mensagem)
        {
            _mensagens?.Adicionar(mensagem);
            _mensagensDebug.Add(mensagem);
        }

        private void AdicionarErroDebug(string mensagem)
        {
            _mensagens?.AdicionarErro(mensagem);
            _mensagensDebug.Add($"❌ {mensagem}");
        }
        #endregion

        /// <summary>
        /// Ponto de entrada principal: gera e persiste sugestões novas
        /// </summary>
        public async Task<List<SugestaoSegmento>> SugerirSegmentosAsync(
            List<Licitacao> licitacoes = null,
            decimal confiancaMinimaPercentual = CONFIANCA_MINIMA_PADRAO,
            int? topSugestoes = 20)
        {
            try
            {
                _mensagensDebug.Clear();

                if (licitacoes == null)
                {
                    var licitacoesDb = await _repositorioLicitacao.ObterTodosAssincrono();
                    licitacoes = licitacoesDb.ToList();
                }

                AdicionarMensagemDebug($"DEBUG: Total de licitações obtidas: {licitacoes.Count}");

                if (!licitacoes.Any())
                {
                    AdicionarErroDebug("Nenhuma licitação encontrada para análise");
                    return new List<SugestaoSegmento>();
                }

                licitacoes = licitacoes.OrderByDescending(x => x.CriadoEm).Take(500).ToList();
                AdicionarMensagemDebug($"DEBUG: Licitações após filtro (últimas 500): {licitacoes.Count}");

                var segmentosExistentes = (await _repositorioSegmentos.ObterTodosAssincrono()).ToList();
                var nomesSegmentosExistentes = new HashSet<string>(segmentosExistentes
                    .Select(s => _normalizador.Normalizar(s.Nome ?? string.Empty))
                    .Where(n => !string.IsNullOrWhiteSpace(n)));

                var sugestoesExistentes = (await _repositorioSugestoes.ObterTodosAssincrono()).ToList();
                var nomesSugestoesExistentes = new HashSet<string>(sugestoesExistentes
                    .Select(s => _normalizador.Normalizar(s.NomeSugerido ?? string.Empty))
                    .Where(n => !string.IsNullOrWhiteSpace(n)));

                AdicionarMensagemDebug($"DEBUG: Segmentos existentes: {nomesSegmentosExistentes.Count}, Sugestões existentes: {nomesSugestoesExistentes.Count}");

                var sugestoes = new List<SugestaoSegmento>();
                var licitacoesJaProcessadas = new HashSet<long>();

                var sugestoesTFIDF = SugerirPorTFIDF(licitacoes);
                AdicionarMensagemDebug($"DEBUG: Sugestões por TF-IDF (brutas): {sugestoesTFIDF.Count}");
                sugestoes.AddRange(sugestoesTFIDF);

                var sugestoesDiretas = await SugerirPorLLMDiretoAsync(licitacoes);
                AdicionarMensagemDebug($"DEBUG: Sugestões por LLM Direto (brutas): {sugestoesDiretas.Count}");
                sugestoes.AddRange(sugestoesDiretas.Where(s => s.ConfiancaPercentual >= CONFIANCA_MINIMA_LLM_DIRETO));

                
                foreach (var sugestao in sugestoesDiretas.Where(s => s.ConfiancaPercentual >= CONFIANCA_MINIMA_LLM_DIRETO))
                {
                    try
                    {
                        var ids = JsonSerializer.Deserialize<List<long>>(sugestao.LicitacaoIds);
                        if (ids != null) licitacoesJaProcessadas.UnionWith(ids);
                    }
                    catch
                    {
                        AdicionarErroDebug($"Falha ao deserializar LicitacaoIds para rastreio: {sugestao.LicitacaoIds}");
                    }
                }

                AdicionarMensagemDebug($"DEBUG: Licitações classificadas diretamente (Gemini): {licitacoesJaProcessadas.Count}");

                var licitacoesParaClustering = licitacoes
                    .Where(l => !licitacoesJaProcessadas.Contains(l.Id))
                    .ToList();

                AdicionarMensagemDebug($"DEBUG: Licitações restantes para Clustering: {licitacoesParaClustering.Count}");

                var (sugestoesClustering, licitacoesClusteredIds) = await SugerirPorClusteringAsync(licitacoesParaClustering);
                AdicionarMensagemDebug($"DEBUG: Sugestões por clustering (brutas): {sugestoesClustering.Count}. Licitações clustered: {licitacoesClusteredIds.Count}");
                sugestoes.AddRange(sugestoesClustering);
                licitacoesJaProcessadas.UnionWith(licitacoesClusteredIds);


                var filtradasInternas = RemoverSugestoesDuplicadas(sugestoes);
                AdicionarMensagemDebug($"DEBUG: Sugestões após deduplicação interna: {filtradasInternas.Count}");

                var filtradasExistentes = FiltrarSugestoesJaExistentes(filtradasInternas, nomesSegmentosExistentes, nomesSugestoesExistentes);
                AdicionarMensagemDebug($"DEBUG: Sugestões após filtrar existentes no BD: {filtradasExistentes.Count}");

                var finais = filtradasExistentes
                    .Where(x => x.ConfiancaPercentual >= confiancaMinimaPercentual)
                    .OrderByDescending(x => x.ConfiancaPercentual)
                    .ToList();

                AdicionarMensagemDebug($"DEBUG: Após filtro de confiança ({confiancaMinimaPercentual}%): {finais.Count}");

                if (topSugestoes.HasValue)
                    finais = finais.Take(topSugestoes.Value).ToList();

                AdicionarMensagemDebug($"DEBUG: Sugestões finais a persistir: {finais.Count}");

                int processadasCount = 0;
                foreach (var sugestao in finais)
                {
                    var nomeNorm = _normalizador.Normalizar(sugestao.NomeSugerido ?? string.Empty);
                    if (nomesSegmentosExistentes.Contains(nomeNorm) || nomesSugestoesExistentes.Contains(nomeNorm))
                    {
                        AdicionarMensagemDebug($"DEBUG: Pulando persistência - já existe: {sugestao.NomeSugerido}");
                        continue;
                    }

                    sugestao.Status = StatusAprovacaoSugestao.Pendente;
                    sugestao.CriadoEm = DateTime.UtcNow;

                    await _repositorioSugestoes.AdicionarAssincrono(sugestao);
                    nomesSugestoesExistentes.Add(nomeNorm);
                    processadasCount++;
                }

                await _repositorioSugestoes.SalvarAlteracoesAssincrono();

                AdicionarMensagemDebug($"Análise concluída! {finais.Count} sugestões avaliadas, {processadasCount} processadas.");

                return finais;
            }
            catch (Exception ex)
            {
                AdicionarErroDebug($"Erro ao sugerir segmentos: {ex.Message}");
                return new List<SugestaoSegmento>();
            }
        }

        #region Estratégias Otimizadas

        private List<SugestaoSegmento> SugerirPorTFIDF(List<Licitacao> licitacoes)
        {
            var sugestoes = new List<SugestaoSegmento>();
            var documentos = licitacoes.ToDictionary(l => l.Id, l => l.Descricao ?? string.Empty);

            var documentosLicitacoes = documentos.Values.ToList();
            if (!documentosLicitacoes.Any()) return sugestoes;

            var resultadosTFIDF = _tfidfServico.CalcularTFIDFParaCorpus(documentosLicitacoes);

            var termosRelevantes = resultadosTFIDF
                .SelectMany(d => d)
                .GroupBy(x => x.Key)
                .Select(g => new
                {
                    Termo = g.Key,
                    ScoreTotal = (decimal)g.Sum(x => x.Value),
                    FrequenciaDocs = g.Count()
                })
                .Where(x => x.FrequenciaDocs >= FREQUENCIA_MINIMA_DOCS)
                .OrderByDescending(x => x.ScoreTotal)
                .Take(TOP_SUGESTOES_TFIDF)
                .ToList();

            foreach (var item in termosRelevantes)
            {
                var licitacaoIds = documentos
                    .Where(doc => _normalizador.Normalizar(doc.Value).Contains(_normalizador.Normalizar(item.Termo)))
                    .Select(doc => doc.Key)
                    .ToList();

                var nome = CapitalizarPrimeira(item.Termo);
                var confianca = CalcularConfiancaTFIDF(item.ScoreTotal, item.FrequenciaDocs, licitacoes.Count);

                sugestoes.Add(new SugestaoSegmento(
                    nomeSugerido: nome,
                    descricaoSugerida: $"Termo de alta relevância (TF-IDF: {item.ScoreTotal:N2}) encontrado em {item.FrequenciaDocs} licitações.",
                    palavrasChaveSugeridas: item.Termo,
                    quantidadeLicitacoes: licitacaoIds.Count,
                    confiancaPercentual: confianca,
                    origem: TipoOrigemSugestao.PalavrasChave)
                {
                    LicitacaoIds = JsonSerializer.Serialize(licitacaoIds)
                });
            }

            return sugestoes;
        }

        private async Task<(List<SugestaoSegmento>, HashSet<long>)> SugerirPorClusteringAsync(List<Licitacao> licitacoes)
        {
            var sugestoes = new List<SugestaoSegmento>();
            var licitacoesProcessadasIds = new HashSet<long>();
            const int LIMIAR_SIMILARIDADE = 50;

            if (licitacoes.Count < TAMANHO_MINIMO_GRUPO)
            {
                AdicionarMensagemDebug($"DEBUG: Não há licitações suficientes ({licitacoes.Count}) para iniciar o Clustering.");
                return (sugestoes, licitacoesProcessadasIds);
            }

            for (int i = 0; i < licitacoes.Count; i++)
            {
                if (licitacoesProcessadasIds.Contains(licitacoes[i].Id)) continue;

                var grupo = new List<Licitacao> { licitacoes[i] };
                var idsDoGrupo = new List<long> { licitacoes[i].Id };

                for (int j = i + 1; j < licitacoes.Count; j++)
                {
                    if (licitacoesProcessadasIds.Contains(licitacoes[j].Id)) continue;

                    // similaridade com base na Descrição
                    var res = _analisador.AnalisarSimilaridade(
                        nomeSegmento: licitacoes[i].Descricao,
                        tituloLicitacao: string.Empty,
                        descricaoLicitacao: _normalizador.Normalizar(licitacoes[j].Descricao),
                        palavrasChaveSegmento: null);

                    if (res.ScoreSimilaridade >= LIMIAR_SIMILARIDADE)
                    {
                        grupo.Add(licitacoes[j]);
                        idsDoGrupo.Add(licitacoes[j].Id);
                    }
                }

                if (grupo.Count >= TAMANHO_MINIMO_GRUPO)
                {
                    foreach (var lic in grupo)
                    {
                        licitacoesProcessadasIds.Add(lic.Id);
                    }

                    var descricoes = grupo.Select(g => g.Descricao ?? string.Empty).ToList();

                    var titulos = Enumerable.Repeat(string.Empty, descricoes.Count).ToList();

                    AdicionarMensagemDebug($"DEBUG: Tentativa de Classificação por Clustering para grupo de {grupo.Count} licitações (apenas Descrição).");

                    var (nomeSugestao, confiancaGemini, termosRelevantes) = await _classificador.ExtrairNomeDoClusterAsync(titulos, descricoes);

                    var confiancaEstatistica = CalcularConfiancaGrupo(grupo, licitacoes.Count);

                    var confiancaFinal = Math.Max(confiancaGemini, confiancaEstatistica);

                    var palavrasChave = termosRelevantes?.Keys.Take(TOP_SUGESTOES_TFIDF).ToList() ?? new List<string>();

                    AdicionarMensagemDebug($"✓ Cluster classificado pelo Gemini: '{nomeSugestao}' (Confiança LLM: {confiancaGemini:N2}%)");

                    sugestoes.Add(new SugestaoSegmento(
                        nomeSugerido: nomeSugestao,
                        descricaoSugerida: $"Segmento gerado por análise automática de {grupo.Count} licitações similares (LLM/Clustering)",
                        palavrasChaveSugeridas: string.Join(", ", palavrasChave),
                        quantidadeLicitacoes: grupo.Count,
                        confiancaPercentual: confiancaFinal,
                        origem: TipoOrigemSugestao.Clustering)
                    {
                        LicitacaoIds = JsonSerializer.Serialize(idsDoGrupo)
                    });
                }
            }

            return (sugestoes, licitacoesProcessadasIds);
        }

        /// <summary>
        /// Sugere segmentos classificando individualmente as licitações usando o Gemini, focado em alta confiança.
        /// ÚNICA chamada em lote (Batch) à API do LLM
        /// </summary>
        private async Task<List<SugestaoSegmento>> SugerirPorLLMDiretoAsync(List<Licitacao> licitacoes)
        {
            var sugestoes = new List<SugestaoSegmento>();

            // Mantém o limite de 200 itens para o processamento direto
            var licitacoesParaProcessar = licitacoes.Take(200).ToList();

            AdicionarMensagemDebug($"DEBUG: Iniciando Classificação Direta em lote em {licitacoesParaProcessar.Count} licitações.");

            var dadosParaLote = licitacoesParaProcessar
                .Where(l => !string.IsNullOrWhiteSpace(l.Descricao))
                .Select(l => (l.Id, l.Descricao!))
                .ToList();

            if (!dadosParaLote.Any())
            {
                AdicionarMensagemDebug("DEBUG: Nenhuma licitação elegível para classificação direta em lote.");
                return sugestoes;
            }

            List<ResultadoAnaliseSemanticaLote> resultadosLote;
            try
            {
                resultadosLote = await _classificador.ClassificarDiretamenteEmLoteAsync(dadosParaLote);

                AdicionarMensagemDebug($"DEBUG: Classificação Direta em lote concluída. {resultadosLote.Count} resultados recebidos.");
            }
            catch (Exception ex)
            {
                AdicionarErroDebug($"Falha na Classificação Direta em lote (Gemini): {ex.Message}");
                return sugestoes;
            }

            foreach (var resultado in resultadosLote)
            {
                var nomeSugestao = (string)resultado.NomeSegmentoPrincipal;
                var confiancaGemini = (decimal)resultado.Confianca;
                var licitacaoId = (long)resultado.LicitacaoId;
                var termosRelevantes = (Dictionary<string, double>)resultado.TermosRelevantes;

                if (confiancaGemini >= CONFIANCA_MINIMA_LLM_DIRETO && !string.IsNullOrWhiteSpace(nomeSugestao))
                {
                    var palavrasChave = termosRelevantes?.Keys.Take(TOP_SUGESTOES_TFIDF).ToList() ?? new List<string>();

                    AdicionarMensagemDebug($"✓ Classificação Direta SUCESSO: '{nomeSugestao}' (Confiança LLM: {confiancaGemini:N2}%) para Licitacao ID {licitacaoId} (via Batch).");

                    sugestoes.Add(new SugestaoSegmento(
                        nomeSugerido: nomeSugestao,
                        descricaoSugerida: $"Segmento gerado por análise direta e individual do LLM (Gemini) com alta confiança (BATCH).",
                        palavrasChaveSugeridas: string.Join(", ", palavrasChave),
                        quantidadeLicitacoes: 1,
                        confiancaPercentual: confiancaGemini,
                        origem: TipoOrigemSugestao.ClassificacaoDiretaLLM)
                    {
                        LicitacaoIds = JsonSerializer.Serialize(new List<long> { licitacaoId })
                    });
                }
                else
                {
                    AdicionarMensagemDebug($"DEBUG: Classificação Direta Falhou/Baixa Confiança ({confiancaGemini:N2}%) para Licitacao ID {licitacaoId}.");
                }
            }

            return sugestoes;
        }

        #endregion

        #region Filtragem / Deduplicação
        // ... (restante do código inalterado) ...
        private List<SugestaoSegmento> RemoverSugestoesDuplicadas(List<SugestaoSegmento> sugestoes)
        {
            var resultado = new List<SugestaoSegmento>();
            var nomes = new List<string>();

            // Priorizar a sugestão de maior confiança para retenção
            foreach (var sugestao in sugestoes.OrderByDescending(x => x.ConfiancaPercentual).ThenByDescending(x => x.QuantidadeLicitacoesOriginarias))
            {
                var norm = _normalizador.Normalizar(sugestao.NomeSugerido ?? string.Empty);
                if (string.IsNullOrWhiteSpace(norm)) continue;

                bool similarEncontrado = nomes.Any(existing => NomesSimilares(existing, norm));
                if (!similarEncontrado)
                {
                    resultado.Add(sugestao);
                    nomes.Add(norm);
                }
                else
                {
                    AdicionarMensagemDebug($"DEBUG: Sugestão descartada por similaridade interna: {sugestao.NomeSugerido}");
                }
            }

            return resultado;
        }

        private List<SugestaoSegmento> FiltrarSugestoesJaExistentes(List<SugestaoSegmento> sugestoes, HashSet<string> nomesSegmentosExistentes, HashSet<string> nomesSugestoesExistentes)
        {
            var resultado = new List<SugestaoSegmento>();

            foreach (var s in sugestoes)
            {
                var norm = _normalizador.Normalizar(s.NomeSugerido ?? string.Empty);
                if (string.IsNullOrWhiteSpace(norm)) continue;

                bool existeSegmento = nomesSegmentosExistentes.Any(ns => NomesSimilares(ns, norm));
                bool existeSugestao = nomesSugestoesExistentes.Any(ns => NomesSimilares(ns, norm));

                if (existeSegmento || existeSugestao)
                {
                    AdicionarMensagemDebug($"DEBUG: Sugestão '{s.NomeSugerido}' filtrada (já existe no BD ou em sugestões)");
                    continue;
                }

                resultado.Add(s);
            }

            return resultado;
        }

        private bool NomesSimilares(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
            if (a == b) return true;
            if (a.Contains(b) || b.Contains(a)) return true;

            var dist = DistanciaLevenshtein(a, b);
            return dist <= 2;
        }
        #endregion

        #region Utilitários de Extração/Confiança

        private decimal CalcularConfiancaTFIDF(decimal scoreTotal, int frequenciaDocs, int totalLicitacoes)
        {
            // Base: 55%, incremento baseado na frequência e no score do termo
            var baseConf = 55m;
            var fatorFrequencia = Math.Min(40m, (decimal)frequenciaDocs / totalLicitacoes * 100m * 1.5m);
            var fatorScore = Math.Min(15m, scoreTotal * 5m);

            return Math.Min(100m, baseConf + fatorFrequencia + fatorScore);
        }

        private decimal CalcularConfiancaGrupo(List<Licitacao> grupo, int totalLicitacoes)
        {
            var baseConf = 50m;
            // O incremento é proporcional ao tamanho do grupo de itens similares (8m por item)
            var incremento = Math.Min(45m, grupo.Count * 8m);
            return Math.Min(100m, baseConf + incremento);
        }
        #endregion

        #region Misc helpers
        private int DistanciaLevenshtein(string s1, string s2)
        {
            if (s1 == null) s1 = string.Empty;
            if (s2 == null) s2 = string.Empty;

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
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        private string CapitalizarPrimeira(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;
            texto = texto.Trim();
            return char.ToUpperInvariant(texto[0]) + (texto.Length > 1 ? texto.Substring(1).ToLowerInvariant() : string.Empty);
        }
        #endregion

        public List<string> ObterMensagens() => _mensagensDebug;

       
        /// <summary>
        /// Obtém uma sugestão específica pelo ID.
        /// </summary>
        public async Task<SugestaoSegmento?> ObterSugestaoAsync(long id)
        {
            return await _repositorioSugestoes.ObterPorIdAssincrono(id) as SugestaoSegmento;
        }

        /// <summary>
        /// Obtém sugestões que estão com o status 'Pendente', aplicando a filtragem
        /// de Confiança Mínima no banco de dados para melhor performance.
        /// </summary>
        /// <param name="confiancaMinima">Score mínimo de 0 a 100. Nulo ignora o filtro.</param>
        public async Task<List<SugestaoSegmento>> ObterSugestoesPendentesAssincrono(decimal? confiancaMinima = null)
        {
            AdicionarMensagemDebug($"DEBUG: Buscando sugestões pendentes (Confiança Minima: {confiancaMinima?.ToString() ?? "N/A"})");

            Expression<Func<SugestaoSegmento, bool>> predicate;

            if (confiancaMinima.HasValue)
            {
                predicate = s => s.Status == StatusAprovacaoSugestao.Pendente && s.ConfiancaPercentual >= confiancaMinima.Value;
            }
            else
            {
                predicate = s => s.Status == StatusAprovacaoSugestao.Pendente;
            }

            var sugestoes = await _repositorioSugestoes.BuscarAssincrono(predicate);

            var sugestoesOrdenadas = sugestoes.OrderByDescending(s => s.ConfiancaPercentual).ToList();

            AdicionarMensagemDebug($"DEBUG: {sugestoesOrdenadas?.Count} sugestões pendentes encontradas.");
            return sugestoesOrdenadas;
        }


        /// <summary>
        /// Aprova uma sugestão, criando ou atualizando um Segmento e atualizando o status.
        /// </summary>
        public async Task<Segmento> AprovarSugestaoAsync(SugestaoSegmento sugestao, string usuarioRevisao)
        {
            if (sugestao.Status != StatusAprovacaoSugestao.Pendente)
            {
                AdicionarErroDebug($"Tentativa de aprovar sugestão não pendente: ID {sugestao.Id}.");
                throw new InvalidOperationException("Somente sugestões pendentes podem ser aprovadas.");
            }

            var novoSegmento = new Segmento(sugestao.NomeSugerido);

            await _repositorioSegmentos.AdicionarAssincrono(novoSegmento);

            if (!string.IsNullOrWhiteSpace(sugestao.LicitacaoIds))
            {
                try
                {
                    var licitacaoIds = JsonSerializer.Deserialize<List<long>>(sugestao.LicitacaoIds);
                    if (licitacaoIds != null && licitacaoIds.Any())
                    {
                        var vinculos = licitacaoIds.Select(id => new LicitacaoSegmento
                        {
                            SegmentoId = novoSegmento.Id,
                            LicitacaoId = id,
                            CriadoEm = DateTime.UtcNow
                        }).ToList();

                        await _repositorioLicitacaoSegmento.BulkAdicionarAssincrono(vinculos);
                        AdicionarMensagemDebug($"DEBUG: {vinculos.Count} licitações vinculadas ao novo segmento '{novoSegmento.Nome}'.");
                    }
                }
                catch (Exception ex)
                {
                    AdicionarErroDebug($"Falha ao deserializar ou vincular licitações na aprovação: {ex.Message}");
                }
            }

            sugestao.Status = StatusAprovacaoSugestao.Aprovada;
            sugestao.UsuarioRevisao = usuarioRevisao;
            sugestao.DataRevisao = DateTime.UtcNow;
            sugestao.SegmentoIdGerado = novoSegmento.Id;

            _repositorioSugestoes.AtualizarAssincrono(sugestao);
            await _repositorioSugestoes.SalvarAlteracoesAssincrono();

            AdicionarMensagemDebug($"Sugestão ID {sugestao.Id} aprovada e Segmento '{novoSegmento.Nome}' criado com sucesso.");
            return novoSegmento;
        }

        /// <summary>
        /// Rejeita uma sugestão, atualizando seu status e registrando o motivo.
        /// </summary>
        public async Task<bool> RejeitarSugestaoAsync(SugestaoSegmento sugestao, string motivo, string usuarioRevisao)
        {
            if (sugestao.Status != StatusAprovacaoSugestao.Pendente)
            {
                AdicionarErroDebug($"Tentativa de rejeitar sugestão não pendente: ID {sugestao.Id}.");
                return false;
            }

            sugestao.Status = StatusAprovacaoSugestao.Rejeitada;
            sugestao.UsuarioRevisao = usuarioRevisao;
            sugestao.DataRevisao = DateTime.UtcNow;
            sugestao.MotivoRejeicao = motivo;

            _repositorioSugestoes.AtualizarAssincrono(sugestao);
            await _repositorioSugestoes.SalvarAlteracoesAssincrono();

            AdicionarMensagemDebug($"Sugestão ID {sugestao.Id} rejeitada por '{usuarioRevisao}'. Motivo: {motivo}.");
            return true;
        }

        /// <summary>
        /// Obtém as estatísticas gerais de sugestões no sistema.
        /// </summary>
        public async Task<(int Total, int Pendentes, int Aprovadas, int Rejeitadas, decimal ConfiancaMedia, decimal ConfiancaMinima, decimal ConfiancaMaxima)> ObterEstatisticasAsync()
        {
            var sugestoes = (await _repositorioSugestoes.ObterTodosAssincrono()).ToList();

            if (!sugestoes.Any())
                return (0, 0, 0, 0, 0m, 0m, 0m);

            var total = sugestoes.Count;
            var pendentes = sugestoes.Count(s => s.Status == StatusAprovacaoSugestao.Pendente);
            var aprovadas = sugestoes.Count(s => s.Status == StatusAprovacaoSugestao.Aprovada);
            var rejeitadas = sugestoes.Count(s => s.Status == StatusAprovacaoSugestao.Rejeitada);

            var confiancas = sugestoes.Select(s => s.ConfiancaPercentual).ToList();
            var confiancaMedia = confiancas.Any() ? confiancas.Average() : 0m;
            var confiancaMinima = confiancas.Any() ? confiancas.Min() : 0m;
            var confiancaMaxima = confiancas.Any() ? confiancas.Max() : 0m;

            return (total, pendentes, aprovadas, rejeitadas, confiancaMedia, confiancaMinima, confiancaMaxima);
        }
    }
}