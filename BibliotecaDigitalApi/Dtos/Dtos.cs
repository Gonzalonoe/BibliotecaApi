using System;
using Microsoft.AspNetCore.Http;

namespace BibliotecaDigitalApi.Dtos
{
    public record LoginRequest(string Email, string Password);
    public record LoginResponse(string Token, int UserId, string Nombre, string Email, string Rol);

    public record BootstrapAdminRequest(string Nombre, string Email, string Password);
    public record CreateLectorRequest(string Nombre, string Email, string Password);

    public record LibroCreateRequest(string Titulo, string Autor, int? Anio, int Stock, string? Descripcion);
    public record LibroUpdateRequest(string Titulo, string Autor, int? Anio, int Stock, string? Descripcion);

    public record PedidoCreateRequest(int? LibroId, string? TituloSolicitado);
    public record AprobarPedidoRequest(DateTime FechaVencimiento);
    public record MarcarPrestadoRequest(string? Observaciones);
    public record MarcarDevolucionRequest(string? Observaciones);

    public record ReporteCreateRequest(
        string TituloLibro,
        string? Sinopsis,
        IFormFile? ImagenPortada
    );
}
