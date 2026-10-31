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


            try
            {

                var producto = await _producto.ListaDeAdmins();
                if (producto != null)
                {

                    return Ok(producto);
                }
                else
                {
                    return BadRequest(new RespuestaGenericaDto<EditarProductoDto>
                    {
                        IsSuccess = false,
                        Mensaje = "error,no se econtraron productos"

                    });

                }
            }
            catch (Exception)
            {


                return BadRequest(new RespuestaGenericaDto<EditarProductoDto>
                {
                    IsSuccess = false,
                    Mensaje = "error,no se econtraron productos"

                });
            }


        }


        [HttpGet]
        [Route("Lista")]
       
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<ProductoDto>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> listar()
        {

            try
            {

                var producto = await _producto.lista();
                if (producto != null)
                {
                    return Ok(producto);
                }
                else
                {
                    return Ok(new RespuestaGenericaDto<EditarProductoDto>
                    {
                        IsSuccess = false,
                        Mensaje = "no se encontraron productos"
                    });
                }


            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el producto: " + ex.Message);


            }

        }
        [HttpGet]
        [Route("ListarParaOferta")]
        public async Task<IActionResult> ListaNorma()
        {
            var producto = await _producto.ListarParaOFerta();

            if (producto != null)
            {
                return Ok(producto);
            }
            else
            {
                return BadRequest(new RespuestaGenericaDto<EditarProductoDto>
                {
                    IsSuccess = false,
                    Mensaje = "no se encontraron productos"
                });
            }
        }

        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Crear([FromBody] CrearProductoDto productoDto)
        {
            try
            {
                var producto = await _producto.Create(productoDto);


                if (await _context.Productos.Where(n => n.Nombre == producto.Nombre).FirstOrDefaultAsync() == null)
                {

                    _context.Add(producto);

                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Producto creado correctamente" });
                }

                else if (await _context.Productos.Where(n => n.Nombre == producto.Nombre).FirstOrDefaultAsync() != null)
                {
                    var pelota = await _context.Productos.Where(n => n.Nombre == producto.Nombre).FirstOrDefaultAsync();
                    return BadRequest(new { message = "El producto ya existe" });
                }
                else
                {
                    return BadRequest(new { message = "Intente mas tarde" });

                }

            }
            catch (Exception ex)
            {

                {
                    return BadRequest(new { message = $"error{ex.Message}" });

                }
            }

        }
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Obtener(int Id)
        {
            try
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
            catch (Exception ex)
            {

                throw new Exception("Error al obtener el producto: " + ex.Message);

            }


        }
        [HttpPut]
        [Route("Editar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Editar([FromBody] EditarProductoDto dto)
        {
            try
            {

                var produ = await _producto.Editar(dto);

                if (produ != null)
                {
                    _context.Update(produ);
                    await _context.SaveChangesAsync();
                    return Ok(new RespuestaGenericaDto<bool>
                    {
                        IsSuccess = true,
                        Mensaje = "Producto Editado Correctamente",
                        Data = true,
                    });
                }
                else
                {
                    return BadRequest(new RespuestaGenericaDto<bool>
                    {
                        IsSuccess = false,
                        Mensaje = "Hubo un error al editar el producto",

                    });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new RespuestaGenericaDto<bool>
                {
                    IsSuccess = false,
                    Mensaje = "Hubo un error al editar el producto",

                });
            }


           
        }












    }
}
