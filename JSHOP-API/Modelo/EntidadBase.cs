using System.ComponentModel.DataAnnotations.Schema;

namespace API_CHATN.Modelo
{
    public class EntidadBase
    {
        [Column("id")]
        public virtual long Id { get; set; }
        [Column("fechacreacion")]
        public virtual DateTime FechaCreacion { get; set; }
        [Column("estado")]
        public virtual bool Estado { get; set; }
    }
}
