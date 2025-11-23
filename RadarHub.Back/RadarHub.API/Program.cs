using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using RadarHub.Dominio.Entidades;
using RadarHub.Dominio.Servicos;
using RadarHub.Infraestrutura;
using RadarHub.Infraestrutura.Repositorios;
using RSK.API.Extensoes;
using RSK.Dominio.Autorizacao.Interfaces;
using RSK.Dominio.Autorizacao.Servicos;
using RSK.Dominio.Entidades;
using RSK.Dominio.Interfaces;
using RSK.Dominio.IRepositorios;
using RSK.Dominio.Notificacoes.Interfaces;
using RSK.Dominio.Notificacoes.Servicos;
using RSK.Dominio.Servicos;
using RSK.Infraestrutura.Dados;
using RSK.Infraestrutura.Repositorios;
using RSK.Infraestrutura.Servicos;
using System.Text;
using RSK.Dominio.Autorizacao.Entidades;
using Microsoft.OpenApi.Models;
using RSK.Agendador.Enums;
using Quartz;
using RSK.Agendador.Extensoes;
using RadarHub.Dominio.Servicos.Analise;
using RadarHub.Integracoes.Gemini.Analise;
using RadarHub.Integracoes.Gemini;
using RadarHub.Integracoes.Gemini.Configuracao;


