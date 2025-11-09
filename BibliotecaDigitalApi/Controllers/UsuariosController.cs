using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Dtos;
using BibliotecaDigitalApi.Models;
using BibliotecaDigitalApi.Services;
using System.Security.Claims;

namespace BibliotecaDigitalApi.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public UsuariosController(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        
        [HttpPost("crear-lector")]
        public async Task<IActionResult> CrearLector([FromBody] CreateLectorRequest req)
        {
            if (await _db.Usuarios.AnyAsync(u => u.email == req.Email))
                return BadRequest("Email ya registrado");

            _auth.HashPassword(req.Password, out var hash, out var salt);
            var lector = new Usuario
            {
                nombre = req.Nombre,
                email = req.Email,
                rol = RolUsuario.Lector,
                passwordHash = hash,
                passwordSalt = salt
            };
            _db.Usuarios.Add(lector);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Lector creado", lector.id, lector.email });
        }
        
        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(idClaim) && string.IsNullOrEmpty(email))
                return Unauthorized("No se pudo obtener la información del usuario desde el token.");

            Usuario? usuario = null;

            
            if (int.TryParse(idClaim, out int id))
                usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.id == id);

            
            if (usuario == null && !string.IsNullOrEmpty(email))
                usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.email == email);

            if (usuario == null)
                return NotFound("Usuario no encontrado en la base de datos.");

            
            return Ok(new
            {
                usuario.id,
                usuario.nombre,
                usuario.email,
                rol = usuario.rol.ToString()
            });
        }
    }
}