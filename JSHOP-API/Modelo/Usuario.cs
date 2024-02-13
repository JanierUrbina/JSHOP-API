using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace API_CHATN.Modelo
{
    [Table("usuario")]
    public class Usuario:IdentityUser
    {
        [Column("codigousuario")]
        public virtual int CodigoUsuario { get; set; }
        [Column("nombre")]
        public virtual string Nombre { get; set; }
      
        public virtual long IdSucursal { get; set; }


    }

    public class UsuarioVM
    {
        [Column("codigousuario")]
        public virtual int CodigoUsuario { get; set; }
        [Column("nombre")]
        public virtual string Nombre { get; set; }
        public string Password { get; set; }
        public string nombrerol { get; set; }
        public string correo { get; set; }
        public virtual long IdSucursal { get; set; }
       
    }
    
}
