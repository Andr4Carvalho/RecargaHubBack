using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using RecargaHubBack.Models;
using System.Security.Claims;

namespace RecargaHubBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservaController(IConfiguration config) : ControllerBase
    {
        private readonly IDbConnection _db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));

        [HttpGet]
        public async Task<IEnumerable<dynamic>> Listar()
        {
            var sql = @"
                SELECT
                    r.id,
                    r.usuario_id,
                    r.data_reserva,
                    r.horario_inicio,
                    r.horario_fim,
                    r.criado_em,
                    v.id AS veiculo_id,
                    v.modelo AS veiculo_nome,
                    v.placa AS veiculo_placa,
                    p.id AS ponto_recarga_id,
                    p.nome AS ponto_recarga_nome,
                    p.endereco AS ponto_recarga_endereco,
                    EXISTS (
                      SELECT 1 FROM avaliacoes_ponto_recarga a
                      WHERE a.reserva_id = r.id
                    ) AS avaliada
                FROM reservas r
                INNER JOIN veiculos v ON v.id = r.veiculo_id
                INNER JOIN pontos_recarga p ON p.id = r.ponto_recarga_id
            ";

            return await _db.QueryAsync<dynamic>(sql);
        }

        [HttpPost]
        public async Task<ActionResult> Criar(Reserva reserva)
        {
            var sql = @"
                INSERT INTO reservas (usuario_id, veiculo_id, ponto_recarga_id, data_reserva, horario_inicio, horario_fim, criado_em)
                VALUES (@UsuarioId, @VeiculoId, @PontoRecargaId, @DataReserva, @HorarioInicio, @HorarioFim, NOW())
                RETURNING id;
            ";

            try
            {
                var id = await _db.ExecuteScalarAsync<int>(sql, reserva);
                return CreatedAtAction(nameof(ObterPorId), new { id }, reserva);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return Conflict("Horário já reservado para esse ponto de recarga.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Reserva reserva)
        {
            // Verifica se existe conflito de horário com outra reserva (ignora a atual)
            var conflito = await _db.QueryFirstOrDefaultAsync<Reserva>(
                @"SELECT * FROM reservas 
                  WHERE ponto_recarga_id = @PontoRecargaId 
                    AND data_reserva = @DataReserva
                    AND id <> @Id
                    AND (
                        (@HorarioInicio BETWEEN horario_inicio AND horario_fim - interval '1 minute')
                        OR (@HorarioFim BETWEEN horario_inicio + interval '1 minute' AND horario_fim)
                        OR (horario_inicio BETWEEN @HorarioInicio AND @HorarioFim - interval '1 minute')
                    )",
                        new
                        {
                            reserva.PontoRecargaId,
                            reserva.DataReserva,
                            reserva.HorarioInicio,
                            reserva.HorarioFim,
                            Id = id
                        });

            if (conflito != null)
                return Conflict("Já existe uma reserva nesse horário para este ponto de recarga.");

            // Atualiza a reserva
            var rows = await _db.ExecuteAsync(
                @"UPDATE reservas
                  SET ponto_recarga_id = @PontoRecargaId,
                      veiculo_id = @VeiculoId,
                      data_reserva = @DataReserva,
                      horario_inicio = @HorarioInicio,
                      horario_fim = @HorarioFim,
                      usuario_id = @UsuarioId
                  WHERE id = @Id",
                new
                {
                    reserva.PontoRecargaId,
                    reserva.VeiculoId,
                    reserva.DataReserva,
                    reserva.HorarioInicio,
                    reserva.HorarioFim,
                    reserva.UsuarioId,
                    Id = id
                });

            return rows > 0 ? NoContent() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> ObterPorId(int id)
        {
            var sql = @"
                SELECT 
                    id, 
                    usuario_id, 
                    veiculo_id, 
                    ponto_recarga_id, 
                    data_reserva, 
                    horario_inicio AS HorarioInicio, 
                    horario_fim AS HorarioFim, 
                    criado_em 
                FROM reservas 
                WHERE id = @id";

            var reserva = await _db.QueryFirstOrDefaultAsync<Reserva>(sql, new { id });

            return reserva == null ? NotFound() : Ok(reserva);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id)
        {
            var sql = "DELETE FROM reservas WHERE id = @id";
            var result = await _db.ExecuteAsync(sql, new { id });
            return result == 0 ? NotFound() : NoContent();
        }
    }
}
