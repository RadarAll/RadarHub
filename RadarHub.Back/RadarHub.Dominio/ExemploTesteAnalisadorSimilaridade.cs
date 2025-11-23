using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos.Analise;
using System;
using System.Collections.Generic;

namespace RadarHub.Testes
{
    /// <summary>
    /// Exemplos de teste do AnalisadorSimilaridadeServico
    /// Demonstra como usar o algoritmo para diferentes cenários
    /// </summary>
    public class ExemploTesteAnalisadorSimilaridade
    {
        public static void ExecutarTestes()
        {
            var analisador = new AnalisadorSimilaridadeServico();

            Console.WriteLine("=== TESTES DE ANÁLISE DE SIMILARIDADE ===\n");

            // Teste 1: Match exato
            Console.WriteLine("TESTE 1: Match Exato");
            var resultado1 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Tecnologia da Informação",
                tituloLicitacao: "LICITAÇÃO ELETRÔNICA: Aquisição de Produtos de Tecnologia da Informação",
                descricaoLicitacao: "Edital para aquisição de equipamentos de TI",
                palavrasChaveSegmento: null);
            
            Console.WriteLine($"Score: {resultado1.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado1.TipoCorrespondencia}");
            Console.WriteLine($"Texto Encontrado: {resultado1.TextoEncontrado}");
            Console.WriteLine($"Resultado: {(resultado1.ScoreSimilaridade >= 80 ? "✓ PASSOU" : "✗ FALHOU")}\n");

            // Teste 2: Palavras-chave
            Console.WriteLine("TESTE 2: Correspondência por Palavras-chave");
            var resultado2 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Construção Civil",
                tituloLicitacao: "Licitação para Construção de Edifício Administrativo",
                descricaoLicitacao: "Projeto de construção com fundações em concreto armado",
                palavrasChaveSegmento: new List<string> { "construção", "edifício", "concreto", "obra" });
            
            Console.WriteLine($"Score: {resultado2.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado2.TipoCorrespondencia}");
            Console.WriteLine($"Texto Encontrado: {resultado2.TextoEncontrado}");
            Console.WriteLine($"Resultado: {(resultado2.ScoreSimilaridade >= 40 ? "✓ PASSOU" : "✗ FALHOU")}\n");

