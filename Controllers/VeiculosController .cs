using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using RecargaHubBack.Models;
using RecargaHubBack.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RecargaHubBack.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class VeiculoController(IConfiguration config, UsuarioRepository usuarioRepository) : ControllerBase
    {
        private readonly IDbConnection _db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        private readonly UsuarioRepository _usuarioRepository = usuarioRepository;

        [HttpGet]
        public async Task<IEnumerable<Veiculo>> ListarTodosDoUsuario()
        {
            var sql = @"SELECT id AS Id, usuario_id AS UsuarioId, modelo, placa, tipo, tipo_conector AS Tipo_Conector, capacidade_bateria AS Capacidade_Bateria, cor, criado_em AS CriadoEm
                FROM veiculos";
            var veiculos = await _db.QueryAsync<Veiculo>(sql);
            return veiculos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> BuscarPorId(int id)
        {
            var sql = @"SELECT id, usuario_id AS UsuarioId, modelo, placa, tipo, tipo_conector AS Tipo_Conector, capacidade_bateria AS Capacidade_Bateria, cor, criado_em AS CriadoEm
                FROM veiculos WHERE id = @id";
            var veiculo = await _db.QueryFirstOrDefaultAsync<Veiculo>(sql, new { id });
            if (veiculo == null) return NotFound();
            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<ActionResult> Criar(Veiculo veiculo)
        {
            var sql = @"INSERT INTO veiculos (usuario_id, modelo, placa, tipo, tipo_conector, capacidade_bateria, criado_em, cor)
                VALUES (@UsuarioId, @Modelo, @Placa, @Tipo, @Tipo_Conector, @Capacidade_Bateria, @CriadoEm, @Cor) RETURNING id";
            var id = await _db.ExecuteScalarAsync<int>(sql, veiculo);
            return CreatedAtAction(nameof(BuscarPorId), new { id }, veiculo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(int id, Veiculo veiculo)
        {
            veiculo.Id = id;
            var sql = @"UPDATE veiculos SET
                modelo = @Modelo,
                placa = @Placa,
                tipo = @Tipo,
                tipo_conector = @Tipo_Conector,
                capacidade_bateria = @Capacidade_Bateria,
                cor = @Cor
                WHERE id = @Id";
            var result = await _db.ExecuteAsync(sql, veiculo);
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id)
        {
            var sql = "DELETE FROM veiculos WHERE id = @id";
            var result = await _db.ExecuteAsync(sql, new { id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
