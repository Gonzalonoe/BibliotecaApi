using System;

namespace BibliotecaDigitalApi.Models
{
    public enum EstadoPedido
    {
        Pendiente = 0,   
        Aprobado = 1,    
        Prestado = 2,    
        Devuelto = 3,   
        Vencido = 4,     
        Cancelado = 5,   
        Desconocido = 6    
    }

    public class Pedido
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int? LibroId { get; set; }
        public Libro? Libro { get; set; }

        public string? TituloSolicitado { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        public string? Observaciones { get; set; }
    }
}
