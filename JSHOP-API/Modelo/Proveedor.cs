using API_CHATN.Modelo;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSHOP_API.Modelo
{
    [Table("Proveedor")]
    public partial class Proveedor:EntidadBase
    {
      
        [Column("Nombre")]
        public string Nombre { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
    
    }
}
