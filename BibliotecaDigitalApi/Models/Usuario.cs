namespace BibliotecaDigitalApi.Models
{
    public enum RolUsuario
    {
        Admin = 1,
        Lector = 2
    }

    public class Usuario
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public RolUsuario rol { get; set; }
        public string passwordHash { get; set; } = string.Empty;
        public string passwordSalt { get; set; } = string.Empty; 
    }
}
