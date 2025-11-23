namespace RadarHub.Dominio.DTOs
{
    /// <summary>
    /// DTO para rejeitar uma sugestão de segmento
    /// </summary>
    public class RejeitarSugestaoSegmentoDto
    {
        /// <summary>
        /// Motivo da rejeição
        /// </summary>
        public string Motivo { get; set; }

        /// <summary>
        /// Usuário que está revisando a sugestão
        /// </summary>
        public string UsuarioRevisao { get; set; }
    }
}
