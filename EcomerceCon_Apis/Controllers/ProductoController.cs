using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        private readonly IProductosService _producto;
        public ProductoController(ProductosDbcontext context,IProductosService ser)
        {

            _context = context;
            _producto = ser;
        }
        [HttpGet]
        [Route("ListaParaAdmins")]
        [Authorize(Roles ="Admin")]
   
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<ProductoDtoParaAminds>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> ListaDeAdmins()
        {


            

                var producto = await _producto.ListaDeAdmins();
               

                    return Ok(producto);
 
        }


        [HttpGet]
        [Route("Lista")]
       
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<ProductoDto>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> listar()
        {

    
                var producto = await _producto.lista();
             
                    return Ok(producto);
   

        }
        [HttpGet]
        [Route("ListarParaOferta")]
        public async Task<IActionResult> ListaNorma()
        {
            var producto = await _producto.ListarParaOFerta();        
                return Ok(producto);
         
            
        }

        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Crear([FromBody] CrearProductoDto productoDto)
        {
  
                await _producto.Create(productoDto);
                return Ok();  
        }
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Obtener(int Id)
        {
            
                var pro = await _producto.Obtener(Id);

                var producto = new RespuestaGenericaDto<EditarProductoDto>
                {
                    IsSuccess=true,
                    Mensaje="producto obetenido",
                    Data=pro
                    
                };

                if (producto != null)
                {
                    return Ok(producto);
                }
                else
                {
                    return BadRequest(new {message="se desconoce el producto"});
                }



        }
        [HttpPut]
        [Route("Editar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Editar([FromBody] EditarProductoDto dto)
        {
            

                await _producto.Editar(dto);
                return Ok();
                
        }












    }
}
