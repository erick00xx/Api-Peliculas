using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AplicationDBContext _db;
        private string claveSecreta;

        public UsuarioRepositorio(AplicationDBContext db, IConfiguration config)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _db.Usuario.FirstOrDefault(u => u.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _db.Usuario.OrderBy(c => c.NombreUsuario).ToList();
        }

        // regresar true si no existe usuario
        public bool IsUniqueUser(string nombre)
        {
            var usuarioExistente = _db.Usuario.FirstOrDefault(u => u.NombreUsuario.ToLower() == nombre.ToLower());
            if (usuarioExistente != null)
            {
                return false;
            }
            return true;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);
            var usuario = _db.Usuario.FirstOrDefault(
                u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                && u.Password == passwordEncriptado
                );
            //Validamos si el usuario no existe con la combinacion de usuario y contraseña correcta
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }
            // Aqui existe el usuario entonces podemos provesar el login
            var manejadoToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadoToken.CreateToken(tokenDescriptor);
            return new UsuarioLoginRespuestaDto()
            {   
                Token = manejadoToken.WriteToken(token),
                Usuario = usuario
            };
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDto.Nombre,
                Role = usuarioRegistroDto.Role
            };

            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();
            usuario.Password = passwordEncriptado;
            return usuario;
        }

        /* Metodo MD5 para registro y acceso */
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}