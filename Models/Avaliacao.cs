namespace RecargaHubBack.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public int PontoRecargaId { get; set; }
        public int ReservaId { get; set; }
        public Guid UsuarioId { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
        public string[] Tags { get; set; } = [];
        public DateTime DataAvaliacao { get; set; }
    }

}