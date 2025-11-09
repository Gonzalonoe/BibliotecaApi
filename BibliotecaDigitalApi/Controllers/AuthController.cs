using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaDigitalApi.Services;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Models;
using BibliotecaDigitalApi.Dtos;

namespace BibliotecaDigitalApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public AuthController(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        
        [HttpPost("bootstrap-admin")]
        public async Task<IActionResult> BootstrapAdmin([FromBody] BootstrapAdminRequest req)
        {
            var anyAdmin = await _db.Usuarios.AnyAsync(u => u.rol == RolUsuario.Admin);
            if (anyAdmin) return BadRequest("Ya existe un administrador.");

            _auth.HashPassword(req.Password, out var hash, out var salt);
            var admin = new Usuario
            {
                nombre = req.Nombre,
                email = req.Email,
                rol = RolUsuario.Admin,
                passwordHash = hash,
                passwordSalt = salt
            };
            _db.Usuarios.Add(admin);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Administrador creado", admin.id, admin.email });
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _auth.FindByEmail(req.Email);
            if (user is null) return Unauthorized("Credenciales inválidas");

            if (!_auth.VerifyPassword(req.Password, user.passwordHash, user.passwordSalt))
                return Unauthorized("Credenciales inválidas");

            var token = _auth.GenerateJwtToken(user);
            return Ok(new LoginResponse(token, user.id, user.nombre, user.email, user.rol.ToString()));
        }
    }
}
