using API_CHATN.Modelo;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSHOP_API.Modelo
{
    [Table("Producto")]
    public partial class Producto : EntidadBase
    {
        [Column("Nombre")]
        public virtual string? Nombre { get; set; }
        [Column("Descripcion")]
        public virtual string? Descripcion { get; set; }

        [Column("Cantidad")]
        public virtual int Cantidad { get; set; }

        [Column("Marca")]
        public virtual string? Marca { get; set; }

        [Column("Precio")]
        public virtual float Precio { get; set; }

        [Column("RutaImagen")]
        public virtual string RutaImagen { get; set; }

        public virtual long CategoriaId { get; set; }
        public virtual long SucursalId { get; set; }
        public virtual long ProveedorId { get; set; }


        
        public virtual Categoria Categoria { get; set; }
       
        public virtual Sucursal Sucursal { get; set; }
        
        public virtual Proveedor Proveedor { get; set; }
    }
}
