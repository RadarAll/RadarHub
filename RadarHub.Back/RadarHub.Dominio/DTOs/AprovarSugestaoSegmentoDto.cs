namespace RadarHub.Dominio.DTOs
{
    /// <summary>
    /// DTO para aprovar uma sugestão de segmento
    /// </summary>
    public class AprovarSugestaoSegmentoDto
    {
        /// <summary>
        /// Usuário que está revisando a sugestão
        /// </summary>
        public string UsuarioRevisao { get; set; }
    }
}
