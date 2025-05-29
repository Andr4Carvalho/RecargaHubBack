using Dapper;
using Npgsql;
using RecargaHubBack.Models;

namespace RecargaHubBack.Repositories
{
    public class AvaliacaoRepository(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));

        public async Task<IEnumerable<Avaliacao>> ListarPorPontoAsync(int pontoRecargaId)
        {
            const string sql = @"
                SELECT id, ponto_recarga_id, nota, comentario, tags, data_avaliacao
                FROM avaliacoes_ponto_recarga
                WHERE ponto_recarga_id = @pontoRecargaId
                ORDER BY data_avaliacao DESC";

            using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Avaliacao>(sql, new { pontoRecargaId });
        }

        public async Task<int> CriarAsync(Avaliacao avaliacao)
        {
            const string sql = @"
                INSERT INTO avaliacoes_ponto_recarga (ponto_recarga_id, reserva_id, usuario_id, nota, comentario, tags)
                VALUES (@PontoRecargaId, @ReservaId, @UsuarioId, @Nota, @Comentario, @Tags)
                RETURNING id;";

            using var conn = new NpgsqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(sql, avaliacao);
        }
    }

}
