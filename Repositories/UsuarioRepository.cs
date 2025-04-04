using Dapper;
using Npgsql;
using SaaSBack.Models;

namespace SaaSBack.Repositories
{
    public class UsuarioRepository(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));

        public async Task<bool> CadastrarUsuarioAsync(string email, string senha, string nome)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            var query = @"
                INSERT INTO Usuarios (id, email, senha, nome)
                VALUES (gen_random_uuid(), @Email, @Senha, @Nome)";

            var result = await connection.ExecuteAsync(query, new
            {
                Email = email,
                Senha = senhaHash,
                Nome = nome
            });

            return result > 0;
        }

        public async Task<Usuario?> GetUsuarioByEmailAndSenhaAsync(string email, string senha)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT * FROM Usuarios WHERE email ILIKE @Email";

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { Email = email });

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                return null;
            }

            return usuario;
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT * FROM Usuarios WHERE email = @Email";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { Email = email });
        }
    }
}
