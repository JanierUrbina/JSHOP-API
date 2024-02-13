using Api_ChatN.Helpers;
using API_CHATN.Modelo;
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSHOP_API.Controllers
{
    
    public class SucursalController : Controller
    {
        Stokdbcontext _context;
        public SucursalController(Stokdbcontext context)
        {
            _context = context;
        }
        [Route("Sucursal/ListarSucursal")]
        [HttpGet]
        public IActionResult ListarSucursal(int id)
        {
            if (id == 0)
            {
                return Ok(_context.sucursal.ToList());
            }
            else
            {
                return Ok(_context.sucursal.Where(x => x.Id == id).FirstOrDefault());
            }
        }
        [Route("Sucursal/AgregarSucursal")]
        [HttpPost]
        public IActionResult AgregarSucursal([FromBody] Sucursal sucursal)
        {
            try
            {
                if (User.Identity != null ? User.Identity.IsAuthenticated : true)
                {
                    sucursal.FechaCreacion = DateTime.Now;
                    _context.sucursal.Add(sucursal);
                    var l = _context.categotia.ToList();
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
        [Route("Sucursal/EditarSucursal")]
        public IActionResult EditarSucursal([FromBody]Sucursal sucur)
        {
            try
            {
                var sucursal = _context.sucursal.Find(sucur.Id);
                if (sucursal != null)
                {

                    sucursal.Nombre = sucur.Nombre;
                    sucursal.Descripcion = sucur.Descripcion;
                    sucursal.Dirección = sucur.Dirección;
                                      
                    _context.Entry(sucursal).State =Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Sucursal modificada" });
                   
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "Sucursal NO Encontrada" });
                }
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("Sucursal/DeshabilitarSucursal")]
        public IActionResult DeshabilitarSucursal(long IdSucur)
        {
            try
            {
                var sucursal = _context.sucursal.Find(IdSucur);
                if (sucursal != null)
                {
                    sucursal.Estado = true; //Cuando el estado base, se encuentra en true, signfica que el registro se encuentra deshabilitado.
                    _context.Entry(sucursal).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Sucursal modificada" });

                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "Sucursal NO Encontrada" });
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
