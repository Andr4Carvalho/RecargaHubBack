using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using saas;
using SaaSBack.Repositories;
using SaaSBack.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaaSBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(UsuarioRepository usuarioRepository) : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository = usuarioRepository;

        [HttpPost("login")]
        public async Task<IActionResult> GenerateTokenRestaurante([FromBody] Requests.LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) ||
                string.IsNullOrWhiteSpace(loginRequest.Senha))
            {
                return BadRequest("Dados inválidos");
            }

            // Verifica se o e-mail e senha correspondem aos registros no banco
            var usuario = await _usuarioRepository.GetUsuarioByEmailAndSenhaAsync(loginRequest.Email, loginRequest.Senha);
            if (usuario == null)
            {
                return Unauthorized("E-mail ou senha inválidos");
            }

            // Gera o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Key.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("email", loginRequest.Email)
                ]),
                Expires = DateTimeHelper.Now().AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Senha) ||
                string.IsNullOrWhiteSpace(request.Nome))
            {
                return BadRequest("Dados inválidos.");
            }

            var usuarioExistente = await _usuarioRepository.GetUsuarioByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                return BadRequest("O e-mail informado já está em uso.");
            }

            var sucesso = await _usuarioRepository.CadastrarUsuarioAsync(request.Email, request.Senha, request.Nome);

            if (!sucesso)
            {
                return StatusCode(500, "Erro ao cadastrar o usuário.");
            }

            // Gera o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Key.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("email", request.Email)
                ]),
                Expires = DateTimeHelper.Now().AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
            });
        }
    }
}