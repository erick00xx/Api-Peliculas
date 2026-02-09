using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{
    // [Authorize(Roles = "Admin")]
    // [ResponseCache(Duration = 20)] //PARA EL CACHE
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[EnableCors("PoliticaCors")]
    [ApiVersion("2.0")] 
    public class CategoriasController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ICategoriaRepositorio _catRepo;
        public CategoriasController(ICategoriaRepositorio catRepo, IMapper mapper)
        {
            _mapper = mapper;
            _catRepo = catRepo;
        }

        [HttpGet]
        // [MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2", "valor3"};
        }




        
    }
}
