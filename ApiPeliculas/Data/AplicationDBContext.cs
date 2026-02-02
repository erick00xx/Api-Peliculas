using ApiPeliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    public class AplicationDBContext : DbContext
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options)
            : base(options)
        {
            
        }

        //Aqui van pasadas todas las entidades (Modelos)
        public DbSet<Categoria> Categorias { get; set; }

    }
}
