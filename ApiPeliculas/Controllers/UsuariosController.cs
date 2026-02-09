using System.Net;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    // [ApiVersion("1.0")]
    [ApiVersionNeutral] //CONTROLADOR QUE NUNCA CAMBIARA Y ESTARA EN TODAS LAS VERSIONES
    public class UsuariosController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usRepo;
        protected RespuestaApi _respuestaApi;
        public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _mapper = mapper;
            _usRepo = usRepo;
            this._respuestaApi = new();
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usRepo.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var list in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(list));
            }
            return Ok(listaUsuariosDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _usRepo.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }
            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }


        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario a existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en le registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = System.Net.HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {

            UsuarioLoginRespuestaDto respuestaLogin = await _usRepo.Login(usuarioLoginDto);


            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);

        }








        // [HttpPost]
        // [ProducesResponseType(201, Type = typeof(UsuarioDto))]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public IActionResult CrearUsuario([FromBody] CrearUsuarioDto crearUsuarioDto)
        // {
        //     if(!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     if(crearUsuarioDto == null)
        //     {
        //         return BadRequest(crearUsuarioDto);
        //     }

        //     if(_usRepo.ExisteUsuario(crearUsuarioDto.Nombre))
        //     {
        //         ModelState.AddModelError("", "La categoría ya existe");
        //         return StatusCode(404, ModelState);
        //     }

        //     var usuario = _mapper.Map<Usuario>(crearUsuarioDto);  
        //     if (!_usRepo.CrearUsuario(usuario))
        //     {
        //         ModelState.AddModelError("", $"Algo salió mal guardando el registro {usuario.Nombre}");
        //         return StatusCode(500, ModelState);
        //     }
        //     return CreatedAtRoute("GetUsuario", new {usuarioId = usuario.Id }, usuario);
        // }

        // [HttpPatch("{usuarioId:int}", Name = "ActualizarPatchUsuario")]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // public IActionResult ActualizarPatchUsuario(int usuarioId, [FromBody] UsuarioDto usuarioDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     if (usuarioDto == null || usuarioId != usuarioDto.Id)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var usuarioExistente = _usRepo.GetUsuario(usuarioId);
        //     if (usuarioExistente == null)
        //     {
        //         return NotFound($"No se encontro la usuario con iD {usuarioId}");
        //     }

        //     var usuario = _mapper.Map<Usuario>(usuarioDto);
        //     if (!_usRepo.ActualizarUsuario(usuario))
        //     {
        //         ModelState.AddModelError("", $"Algo salió mal actualizando el registro {usuario.Nombre}");
        //         return StatusCode(500, ModelState);
        //     }
        //     return NoContent();
        // }

        // [HttpDelete("{usuarioId:int}", Name = "BorrarUsuario")]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public IActionResult BorrarUsuario(int usuarioId)
        // {

        //     if (!_usRepo.ExisteUsuario(usuarioId))
        //     {
        //         return NotFound();
        //     }

        //     var usuario = _usRepo.GetUsuario(usuarioId);

        //     if (!_usRepo.BorrarUsuario(usuario))
        //     {
        //         ModelState.AddModelError("", $"Algo salió mal borrando el registro {usuario.Nombre}");
        //         return StatusCode(500, ModelState);
        //     }
        //     return NoContent();
        // }

        // [HttpGet("GetUsuariosEnCategoria/{categoriaId:int}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public IActionResult GetUsuariosEnCategoria(int categoriaId)
        // {
        //     var listaUsuarios = _usRepo.GetUsuariosEnCategoria(categoriaId);

        //     if (listaUsuarios == null || listaUsuarios.Count == 0)
        //     {
        //         return NotFound($"No se encontraron usuarios en la categoria con ID {categoriaId}");
        //     }

        //     var itemUsuario = new List<UsuarioDto>();

        //     foreach (var list in listaUsuarios)
        //     {
        //         itemUsuario.Add(_mapper.Map<UsuarioDto>(list));
        //     }
        //     return Ok(itemUsuario);
        // }

        // [HttpGet("Buscar")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public IActionResult Buscar(string nombre)
        // {
        //     try
        //     {
        //         var resultado = _usRepo.BuscarUsuario(nombre);
        //         if (resultado.Any())
        //         {
        //             return Ok(resultado);
        //         }
        //         return NotFound($"No se encontraron usuarios que coincidan con el nombre '{nombre}'");
        //     }catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Error al buscar usuarios: {ex.Message}");
        //     }
        // }


    }
}
