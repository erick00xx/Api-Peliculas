using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("PoliticaCors")]
    public class CategoriasController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ICategoriaRepositorio _catRepo;
        public CategoriasController(ICategoriaRepositorio catRepo, IMapper mapper)
        {
            _mapper = mapper;
            _catRepo = catRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("PoliticaCors")] // Aplica la politica ros a este metodo
        public IActionResult GetCategorias()
        {
            var listaCategorias = _catRepo.GetCategorias();

            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var list in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(list));
            }
            return Ok(listaCategoriasDto);
        }

        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _catRepo.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }
            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);
            return Ok(itemCategoriaDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (crearCategoriaDto == null)
            {
                return BadRequest(crearCategoriaDto);
            }

            if (_catRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);
            if (!_catRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaExistente = _catRepo.GetCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con iD {categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if (!_catRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int categoriaId)
        {

            if (!_catRepo.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _catRepo.GetCategoria(categoriaId);

            if (!_catRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
