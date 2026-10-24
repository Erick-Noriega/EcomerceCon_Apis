using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Data
{
    public class ProductosDbcontext : IdentityDbContext<Usuario>
    {
        public ProductosDbcontext(DbContextOptions<ProductosDbcontext> options) : base(options)
        {
        }
        public DbSet<Models.Producto> Productos { get; set; }
        public DbSet<Models.Categoria> Categorias { get; set; }
        public DbSet<Models.DetalleCarrito> DetalleCarritos { get; set; }
        public DbSet<Models.Carrito> Carritos { get; set; }
        public DbSet<Models.MensajeRecibido>MensajeRecibidos{ get; set; }
        public DbSet<Models.DetallePedido> DetallePedidos { get; set; }
        public DbSet<Models.Pedido> Pedidos { get; set; }
        public DbSet<Models.Ofertas> Ofertas { get; set; }
        public DbSet<Models.Pagos> Pagos { get; set; }
        public DbSet<OfertaProducto>ofertaProductos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definición explícita de la relación Muchos a Muchos
            modelBuilder.Entity<OfertaProducto>()
       .ToTable("OfertaProducto") // Nombre exacto de tu tabla en SQL
       .HasKey(op => new { op.OfertaId, op.ProductoId }); // Clave primaria compuesta

            modelBuilder.Entity<OfertaProducto>()
                .HasOne(op => op.producto)
                .WithMany(p => p.ofertas)
                .HasForeignKey(op => op.ProductoId);

            modelBuilder.Entity<OfertaProducto>()
                .HasOne(op => op.oferta)
                .WithMany(o => o.Productos)
                .HasForeignKey(op => op.OfertaId);

            modelBuilder.Entity<MensajeRecibido>()
            .HasIndex(m => m.Mensaje)
            .IsUnique();
          
        }
    }
}
