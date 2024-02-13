using API_CHATN.Modelo;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSHOP_API.ViewModels
{
  
    public class Producto : EntidadBase
    {
        [Column("Nombre")]
        public string? Nombre { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Marca")]
        public string? Marca { get; set; }

        [Column("Descripcion")]
        public string? Descripcion { get; set; }


        [Column("IdSucursal")]
        public long SucursalId { get; set; }

        [Column("IdCategoría")]
        public long CategoriaId { get; set; }

        [Column("IdProveedor")]
        public long ProveedorId { get; set; }

        [Column("Precio")]
        public virtual float Precio { get; set; }

        public virtual string RutaImagen { get; set; }

    }
}
