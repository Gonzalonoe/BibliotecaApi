using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Models;
using BibliotecaDigitalApi.Dtos;

namespace BibliotecaDigitalApi.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PedidosController(AppDbContext db) => _db = db;

        private int GetUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(claimValue, out int userId))
                return userId;

            var usuario = _db.Usuarios.FirstOrDefault(u => u.email == claimValue);
            if (usuario != null)
                return usuario.id;

            throw new Exception("No se pudo determinar el usuario autenticado.");
        }

        
        [HttpPost("crear")]
        [Authorize(Roles = "Lector,Admin")]
        public async Task<IActionResult> Crear([FromBody] PedidoCreateRequest req)
        {
            try
            {
                var userId = GetUserId();
                var libro = await _db.Libros.FindAsync(req.LibroId);

                if (libro == null)
                    return NotFound("📚 Libro no encontrado.");

                if (libro.Stock <= 0)
                    return BadRequest("❌ No hay stock disponible para este libro.");

                var pedido = new Pedido
                {
                    UsuarioId = userId,
                    LibroId = libro.Id,
                    TituloSolicitado = libro.Titulo,
                    Estado = EstadoPedido.Pendiente,
                    FechaPedido = DateTime.UtcNow,
                    FechaVencimiento = DateTime.UtcNow.AddDays(7),
                    Observaciones = "Pendiente de entrega"
                };

                
                libro.Stock -= 1;

                _db.Pedidos.Add(pedido);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(MisPedidos), new { id = pedido.Id }, pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el pedido", error = ex.Message });
            }
        }

        
        [HttpGet("mios")]
        [Authorize(Roles = "Lector,Admin")]
        public async Task<IActionResult> MisPedidos()
        {
            try
            {
                var userId = GetUserId();

                var pedidos = await _db.Pedidos
                    .Include(p => p.Libro)
                    .Include(p => p.Usuario)
                    .Where(p => p.UsuarioId == userId)
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener pedidos", error = ex.Message });
            }
        }

        
        [HttpPut("{id:int}/cancelar")]
        [Authorize(Roles = "Lector,Admin")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var userId = GetUserId();

                var pedido = await _db.Pedidos
                    .Include(p => p.Libro)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == userId);

                if (pedido == null)
                    return NotFound("Pedido no encontrado o no pertenece al usuario.");

                if (pedido.Estado != EstadoPedido.Pendiente)
                    return BadRequest("Solo se pueden cancelar pedidos pendientes.");

                pedido.Estado = EstadoPedido.Cancelado;
                pedido.FechaDevolucion = DateTime.UtcNow;
                pedido.Observaciones = "Cancelado por el usuario.";

                if (pedido.Libro != null)
                    pedido.Libro.Stock += 1; 

                await _db.SaveChangesAsync();
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al cancelar pedido", error = ex.Message });
            }
        }

        
        [HttpPut("{id:int}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int nuevoEstado)
        {
            try
            {
                var pedido = await _db.Pedidos
                    .Include(p => p.Libro)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                    return NotFound("Pedido no encontrado.");

                var estadoAnterior = pedido.Estado;
                pedido.Estado = (EstadoPedido)nuevoEstado;

                
                if (estadoAnterior != EstadoPedido.Cancelado && estadoAnterior != EstadoPedido.Devuelto)
                {
                    if (nuevoEstado == (int)EstadoPedido.Prestado)
                    {
                    
                    }
                    else if (nuevoEstado == (int)EstadoPedido.Devuelto || nuevoEstado == (int)EstadoPedido.Cancelado)
                    {
                        pedido.Libro.Stock += 1; 
                    }
                }

                await _db.SaveChangesAsync();
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al cambiar el estado del pedido", error = ex.Message });
            }
        }

        
        [HttpPut("{id:int}/devolver")]
        [Authorize(Roles = "Lector,Admin")]
        public async Task<IActionResult> Devolver(int id)
        {
            try
            {
                var pedido = await _db.Pedidos
                    .Include(p => p.Libro)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                    return NotFound("Pedido no encontrado.");

                if (pedido.Estado != EstadoPedido.Prestado)
                    return BadRequest("Solo se pueden devolver pedidos que estén en estado Prestado.");

                pedido.Estado = EstadoPedido.Devuelto;
                pedido.FechaDevolucion = DateTime.UtcNow;
                pedido.Observaciones = "Libro devuelto correctamente.";

                if (pedido.Libro != null)
                    pedido.Libro.Stock += 1; 

                await _db.SaveChangesAsync();
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al devolver el pedido", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Todos()
        {
            var pedidos = await _db.Pedidos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return Ok(pedidos);
        }

        
        [HttpPost("actualizar-vencidos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarVencidos()
        {
            var ahora = DateTime.UtcNow;

            var vencidos = await _db.Pedidos
                .Include(p => p.Libro)
                .Where(p => p.Estado == EstadoPedido.Pendiente && p.FechaVencimiento < ahora)
                .ToListAsync();

            foreach (var p in vencidos)
            {
                p.Estado = EstadoPedido.Cancelado;
                p.Observaciones = "Cancelado automáticamente por vencimiento.";

                if (p.Libro != null)
                    p.Libro.Stock += 1; 
            }

            await _db.SaveChangesAsync();
            return Ok(new { mensaje = $"{vencidos.Count} pedidos vencidos fueron cancelados y stock actualizado." });
        }
    }
}
