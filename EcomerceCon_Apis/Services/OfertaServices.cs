using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;

namespace EcomerceCon_Apis.Services
{
    public class OfertaServices : IOfertaService

    {
        private readonly IOfertaRepository _ofeertas;
        public OfertaServices(IOfertaRepository context)
        {
            _ofeertas = context;
        }
        public async Task CrearOferta(CrearOfertaDto ofertaDto)
            
        {
            var oferta = new Ofertas
            {
                MotivoDeOferta = ofertaDto.MotivoDeOferta,
                Fecha_inicio = ofertaDto.Fecha_inicio,
                Fecha_fin = ofertaDto.Fecha_fin,
                MontoAdescontar = ofertaDto.MontoAdescontar
            };
            if (await _ofeertas.VerificarExistenciaOferta(ofertaDto.Fecha_inicio, ofertaDto.Fecha_fin) == null)
            {
               await _ofeertas.CrearOferta(oferta);

            }
            else
            {
                throw new Exception("Ya hay una oferta disponible en esta fecha");
            }

            if(ofertaDto.ProductoIds!=null&& ofertaDto.ProductoIds.Any())
            {
                var relaciones = ofertaDto.ProductoIds.Select(proId => new OfertaProducto
                {
                    OfertaId = oferta.Id,
                    ProductoId = proId
                });
                await _ofeertas.CrearRelacionOfertasProducto(relaciones);
            }
        }

        public async Task<List<OfertaDto>> ListarOfertas()
        {
            var lista = await _ofeertas.ListarOfertas();
            if(lista!=null)
            {
                return lista.Select(o => new OfertaDto
                {
                    Id = o.Id,
                    MotivoDeOferta = o.MotivoDeOferta,
                    Fecha_inicio = o.Fecha_inicio,
                    Fecha_fin = o.Fecha_fin,
                    porcentajeADescontar = o.MontoAdescontar
                }).ToList();
            }
            else
            {
                throw new Exception("No hay ofertas disponibles");
            }
        }

        public async Task<OfertaDto>ObtenerOfertaPorId(int id)
        {
            var oferta = await _ofeertas.ObtenerOfertaPorId(id);
            if (oferta != null)
            {
                return new OfertaDto
                {
                    Id = oferta.Id,
                    MotivoDeOferta = oferta.MotivoDeOferta,
                    Fecha_inicio = oferta.Fecha_inicio,
                    Fecha_fin = oferta.Fecha_fin,
                    porcentajeADescontar = oferta.MontoAdescontar
                };
            }
            else
            {
                throw new Exception("No se encontró la oferta");
            }



        }

       public async Task EditarOferta(OfertaDto oferta)
        {
            var ofertaExistente = await _ofeertas.ObtenerOfertaPorId(oferta.Id);
            if (ofertaExistente != null)
            {
                ofertaExistente.MotivoDeOferta = oferta.MotivoDeOferta;
                ofertaExistente.Fecha_inicio = oferta.Fecha_inicio;
                ofertaExistente.Fecha_fin = oferta.Fecha_fin;
                ofertaExistente.MontoAdescontar = oferta.porcentajeADescontar;
                await _ofeertas.EditarOferta(ofertaExistente);
            }
            else
            {
                throw new Exception("No se encontró la oferta para editar");
            }
        }

        public async Task Agregar(AgregarProductosAofertaDto agregarProductosAofertaDto)
        {
           var agregar = agregarProductosAofertaDto.productosId.Select(proId => new OfertaProducto
           {
               OfertaId = agregarProductosAofertaDto.OfertaId,
               ProductoId = proId
           });
            if(agregar.Any())
            {
                await _ofeertas.CrearRelacionOfertasProducto(agregar);
            }
            else
            {
                throw new Exception("No se proporcionaron productos para agregar a la oferta");
            }
        }

        public async Task EliminarProductosDeLaOferta(AgregarProductosAofertaDto productos)
        {
           var eliminar = productos.productosId.Select(proId => new OfertaProducto
           {
               OfertaId = productos.OfertaId,
               ProductoId = proId
           });
            if (eliminar.Any())
            {
                await _ofeertas.EliminarProduDeOferta(eliminar.ToList());
            }
            else
            {
                throw new Exception("No se proporcionaron productos para eliminar de la oferta");
            }
        }

        public async Task<List<ListarOfertaProductoDto>> obtenerproductos(int Id)
        {

            var oferta = await _ofeertas.Obtenerofertaproducto(Id);

            var lista = oferta.Select(op => new ListarOfertaProductoDto
            {
                ProductoId = op.ProductoId,
                NombreProducto = op.producto.Nombre,
                OfertaId = op.OfertaId,
                NombreOferta = op.oferta.MotivoDeOferta
            }).ToList();
            return lista;


        }
            
        }
    }

