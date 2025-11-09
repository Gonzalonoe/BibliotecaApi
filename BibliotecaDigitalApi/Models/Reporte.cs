using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaDigitalApi.Models
{
    public class Reporte
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required, StringLength(100)]
        public string TituloLibro { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Sinopsis { get; set; }

        public string? ImagenPortada { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required, StringLength(100)]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string UsuarioEmail { get; set; } = string.Empty;
    }
}