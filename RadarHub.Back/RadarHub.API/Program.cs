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


namespace RadarHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

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

            // ======= Repositórios de importação =======
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

            // ======= Repositórios e serviços base =======
            builder.Services.AddScoped(typeof(IRepositorioBaseAssincrono<>), typeof(RepositorioBaseRadarHub<>));
            builder.Services.AddScoped(typeof(IServicoConsultaBase<>), typeof(ServicoConsultaBase<>));
            builder.Services.AddScoped(typeof(IServicoCrudBase<>), typeof(ServicoCrudBase<>));
            builder.Services.AddScoped(typeof(IServicoImportacaoTerceiro<>), typeof(ServicoImportacaoTerceiro<>));

            // ======= Notificações =======
            builder.Services.AddScoped<IServicoUsuario, ServicoUsuario>();
            builder.Services.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();


            // ======= Transação =======
            builder.Services.AddScoped<ITransacao, TransacaoEF<RadarHubDbContext>>();

            // ======= Autorização =======
            builder.Services.AddScoped<IHasher, Sha256Hasher>();
            builder.Services.AddScoped<IRepositorioAplicacaoPermitida, RepositorioAplicacaoPermitida<RadarHubDbContext>>();
            builder.Services.AddScoped<IServicoAplicacaoPermitidaPermissao, ServicoAplicacaoPermitidaPermissao>();
            builder.Services.AddScoped<IServicoVerificacaoAplicacao, ServicoVerificacaoAplicacao>();


            // Registra a classe concreta
            builder.Services.AddScoped<ServicoMensagem>();

            // Ambas as interfaces usam a mesma instância
            builder.Services.AddScoped<IServicoMensagem>(sp => sp.GetRequiredService<ServicoMensagem>());
            builder.Services.AddScoped<INotificador>(sp => sp.GetRequiredService<ServicoMensagem>());


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
                // Define o esquema de segurança Bearer
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

            // ======= Middleware pipeline =======
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");            // Habilita CORS
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            // Configuração manual de cada EntitySet (conjunto de entidades) para OData,
            // conforme solicitado. É crucial chamar .HasKey(e => e.Id) para
            // garantir que o OData reconheça a chave primária herdada da EntidadeBase.

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

            // Entidades de RSK.Dominio.Autorizacao.Entidades
            builder.EntitySet<AplicacaoPermitida>("AplicacaoPermitida").EntityType.HasKey(e => e.Id);

            // NOTE: A classe EntidadeBaseImportacaoTerceiro não é registrada pois é uma classe base
            // e não deve ser um EntitySet consultável.

            return builder.GetEdmModel();
        }

    }
}
