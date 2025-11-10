using Microsoft.EntityFrameworkCore;
using RadarHub.Dominio.Entidades;
using RSK.Dominio.Autorizacao.Entidades;

namespace RadarHub.Infraestrutura;

public partial class RadarHubDbContext : DbContext
{
    public DbSet<Modalidade> Modalidades { get; set; }
    public DbSet<Orgao> Orgaos { get; set; }
    public DbSet<Unidade> Unidades { get; set; }
    public DbSet<Ufs> Ufs { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<Poder> Poderes { get; set; }
    public DbSet<Tipo> Tipos { get; set; }
    public DbSet<Esfera> Esferas { get; set; }
    public DbSet<AplicacaoPermitida> AplicacaoPermitida { get; set; }
    public DbSet<Permissao> Permissao { get; set; }
    public DbSet<AplicacaoPermitidaPermissao> AplicacaoPermitidaPermissao { get; set; }
    public DbSet<Perfil> Perfil { get; set; }
    public DbSet<PerfilPermissao> PerfilPermissao { get; set; }
    public DbSet<UsuarioBase> UsuarioBase { get; set; }
    public DbSet<UsuarioPerfil> UsuarioPerfil { get; set; }
    public DbSet<Licitacao> Licitacoes { get; set; }
    public DbSet<Segmento> Segmentos { get; set; }
    public DbSet<Plano> Planos { get; set; }


    public DbSet<TipoMargemPreferencia> TiposMargemPreferencia { get; set; }

    public DbSet<FonteOrcamentaria> FontesOrcamentarias { get; set; }
    public RadarHubDbContext()
    {
    }

    public RadarHubDbContext(DbContextOptions<RadarHubDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
