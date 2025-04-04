namespace SaaSBack.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int Tenant_Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
