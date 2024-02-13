using Api_ChatN.Helpers;
using API_CHATN.Modelo;
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSHOP_API.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        Stokdbcontext _context;
        public CategoriaController(Stokdbcontext context)
        {
            _context = context;
        }

      
        [Route("Categoria/ListarCategoria")]
        [HttpGet]
        public IActionResult ListarCategoria(int id)
        {
            if (id == 0)
            {
                return Ok(_context.categotia.ToList());
            }
            else
            {
                return Ok(_context.categotia.Where(x => x.Id == id).FirstOrDefault());
            }
        }
        [Route("Categoria/AgregarCategoria")]
        [HttpPost]
        public IActionResult AgregarCategoria([FromBody] Categoria categoria)
        {
            try
            {
                if (User.Identity != null ? User.Identity.IsAuthenticated : true)
                {
                    categoria.FechaCreacion = DateTime.Now;
                    _context.categotia.Add(categoria);                    
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
        [Route("Categoria/EditarCategoria")]
        public IActionResult EditarCategoria([FromBody] Categoria cate)
        {
            try
            {
                var categoria = _context.categotia.Find(cate.Id);
                if (cate != null)
                {

                    categoria.Nombre = cate.Nombre;
                    categoria.Descripcion = cate.Descripcion;
                                     
                    _context.Entry(categoria).State =Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Categoria modificada" });
                   
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "Categoria NO Encontrada" });
                }
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("Categoria/DeshabilitarCategoria")]
        public IActionResult DeshabilitarCategoria(long IdSucur)
        {
            try
            {
                var categoria = _context.categotia.Find(IdSucur);
                if (categoria != null)
                {
                    categoria.Estado = true; //Cuando el estado base, se encuentra en true, signfica que el registro se encuentra deshabilitado.
                    _context.Entry(categoria).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Categoria modificada" });
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "Categoria NO Encontrada" });
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
