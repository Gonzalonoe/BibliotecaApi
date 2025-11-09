namespace BibliotecaDigitalApi.Models
{
    public class Libro
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Autor { get; set; } = string.Empty;

        public int? Anio { get; set; }

        public int Stock { get; set; } = 0;

        public string? Portada { get; set; }
        public string? Descripcion { get; set; }
    }
}
