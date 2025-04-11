namespace RecargaHubBack.Models
{
    public class PontoDeRecarga
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Potencia { get; set; }
        public bool Ativo { get; set; }
    }
}
