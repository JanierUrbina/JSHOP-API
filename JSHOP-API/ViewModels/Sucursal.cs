﻿using API_CHATN.Modelo;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSHOP_API.ViewModels
{
  
    public class Sucursal : EntidadBase
    {
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Dirección")]
        public string Dirección { get; set; }
       
        [Column("Descripcion")]
        public string Descripcion { get; set; }
    }
}