namespace RadarHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            builder.Services.Configure<GeminiSettings>(configuration.GetSection("Gemini"));

            // ======= DbContext =======
            builder.Services.AddDbContext<RadarHubDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("RadarGovDb"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("RadarGovDb"))
                )
            );

            // ======= CORS =======
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ======= JWT Authentication =======
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // ======= Serviços de domínio =======
            builder.Services.AddScoped<ModalidadeServico>();
            builder.Services.AddScoped<MunicipioServico>();
            builder.Services.AddScoped<OrgaoServico>();
            builder.Services.AddScoped<UnidadeServico>();
            builder.Services.AddScoped<UfsServico>();
            builder.Services.AddScoped<PoderServico>();
            builder.Services.AddScoped<TipoServico>();
            builder.Services.AddScoped<EsferaServico>();
            builder.Services.AddScoped<TipoMargemPreferenciaServico>();
            builder.Services.AddScoped<FonteOrcamentariaServico>();
            builder.Services.AddScoped<LicitacaoServico>();
            builder.Services.AddScoped<SegmentoServico>();
            builder.Services.AddScoped<PlanoServico>();
            builder.Services.AddScoped<SugestorSegmentosServico>();
            builder.Services.AddScoped<NormalizadorTextoServico>();
            builder.Services.AddScoped<AnalisadorSimilaridadeServico>();
            builder.Services.AddScoped<AnalisadorTFIDFServico>();
            builder.Services.AddScoped<ClassificadorSegmentosServico>();
            builder.Services.AddScoped<ExtractorDeCategoriasProdutosServico>();


            // ======= Agendamentos ========
            builder.Services.AddQuartz(); // Registra Quartz
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            builder.Services.AdicionarJobServico<ModalidadeServico>(
                nomeJob: "ImportacaoModalidades",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<MunicipioServico>(
                nomeJob: "ImportacaoMunicipio",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<OrgaoServico>(
                nomeJob: "ImportacaoOrgao",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<UnidadeServico>(
                nomeJob: "ImportacaoUnidade",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<UfsServico>(
                nomeJob: "ImportacaoUfs",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<PoderServico>(
                nomeJob: "ImportacaoPoder",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<TipoServico>(
                nomeJob: "ImportacaoTipo",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<EsferaServico>(
                nomeJob: "ImportacaoEsfera",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<TipoMargemPreferenciaServico>(
                nomeJob: "ImportacaoTipoMargemPreferencia",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            );
            builder.Services.AdicionarJobServico<FonteOrcamentariaServico>(
                nomeJob: "ImportacaoFonteOrcamentaria",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Semanal,
                horaInicio: new TimeOnly(3, 0)
            ); 
            builder.Services.AdicionarJobServico<LicitacaoServico>(
                nomeJob: "ImportacaoLicitacao",
                metodo: "ImportarAsync",
                tipoAgendamento: TipoAgendamento.Diario,
                horaInicio: new TimeOnly(2, 0)
            );

            // ======= Reposit�rios de importa��o =======
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Modalidade>, RepositorioImportacaoTerceiro<Modalidade, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Orgao>, RepositorioImportacaoTerceiro<Orgao, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Unidade>, RepositorioImportacaoTerceiro<Unidade, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Ufs>, RepositorioImportacaoTerceiro<Ufs, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Municipio>, RepositorioImportacaoTerceiro<Municipio, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Poder>, RepositorioImportacaoTerceiro<Poder, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Tipo>, RepositorioImportacaoTerceiro<Tipo, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Esfera>, RepositorioImportacaoTerceiro<Esfera, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<TipoMargemPreferencia>, RepositorioImportacaoTerceiro<TipoMargemPreferencia, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<FonteOrcamentaria>, RepositorioImportacaoTerceiro<FonteOrcamentaria, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioImportacaoTerceiro<Licitacao>, RepositorioImportacaoTerceiro<Licitacao, RadarHubDbContext>>();


            builder.Services.AddScoped<IRepositorioBaseAssincrono<Modalidade>, RepositorioBaseAssincrono<Modalidade, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Orgao>, RepositorioBaseAssincrono<Orgao, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Unidade>, RepositorioBaseAssincrono<Unidade, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Ufs>, RepositorioBaseAssincrono<Ufs, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Municipio>, RepositorioBaseAssincrono<Municipio, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Poder>, RepositorioBaseAssincrono<Poder, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Tipo>, RepositorioBaseAssincrono<Tipo, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Esfera>, RepositorioBaseAssincrono<Esfera, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<TipoMargemPreferencia>, RepositorioBaseAssincrono<TipoMargemPreferencia, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<FonteOrcamentaria>, RepositorioBaseAssincrono<FonteOrcamentaria, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Licitacao>, RepositorioBaseAssincrono<Licitacao, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Segmento>, RepositorioBaseAssincrono<Segmento, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<LicitacaoSegmento>, RepositorioBaseAssincrono<LicitacaoSegmento, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<Plano>, RepositorioBaseAssincrono<Plano, RadarHubDbContext>>();
            builder.Services.AddScoped<IRepositorioBaseAssincrono<SugestaoSegmento>, RepositorioSugestaoSegmento>();

            // ======= Reposit�rios e servi�os base =======
            builder.Services.AddScoped(typeof(IRepositorioBaseAssincrono<>), typeof(RepositorioBaseRadarHub<>));
            builder.Services.AddScoped(typeof(IServicoConsultaBase<>), typeof(ServicoConsultaBase<>));
            builder.Services.AddScoped(typeof(IServicoCrudBase<>), typeof(ServicoCrudBase<>));
            builder.Services.AddScoped(typeof(IServicoImportacaoTerceiro<>), typeof(ServicoImportacaoTerceiro<>));

            // ======= Notifica��es =======
            builder.Services.AddScoped<IServicoUsuario, ServicoUsuario>();
            builder.Services.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();


            // ======= Transa��o =======
            builder.Services.AddScoped<ITransacao, TransacaoEF<RadarHubDbContext>>();

            // ======= Autoriza��o =======
            builder.Services.AddScoped<IHasher, Sha256Hasher>();
            builder.Services.AddScoped<IRepositorioAplicacaoPermitida, RepositorioAplicacaoPermitida<RadarHubDbContext>>();
            builder.Services.AddScoped<IServicoAplicacaoPermitidaPermissao, ServicoAplicacaoPermitidaPermissao>();
            builder.Services.AddScoped<IServicoVerificacaoAplicacao, ServicoVerificacaoAplicacao>();


            // Registra a classe concreta
            builder.Services.AddScoped<ServicoMensagem>();

            // Ambas as interfaces usam a mesma inst�ncia
            builder.Services.AddScoped<IServicoMensagem>(sp => sp.GetRequiredService<ServicoMensagem>());
            builder.Services.AddScoped<INotificador>(sp => sp.GetRequiredService<ServicoMensagem>());


            builder.Services.AddHttpClient<IAnalisadorSemanticoExterno, GeminiAnalisadorServico>();


            // ======= Serviços RSK =======
            builder.Services.AdicionarServicosRSK();

            // ======= Controllers =======
            builder.Services.AddControllers()
            .AddApplicationPart(typeof(RSK.API.Controllers.UsuarioController).Assembly)
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            })
           .AddOData(opt =>
           {
               opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);                                                                              
               opt.AddRouteComponents("api", GetEdmModel());
           });


            // ======= Swagger =======
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Define o esquema de seguran�a Bearer
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT no campo abaixo: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Aplica o Bearer a todos os endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });


            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var contexto = scope.ServiceProvider.GetRequiredService<RadarHubDbContext>();
            //    Console.WriteLine($"Conex�o: {contexto.Database.GetConnectionString()}");

            //    contexto.Add(new Modalidade { IdTerceiro = "teste", Nome = "TesteDireto" });
            //    var result = contexto.SaveChanges();
            //    Console.WriteLine($"{result} registro(s) salvo(s)!");
            //}


            // ======= Middleware pipeline =======
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll"); // Habilita CORS
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseODataRouteDebug();

            app.MapControllers();
            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            // Configura��o manual de cada EntitySet (conjunto de entidades) para OData,
            // conforme solicitado. � crucial chamar .HasKey(e => e.Id) para
            // garantir que o OData reconhe�a a chave prim�ria herdada da EntidadeBase.

            // Entidades de RadarHub.Dominio.Entidades
            builder.EntitySet<Modalidade>("Modalidade").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Municipio>("Municipio").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Orgao>("Orgao").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Unidade>("Unidade").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Ufs>("Ufs").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Poder>("Poder").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Tipo>("Tipo").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Esfera>("Esfera").EntityType.HasKey(e => e.Id);
            builder.EntitySet<TipoMargemPreferencia>("TipoMargemPreferencia").EntityType.HasKey(e => e.Id);
            builder.EntitySet<FonteOrcamentaria>("FonteOrcamentaria").EntityType.HasKey(e => e.Id);
            builder.EntitySet<UsuarioBase>("UsuarioBase").EntityType.HasKey(e => e.Id);
            builder.EntitySet<Licitacao>("Licitacao").EntityType.HasKey(e => e.Id);

            // Entidades de RSK.Dominio.Autorizacao.Entidades
            builder.EntitySet<AplicacaoPermitida>("AplicacaoPermitida").EntityType.HasKey(e => e.Id);

            // NOTE: A classe EntidadeBaseImportacaoTerceiro n�o � registrada pois � uma classe base
            // e n�o deve ser um EntitySet consult�vel.

            return builder.GetEdmModel();
        }

    }
}
