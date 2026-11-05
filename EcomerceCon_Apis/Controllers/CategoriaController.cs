using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Categoria;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Usuarios;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
        [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ProductosDbcontext _context;

        public CategoriaController(ProductosDbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("Listar")]
       // [Authorize]
        public async Task<IActionResult> Listar()
        {
            var categorias = await _context.Categorias.ToListAsync();
            if (categorias == null || categorias.Count == 0)
            {
                return NotFound(new { message = "No se encontraron categorias" });
            }
            else
            {

                return Ok(categorias);
            }
        }

        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearCategoriaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Create([FromBody]CrearCategoriaDto catdto)
        {
           
            if (await _context.Categorias.Where(c=>c.Nombre==catdto.Nombre).FirstOrDefaultAsync()==null)
            {
                _context.Categorias.Add(new Categoria
                {
                    Nombre = catdto.Nombre,
                });

                await _context.SaveChangesAsync();
            return Ok(new { message = "Categoria creada correctamente" });
           
            }
            else
            {
                return BadRequest(new { message = "Ya existe esta categoria" });
            }

        }
            [HttpGet]
            [Route("Obtener/{Id:int}")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarCategoriaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Obtener(int id)
        {
            var obtener = await _context.Categorias.Where(c=>c.Id==id).FirstOrDefaultAsync();
            var dto = new EditarCategoriaDto
            {
                Id = obtener.Id,
                Nombre = obtener.Nombre
            };
            if (dto == null)
            {
                return Ok(new RespuestaGenericaDto<string>
                {
                    IsSuccess = false,
                    Mensaje = "No se econtro el producto "
                    
                });
            }
            return Ok(new RespuestaGenericaDto<EditarCategoriaDto>
            {
                IsSuccess = true,
                Mensaje = "producto obtenido",
                Data = dto
            });


        }
        [HttpPut]
        [Route("Editar/{Id:int}")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarCategoriaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Editar(int id, [FromBody] EditarCategoriaDto dto)
        {
            var comprobar = await _context.Categorias.Where(_ => _.Id == id).FirstOrDefaultAsync();
            if (comprobar == null)
            {
                return BadRequest(new RespuestaGenericaDto<bool>
                {
                    IsSuccess = false,
                    Mensaje = "Error ,intente de nuevo",
                });

            }
            _context.Update(new Categoria
            {
                Id = id,
                Nombre = dto.Nombre,
            });
            await _context.SaveChangesAsync();
            return Ok(new RespuestaGenericaDto<bool>
            {
                IsSuccess = true,
                Mensaje = "Categoria editada correctamente"
            }
                );

           

        }


    }
}
