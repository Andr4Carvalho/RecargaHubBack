using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using RecargaHubBack.Models;

namespace RecargaHubBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PontosDeRecargaController(IConfiguration config) : ControllerBase
    {
        private readonly IDbConnection _db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));

        [HttpGet]
        public async Task<IEnumerable<PontoDeRecarga>> ListarTodos()
        {
            var sql = "SELECT id, nome, endereco, tipo_conector AS Tipo, potencia_kw AS Potencia, ativo, criado_em FROM pontos_recarga ORDER BY id";
            return await _db.QueryAsync<PontoDeRecarga>(sql);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PontoDeRecarga>> BuscarPorId(int id)
        {
            var sql = "SELECT id, nome, endereco, tipo_conector AS Tipo, potencia_kw AS Potencia, ativo, criado_em FROM pontos_recarga WHERE id = @id";
            var ponto = await _db.QueryFirstOrDefaultAsync<PontoDeRecarga>(sql, new { id });
            if (ponto == null) return NotFound();
            return Ok(ponto);
        }

        [HttpPost]
        public async Task<ActionResult> Criar(PontoDeRecarga ponto)
        {
            var sql = @"INSERT INTO pontos_recarga (nome, endereco, tipo_conector, potencia_kw, ativo)
                        VALUES (@Nome, @Endereco, @Tipo, @Potencia, @Ativo) RETURNING id";
            var id = await _db.ExecuteScalarAsync<int>(sql, ponto);
            return CreatedAtAction(nameof(BuscarPorId), new { id }, ponto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(int id, PontoDeRecarga ponto)
        {
            ponto.Id = id;
            var sql = @"UPDATE pontos_recarga SET
                        nome = @Nome,
                        endereco = @Endereco,
                        tipo_conector = @Tipo,
                        potencia_kw = @Potencia,
                        ativo = @Ativo
                        WHERE id = @Id";
            var result = await _db.ExecuteAsync(sql, ponto);
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id)
        {
            var sql = "DELETE FROM pontos_recarga WHERE id = @id";
            var result = await _db.ExecuteAsync(sql, new { id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
