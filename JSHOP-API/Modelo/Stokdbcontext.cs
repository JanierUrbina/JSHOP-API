
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace API_CHATN.Modelo
{
    public class Stokdbcontext : IdentityDbContext<Usuario>
    {
        protected readonly IConfiguration _configuration;        
        public Stokdbcontext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("WebApiDatabase"));
        }    
        public virtual DbSet<Producto> productos { get; set; }
        public virtual DbSet<Sucursal> sucursal { get; set; }
        public virtual DbSet<Categoria> categotia { get; set; }
        public virtual DbSet<Proveedor> proveedor { get; set; }

    }
}
