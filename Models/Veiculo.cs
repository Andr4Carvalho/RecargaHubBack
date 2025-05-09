namespace RecargaHubBack.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Tipo_Conector { get; set; } = string.Empty;
        public decimal Capacidade_Bateria { get; set; }
        public string Cor { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
