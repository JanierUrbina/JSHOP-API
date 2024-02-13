using Api_ChatN.Helpers;
using API_CHATN.Modelo;
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSHOP_API.Controllers
{
    [Authorize] 
    public class ProveedorController : Controller
    {
        Stokdbcontext _context;
        public ProveedorController(Stokdbcontext context)
        {
            _context = context;
        }
        [Route("Proveedor/ListarProveedor")]
        [HttpGet]
        public IActionResult ListarProveedor(int id)
        {
            if(id==0)
            {
                return Ok(_context.proveedor.ToList());
            }
            else
            {
                return Ok(_context.proveedor.Where(x=>x.Id==id).FirstOrDefault());
            }
        }
        [Route("Proveedor/AgregarProveedor")]
        [HttpPost]
        public IActionResult AgregarProveedor([FromBody] Proveedor proveedor)
        {
            try
            {
                if (User.Identity != null ? User.Identity.IsAuthenticated : true)
                {
                    proveedor.FechaCreacion = DateTime.Now;
                    _context.proveedor.Add(proveedor);                 
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Guardado con éxito" });
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno" });
                }

            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }
        [HttpPost]
        [Route("Proveedor/EditarProveedor")]
        public IActionResult EditarProveedor([FromBody]Proveedor prov)
        {
            try
            {
                var proveedor = _context.proveedor.Find(prov.Id);
                if (proveedor != null)
                {
                    proveedor.Nombre = prov.Nombre;
                    proveedor.Descripcion = prov.Descripcion;
                                     
                    _context.Entry(proveedor).State =Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "proveedor modificado" });
                   
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "proveedor NO Encontrado" });
                }
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("Proveedor/DeshabilitarProveedor")]
        public IActionResult DeshabilitarProveedor(long IdProv)
        {
            try
            {
                var proveedor = _context.proveedor.Find(IdProv);
                if (proveedor != null)
                {
                    proveedor.Estado = true; //Cuando el estado base, se encuentra en true, signfica que el registro se encuentra deshabilitado.
                    _context.Entry(proveedor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "proveedor modificado" });

                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "proveedor NO Encontrado" });
                }
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }
    }
}
