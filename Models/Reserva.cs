namespace RecargaHubBack.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public int VeiculoId { get; set; }
        public int PontoRecargaId { get; set; }
        public DateTime DataReserva { get; set; }
        public TimeSpan HorarioInicio { get; set; }
        public TimeSpan HorarioFim { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}