            // Teste 3: Conteúdo similar
            Console.WriteLine("TESTE 3: Conteúdo Similar com Levenshtein");
            var resultado3 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Segurança Pública",
                tituloLicitacao: "EDITAL: Contratação de Serviços de Segurança Pública",
                descricaoLicitacao: "Prestação de serviços especializados de proteção e vigilância",
                palavrasChaveSegmento: null);
            
            Console.WriteLine($"Score: {resultado3.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado3.TipoCorrespondencia}");
            Console.WriteLine($"Texto Encontrado: {resultado3.TextoEncontrado}");
            Console.WriteLine($"Resultado: {(resultado3.ScoreSimilaridade >= 40 ? "✓ PASSOU" : "✗ FALHOU")}\n");

            // Teste 4: Sem similaridade
            Console.WriteLine("TESTE 4: Sem Similaridade");
            var resultado4 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Saúde Pública",
                tituloLicitacao: "Compra de Pneus para Caminhões",
                descricaoLicitacao: "Pneumáticos diversos para frota de transporte",
                palavrasChaveSegmento: null);
            
            Console.WriteLine($"Score: {resultado4.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado4.TipoCorrespondencia}");
            Console.WriteLine($"Resultado: {(!analisador.EhAceitavel(resultado4.ScoreSimilaridade) ? "✓ PASSOU (corretamente rejeitado)" : "✗ FALHOU")}\n");

            // Teste 5: Variações de escrita
            Console.WriteLine("TESTE 5: Variações de Escrita (com acentos e caracteres especiais)");
            var resultado5 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Educação - Ensino Superior",
                tituloLicitacao: "LICITACAO: Aquisição de Equipamentos para Educacao Ensino Superior",
                descricaoLicitacao: "Compra de laboratórios didáticos para educação",
                palavrasChaveSegmento: null);
            
            Console.WriteLine($"Score: {resultado5.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado5.TipoCorrespondencia}");
            Console.WriteLine($"Resultado: {(resultado5.ScoreSimilaridade >= 40 ? "✓ PASSOU" : "✗ FALHOU")}\n");

            // Teste 6: Múltiplas palavras-chave
            Console.WriteLine("TESTE 6: Múltiplas Palavras-chave");
            var resultado6 = analisador.AnalisarSimilaridade(
                nomeSegmento: "Logística e Transportes",
                tituloLicitacao: "Licitação para Serviços de Transportes e Logística",
                descricaoLicitacao: "Contratação de empresa para transportes de cargas com distribuição logística",
                palavrasChaveSegmento: new List<string> { 
                    "transportes", "logística", "cargas", "distribuição", "frota" });
            
            Console.WriteLine($"Score: {resultado6.ScoreSimilaridade:F2}");
            Console.WriteLine($"Tipo: {resultado6.TipoCorrespondencia}");
            Console.WriteLine($"Texto Encontrado: {resultado6.TextoEncontrado}");
            Console.WriteLine($"Detalhes dos Scores:");
            foreach (var detalhe in resultado6.DetalhesScores)
            {
                Console.WriteLine($"  - {detalhe.Key}: {detalhe.Value:F2}");
            }
            Console.WriteLine($"Resultado: {(resultado6.ScoreSimilaridade >= 60 ? "✓ PASSOU" : "✗ FALHOU")}\n");

            // Resumo
            Console.WriteLine("=== RESUMO DOS TESTES ===");
            var testes = new[] { resultado1, resultado2, resultado3, resultado4, resultado5, resultado6 };
            var aprovados = 0;

            foreach (var teste in testes)
            {
                if (teste.ScoreSimilaridade >= 40)
                    aprovados++;
            }

            Console.WriteLine($"Total de Testes: {testes.Length}");
            Console.WriteLine($"Testes Aprovados (conceitual): {aprovados}");
            Console.WriteLine($"Taxa de Aprovação: {(aprovados * 100 / testes.Length)}%");
        }

        /// <summary>
        /// Demonstra como usar o serviço em um cenário real
        /// </summary>
        public static void ExemploUsoReal()
        {
            Console.WriteLine("\n=== EXEMPLO DE USO REAL ===\n");

            var analisador = new AnalisadorSimilaridadeServico();

            // Simulando licitações importadas
            var licitacoes = new List<(string id, string titulo, string descricao)>
            {
                ("1", "Aquisição de Servidores para Dados",
                    "Compra de 50 servidores high-performance para data center"),
                ("2", "Reformas em Creche Municipal",
                    "Reforma completa de imóvel destinado a educação infantil"),
                ("3", "Pneus para Frota Municipal",
                    "Aquisição de pneumáticos para caminhões de coleta"),
            };

            // Simulando segmentos cadastrados
            var segmentos = new List<(string id, string nome)>
            {
                ("S1", "Tecnologia da Informação"),
                ("S2", "Educação"),
                ("S3", "Transportes"),
            };

            Console.WriteLine("Analisando 3 licitações contra 3 segmentos...\n");

            foreach (var licita in licitacoes)
            {
                Console.WriteLine($"Licitação: {licita.titulo}");

                foreach (var seg in segmentos)
                {
                    var resultado = analisador.AnalisarSimilaridade(
                        seg.nome, licita.titulo, licita.descricao, null);

                    if (analisador.EhAceitavel(resultado.ScoreSimilaridade))
                    {
                        var confianca = analisador.EhAltaConfianca(resultado.ScoreSimilaridade) 
                            ? "✓ Alta" : "◐ Média";
                        
                        Console.WriteLine(
                            $"  → {seg.nome}: {resultado.ScoreSimilaridade:F1} " +
                            $"({resultado.TipoCorrespondencia}) {confianca}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
