using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Models;
using BibliotecaDigitalApi.Dtos;
using System.Security.Claims;

namespace BibliotecaDigitalApi.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ReportesController(AppDbContext db) => _db = db;

        
        [HttpPost("crear")]
        [Authorize]
        public async Task<IActionResult> CrearReporte([FromForm] ReporteCreateRequest req)
        {
            try
            {
                
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userName = User.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized("No se pudo identificar al usuario logueado.");

                
                var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.email == userEmail);
                if (usuario is null)
                    return Unauthorized("El usuario no existe en la base de datos.");

                string? urlImagen = null;

                
                if (req.ImagenPortada != null && req.ImagenPortada.Length > 0)
                {
                    var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reportes");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(req.ImagenPortada.FileName)}";
                    var ruta = Path.Combine(carpeta, nombreArchivo);

                    using var stream = new FileStream(ruta, FileMode.Create);
                    await req.ImagenPortada.CopyToAsync(stream);

                    
                    urlImagen = $"http://127.0.0.1:5000/reportes/{nombreArchivo}";
                }

                
                var reporte = new Reporte
                {
                    Fecha = DateTime.Now,
                    TituloLibro = req.TituloLibro,
                    Sinopsis = req.Sinopsis,
                    ImagenPortada = urlImagen,
                    UsuarioId = usuario.id,            
                    UsuarioNombre = usuario.nombre,    
                    UsuarioEmail = usuario.email       
                };

                _db.Reportes.Add(reporte);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = reporte.Id }, reporte);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el reporte", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (rol == "Admin")
            {
                var todos = await _db.Reportes
                    .OrderByDescending(r => r.Fecha)
                    .ToListAsync();
                return Ok(todos);
            }

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            
            var propios = await _db.Reportes
                .Where(r => r.UsuarioEmail == userEmail)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            return Ok(propios);
        }

        
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var reporte = await _db.Reportes.FindAsync(id);
            if (reporte is null)
                return NotFound();

            var rol = User.FindFirstValue(ClaimTypes.Role);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            
            if (rol != "Admin" && userEmail != reporte.UsuarioEmail)
                return Forbid("No tienes permiso para ver este reporte.");

            return Ok(reporte);
        }
    }
}

