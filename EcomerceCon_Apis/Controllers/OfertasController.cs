using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        public OfertasController(ProductosDbcontext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CrearOferta")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearOfertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> CrearOferta([FromBody] CrearOfertaDto ofertadto)
        {
            var fecha = DateTime.Now;
            var oferta = new Ofertas
            {
                MotivoDeOferta = ofertadto.MotivoDeOferta,
                MontoAdescontar = ofertadto.MontoAdescontar,
                Fecha_inicio = ofertadto.Fecha_inicio,
                Fecha_fin = ofertadto.Fecha_fin,
                disponible = true

            };

            bool exitencia = _context.Ofertas.Any(o => ofertadto.Fecha_inicio <= o.Fecha_fin && ofertadto.Fecha_fin >= o.Fecha_inicio);

            if (! exitencia)

            { 
            _context.Ofertas.Add(oferta);
                await _context.SaveChangesAsync();
            }

            
            else if(exitencia)
            {
                return Ok(new { message = "Ya hay una oferta disponible en esta fecha" });
            }

        if (ofertadto.ProductoIds != null && ofertadto.ProductoIds.Any())
            {
                var relaciones = ofertadto.ProductoIds.Select(proId => new OfertaProducto
                {
                    OfertaId = oferta.Id,
                    ProductoId = proId
                });
                _context.ofertaProductos.AddRange(relaciones);
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Oferta creada exitosamente" });

        }
        [HttpGet]
        [Route("Lista")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<OfertaDto>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Lista()
        {
           // var aux =await _context.Ofertas.ToListAsync();
           //if ( _context.Ofertas.ToListAsync() == null)
           // {
           //     return BadRequest(new {message=""});
           // }
            var oferta = await _context.Ofertas.Select(o => new OfertaDto
            {
                Id = o.Id,
                MotivoDeOferta = o.MotivoDeOferta,
                porcentajeADescontar = o.MontoAdescontar,
                Fecha_inicio = o.Fecha_inicio,
                Fecha_fin = o.Fecha_fin,
                disponible=o.disponible,
              
            }).ToListAsync();
           
                
            return Ok(oferta);

        }
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        public async Task<IActionResult>Obtener(int Id)
        {
            var oferta= await _context.Ofertas.Select(o => new OfertaDto
            {
                Id=o.Id,
                disponible = o.disponible,
                Fecha_fin=o.Fecha_fin,
                Fecha_inicio=o.Fecha_inicio,
                MotivoDeOferta=o.MotivoDeOferta,
                porcentajeADescontar=o.MontoAdescontar,


            }).FirstOrDefaultAsync(o=>o.Id==Id);

            return Ok(oferta);


        }

        [HttpPut]
        [Route("Editar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<OfertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Editar([FromBody]OfertaDto ofertadto)
        {
            bool exitencia = _context.Ofertas.Any(o =>ofertadto.Id!=ofertadto.Id &&ofertadto.Fecha_inicio <= o.Fecha_fin && ofertadto.Fecha_fin >= o.Fecha_inicio);
            if (!exitencia)
            {
                _context.Ofertas.Update(new Ofertas
                {
                    Id = ofertadto.Id,
                    MotivoDeOferta = ofertadto.MotivoDeOferta,
                    Fecha_inicio = ofertadto.Fecha_inicio,
                    Fecha_fin = ofertadto.Fecha_fin,
                    MontoAdescontar = ofertadto.porcentajeADescontar,
                    disponible = ofertadto.disponible,

                });
                await _context.SaveChangesAsync();
                if(ofertadto.IdProductos!=null && ofertadto.IdProductos.Any())
                {
                    var agregarProductos = ofertadto.IdProductos.Select(o => new OfertaProducto
                    {
                        OfertaId = ofertadto.Id,
                        ProductoId=o
                    });
                    _context.AddRange(agregarProductos);
                    await _context.SaveChangesAsync();

                }

                return Ok("Oferta Actualizada Con exito");
            }
            else
            {
                return Ok("en ese Rango de fecha ya hay Otra Oferta Disponible!!!");
            }
        }
        [HttpPost]
        [Route("AgregarProductos")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<AgregarProductosAofertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> AgregarProductos(AgregarProductosAofertaDto productos)
        {

            var agregar = productos.productosId.Select(p => new OfertaProducto
            {
                OfertaId = productos.OfertaId
                ,
                ProductoId = p
            });
            _context.AddRange(agregar);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "productos agregados correctamente"
            });


        }
        [HttpPost]
        [Route("SacarDeOferta")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<AgregarProductosAofertaDto>), StatusCodes.Status200OK)] 

        public async Task<IActionResult> Eliminarprodu(AgregarProductosAofertaDto productos)
        {
            var agregar = productos.productosId.Select(p => new OfertaProducto
            {
                OfertaId = productos.OfertaId
               ,
                ProductoId = p
            });
            _context.RemoveRange(agregar);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Productos Eliminados de la oferta Correctamente" });

        }
        [HttpGet]
        [Route("ObtenerOfertaProducto/{Id:int}")]
        public async Task<IActionResult> Obtenerproductos(int Idoferta)
        {
            var produoff = _context.ofertaProductos.Where(p => p.OfertaId == Idoferta).Select(op => new ListarOfertaProductoDto
            {
                ProductoId = op.ProductoId,
                NombreProducto = op.producto.Nombre,
                OfertaId=op.OfertaId,
                NombreOferta=op.oferta.MotivoDeOferta
            });
            return Ok(produoff);
        }
        
    }
}
