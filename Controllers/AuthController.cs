using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecargaHubBack.Repositories;
using RecargaHubBack.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecargaHubBack.Controllers
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

            var usuario = await _usuarioRepository.GetUsuarioByEmailAndSenhaAsync(loginRequest.Email, loginRequest.Senha);
            if (usuario == null)
            {
                return Unauthorized("E-mail ou senha inválidos");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Key.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("email", loginRequest.Email),
                    new Claim("userId", usuario.Id.ToString())
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