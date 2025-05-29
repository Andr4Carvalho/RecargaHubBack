using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using RecargaHubBack.Models;
using RecargaHubBack.Repositories;
using System.Security.Claims;

namespace RecargaHubBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvaliacoesController(AvaliacaoRepository repo) : ControllerBase
    {
        private readonly AvaliacaoRepository _repo = repo;

        [HttpGet("{pontoRecargaId}")]
        public async Task<IActionResult> Listar(int pontoRecargaId)
        {
            var avaliacoes = await _repo.ListarPorPontoAsync(pontoRecargaId);
            return Ok(avaliacoes);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Avaliacao avaliacao)
        {
            if (avaliacao.Nota < 1 || avaliacao.Nota > 5)
                return BadRequest("Nota inválida.");

            var id = await _repo.CriarAsync(avaliacao);
            return CreatedAtAction(nameof(Listar), new { pontoRecargaId = avaliacao.PontoRecargaId }, new { id });
        }
    }

}
