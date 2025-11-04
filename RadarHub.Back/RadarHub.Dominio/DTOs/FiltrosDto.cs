namespace RadarHub.Dominio.DTOs
{
    public class FiltrosDto
    {
        public Filters Filters { get; set; } 
    }
    public class Filters
    {
        public List<ModalidadeDto> Modalidades { get; set; } = new();
        public List<OrgaoDto> Orgaos { get; set; } = new();
        public List<UnidadeDto> Unidades { get; set; } = new();
        public List<AnoDto> Anos { get; set; } = new();
        public List<UfDto> Ufs { get; set; } = new();
        public List<MunicipioDto> Municipios { get; set; } = new();
        public List<EsferaDto> Esferas { get; set; } = new();
        public List<PoderDto> Poderes { get; set; } = new();
        public List<TipoDto> Tipos { get; set; } = new();
        public List<TipoContratoDto> TiposContrato { get; set; } = new();
        public List<FonteOrcamentariaDto> FontesOrcamentarias { get; set; } = new();
        public List<TipoMargemPreferenciaDto> TiposMargensPreferencia { get; set; } = new();
    }

    // DTOs para cada tipo de item dentro das listas

    public class ModalidadeDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class OrgaoDto
    {
        public string Id { get; set; }
        public string Cnpj { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class UnidadeDto
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
        public string CodigoNome { get; set; }
    }

    public class AnoDto
    {
        public string Ano { get; set; }
        public int Total { get; set; }
    }

    public class UfDto
    {
        public string Id { get; set; }
        public int Total { get; set; }
    }

    public class MunicipioDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class EsferaDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class PoderDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class TipoDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class TipoContratoDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class FonteOrcamentariaDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }

    public class TipoMargemPreferenciaDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
    }
}
