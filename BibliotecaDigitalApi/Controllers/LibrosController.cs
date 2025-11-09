using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Models;
using BibliotecaDigitalApi.Dtos;

namespace BibliotecaDigitalApi.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LibrosController(AppDbContext db) => _db = db;

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Libros.OrderBy(l => l.Titulo).ToListAsync());

        
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _db.Libros.FindAsync(id);
            return libro is null ? NotFound() : Ok(libro);
        }

        
        [HttpGet("buscar")]
[Authorize]
public async Task<IActionResult> BuscarLibros(
    [FromQuery] string? titulo,
    [FromQuery] string? autor,
    [FromQuery] int? anio,
    [FromQuery] int? stock,
    [FromQuery] string? descripcion,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    if (page <= 0) page = 1;
    if (pageSize <= 0 || pageSize > 100) pageSize = 10;

    var query = _db.Libros.AsQueryable();

    
    if (!string.IsNullOrWhiteSpace(titulo))
        query = query.Where(l => l.Titulo.ToLower().Contains(titulo.ToLower()));

    if (!string.IsNullOrWhiteSpace(autor))
        query = query.Where(l => l.Autor.ToLower().Contains(autor.ToLower()));

    if (anio.HasValue)
        query = query.Where(l => l.Anio == anio);

    if (stock.HasValue)
        query = query.Where(l => l.Stock == stock);

    if (!string.IsNullOrWhiteSpace(descripcion))
        query = query.Where(l =>
            l.Descripcion != null && l.Descripcion.ToLower().Contains(descripcion.ToLower()));

    
    var totalRegistros = await query.CountAsync();
    var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

    
    var resultados = await query
        .OrderBy(l => l.Titulo)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    
    var response = new
    {
        page,
        pageSize,
        totalRegistros,
        totalPaginas,
        resultados
    };

    return Ok(response);
}

        
        [HttpPost("crear")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateConPortada(
            [FromForm] string titulo,
            [FromForm] string autor,
            [FromForm] int? anio,
            [FromForm] int stock,
            [FromForm] string? descripcion,
            [FromForm] IFormFile? portada)
        {
            string? nombreArchivo = null;
            string? urlPortada = null;

            if (portada != null && portada.Length > 0)
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "portadas");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(portada.FileName)}";
                var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaArchivo, FileMode.Create);
                await portada.CopyToAsync(stream);

                
                urlPortada = $"http://127.0.0.1:5000/portadas/{nombreArchivo}";
                
            }

            var libro = new Libro
            {
                Titulo = titulo,
                Autor = autor,
                Anio = anio,
                Stock = stock,
                Descripcion = descripcion,
                Portada = urlPortada
            };

            _db.Libros.Add(libro);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = libro.Id }, libro);
        }

        
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] LibroUpdateRequest req)
        {
            var libro = await _db.Libros.FindAsync(id);
            if (libro is null)
                return NotFound();

            libro.Titulo = req.Titulo;
            libro.Autor = req.Autor;
            libro.Anio = req.Anio;
            libro.Stock = req.Stock;
            libro.Descripcion = req.Descripcion; 

            await _db.SaveChangesAsync();

            return Ok(libro);
        }

        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _db.Libros.FindAsync(id);
            if (libro is null)
                return NotFound();

            _db.Libros.Remove(libro);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
