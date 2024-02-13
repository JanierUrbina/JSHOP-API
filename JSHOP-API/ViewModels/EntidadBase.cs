using System.ComponentModel.DataAnnotations.Schema;

namespace API_CHATN.ViewModels
{
    public class EntidadBase
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
