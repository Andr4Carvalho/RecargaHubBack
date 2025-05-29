namespace RecargaHubBack.Models
{
    public class PontoDeRecarga
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Potencia { get; set; }
        public bool Ativo { get; set; }
    }
}
