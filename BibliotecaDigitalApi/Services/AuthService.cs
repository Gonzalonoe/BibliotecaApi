using System.Security.Cryptography;
using System.Text;
using BibliotecaDigitalApi.Data;
using BibliotecaDigitalApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BibliotecaDigitalApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<Usuario?> FindByEmail(string email) =>
            await _db.Usuarios.FirstOrDefaultAsync(u => u.email == email);

        public void HashPassword(string password, out string hashB64, out string saltB64)
        {
            // Per-user random salt + app pepper from appsettings["Salt"]
            var pepper = _config["Salt"] ?? "";
            var salt = RandomNumberGenerator.GetBytes(16);
            var combined = Encoding.UTF8.GetBytes(pepper);
            // PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(password + pepper, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            hashB64 = Convert.ToBase64String(hash);
            saltB64 = Convert.ToBase64String(salt);
        }

        public bool VerifyPassword(string password, string storedHashB64, string storedSaltB64)
        {
            var pepper = _config["Salt"] ?? "";
            var salt = Convert.FromBase64String(storedSaltB64);
            using var pbkdf2 = new Rfc2898DeriveBytes(password + pepper, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash) == storedHashB64;
        }

        public string GenerateJwtToken(Usuario u)
        {
            var jwtSection = _config.GetSection("TokenJwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, u.email),
                new Claim(ClaimTypes.NameIdentifier, u.id.ToString()),
                new Claim(ClaimTypes.Name, u.nombre),
                new Claim(ClaimTypes.Email, u.email),
                new Claim(ClaimTypes.Role, u.rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
