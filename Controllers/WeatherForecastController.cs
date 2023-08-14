using Microsoft.AspNetCore.Mvc;

namespace RpgApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
 [HttpPost("Registrar")]
    public async Task<IActionResult> RegistrarUsuario(Usuario user)
    {
        try{
            if (await UsuarioExixstente(user.Username))
            throw new System.Exception("Nome de úsuario já existe");

            Criptografia.CriarPasswordHasj(user.PasswordString, out byte [] hash, out byte[] salt);
            user.PasswordString = string.Empty;
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            await _context.Usuarios.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }
        catch(System.Exception ex)
        {
            return BadRequest(ex.Message);
            
        }
    }
    [HttpPost("Autenticar")]
    public async Task<IActionResult> AutenticarUsuario(Usuario credenciais)
    {
        try{
            Usuario usuario = await _context.Usuarios.
            .FirstOrDefaultAsync(x => x.Username.ToLower().Equals(credenciais.Username.ToLower()));

            if (usuario == null)
            {
                throw new System.Exception("Úsuario não encontrado.");
            }
            else if (!Criptografia
            .VerificarPasswordHash(credenciais.PasswordString, usuario.PasswordHash, usuario.PasswordSalt))
            {
                throw new System.Exception("Senha incorreta.");
            }
            else{
                return Ok(usuario);
            }
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message)
        }
    }
