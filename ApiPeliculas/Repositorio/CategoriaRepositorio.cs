using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly AplicationDBContext _db;

        public CategoriaRepositorio(AplicationDBContext db)
        {
            _db = db;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            //categoria.FechaCreacion = DateTime.UtcNow;
            //_db.Categorias.Update(categoria);
            //return Guardar();

            categoria.FechaCreacion = DateTime.UtcNow;

            //Arreglar problema con PUT
            var categoriaExistente = _db.Categorias.Find(categoria.Id);
            if (categoriaExistente != null)
            {
                _db.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _db.Categorias.Update(categoria);
            }
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _db.Categorias.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.UtcNow;
            _db.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(int id)
        {
            return _db.Categorias.Any(c => c.Id == id);
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _db.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _db.Categorias.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _db.Categorias.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
