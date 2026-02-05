using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly IPeliculaRepositorio _pelRepo;
        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _mapper = mapper;
            _pelRepo = pelRepo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var list in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(list));
            }
            return Ok(listaPeliculasDto);
        }
    }
}
