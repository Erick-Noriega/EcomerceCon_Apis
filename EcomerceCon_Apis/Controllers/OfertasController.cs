using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class OfertasController : ControllerBase
    {
        private readonly IProductosService _context;
        private readonly IOfertaService _oferta;
        public OfertasController(IProductosService context, IOfertaService oferta)
        {
            
            _oferta = oferta;
        }

        [HttpPost]
        [Route("CrearOferta")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearOfertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> CrearOferta([FromBody] CrearOfertaDto ofertadto)
        {
            
            await _oferta.CrearOferta(ofertadto);
            
                return Ok(new { message = "oferta Creada Correctamente" });
          

        }
        [HttpGet]
        [Route("Lista")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<OfertaDto>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Lista()
        {
           
            var oferta =await _oferta.ListarOfertas();
            return Ok(oferta);

        }
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        public async Task<IActionResult>Obtener(int Id)
        {
           var oferta =await _oferta.ObtenerOfertaPorId(Id);

            return Ok(oferta);


        }

        [HttpPut]
        [Route("Editar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<OfertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Editar([FromBody]OfertaDto ofertadto)
        {
            await _oferta.EditarOferta(ofertadto);
            return Ok(new { message = "Oferta Editada Correctamente" });
        }
        [HttpPost]
        [Route("AgregarProductos")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<AgregarProductosAofertaDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> AgregarProductos(AgregarProductosAofertaDto productos)
        {

          await _oferta.Agregar(productos);
            return Ok(new { message = "Productos Agregados a la oferta Correctamente" });


        }
        [HttpPost]
        [Route("SacarDeOferta")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<AgregarProductosAofertaDto>), StatusCodes.Status200OK)] 

        public async Task<IActionResult> Eliminarprodu(AgregarProductosAofertaDto productos)
        {
            await _oferta.EliminarProductosDeLaOferta(productos);
            return Ok(new { message = "Productos Eliminados de la oferta Correctamente" });

        }
        [HttpGet]
        [Route("ObtenerOfertaProducto/{Id:int}")]
        public async Task<IActionResult> Obtenerproductos(int Id)
        {
            var produoff = await _oferta.obtenerproductos(Id);
            return Ok(produoff);
        }
        [HttpGet]
        [Route("VistaParaViewModel/{Id:int}")]
        public async Task<IActionResult> VistaParaViewModel(int Id)
        {
           var oferta = await _oferta.VistaParaViewModel(Id);
            return Ok(oferta);
        }
    }
}
