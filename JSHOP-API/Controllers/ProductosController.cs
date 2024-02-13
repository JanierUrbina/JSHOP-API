using Api_ChatN.Helpers;
using API_CHATN.Modelo;
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace JSHOP_API.Controllers
{
    public class BuscarProductoModel
    {
        public List<long> IdParametro { get; set; }
        public List<int> TipoParametro { get; set; }
    }
    [Authorize]
    public class ProductosController : ControllerBase
    {
        Stokdbcontext _context;
        public ProductosController(Stokdbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("Producto/ListarProductos")]
        public IActionResult ListarProductos()
        {
            try
            {
                var ListaProductos =  from prodcutos in _context.productos.ToList()
                                      join categoria in _context.categotia.ToList() on prodcutos.CategoriaId equals categoria.Id
                                      join sucursal in _context.sucursal.ToList() on prodcutos.SucursalId equals sucursal.Id
                                      join proveedor in _context.proveedor.ToList() on prodcutos.ProveedorId equals proveedor.Id
                                      select prodcutos;

               

                return Ok (ListaProductos);
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("Producto/EditarProducto")]
        public IActionResult EditarProducto([FromBody] Producto prod)
        {
            try
            {
                var producto = _context.productos.Find(prod.Id);
                if (prod != null)
                {
                    producto.Cantidad = prod.Cantidad;
                    producto.Precio = prod.Precio;
                    producto.Nombre = prod.Nombre;
                    producto.Descripcion = prod.Descripcion;
                    producto.SucursalId = prod.SucursalId;
                    producto.CategoriaId = prod.CategoriaId;
                    producto.ProveedorId = prod.ProveedorId;
                    producto.Marca = prod.Marca;
                    producto.RutaImagen = prod.RutaImagen;
                   
                          
                    _context.Entry(producto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
        [Route("Producto/BuscarProducto")]
        public IActionResult BuscarProducto(long id)
        {
            var producto = from p in _context.productos.ToList() where p.Id==id select p;
            return Ok(producto.FirstOrDefault());
        }
        //public IActionResult BuscarProducto([FromBody] BuscarProductoModel model)
        //{
        //    try
        //    {
        //        var producto = from p in _context.productos.ToList() select p;
        //        IEnumerable<Producto> condicion = producto;
        //        int n=0;
        //        foreach(var tp in model.TipoParametro)
        //        {
        //            switch(tp)
        //            {
        //                case 1: condicion = condicion.Where(x => x.Sucursal.Id == model.IdParametro[n]); break;
        //                case 2: condicion = condicion.Where(x => x.Proveedor.Id == model.IdParametro[n]); break;
        //                case 3: condicion = condicion.Where(x => x.Categoria.Id == model.IdParametro[n]); break;
        //                default: condicion = producto; break;
        //            }
        //            n++;
        //        }
        //        return Ok(condicion.ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.LogErrores(ex.Message);
        //        return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
        //    }
        //}

        [HttpPost]
        [Route("Producto/AgregarProducto")]       
        public IActionResult AgregarProducto([FromBody] Producto producto)
        {
            try
            {
                if (User.Identity != null ? User.Identity.IsAuthenticated : true)
                {
                    producto.FechaCreacion = DateTime.Now;
                    _context.productos.Add(producto);
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
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: "+ex.Message });
            }
        }
        [HttpGet]
        [Route("Producto/DescontarProducto")]
        public IActionResult DescontarProducto(long IdProducto, int Cantidad)
        {
            try
            {
                var productoEncontrado = _context.productos.Find(IdProducto);
                if (productoEncontrado != null)
                {
                    if(productoEncontrado.Cantidad==0)
                    {
                        return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "De momento no contamos con el producto "+productoEncontrado.Nombre +" en Stock" });
                    }

                    if(Cantidad<=productoEncontrado.Cantidad)
                    {
                        productoEncontrado.Cantidad = productoEncontrado.Cantidad - Cantidad;
                        _context.Entry(productoEncontrado).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Producto modificado" });
                    }
                    else
                    {
                        return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "El producto "+productoEncontrado.Nombre+" solo dispone de "+productoEncontrado.Cantidad + " en Stock" });
                    }
                   
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.NoEncontrado, Mensaje = "Producto NO Encontrado" });
                }
            }
            catch (Exception ex)
            {
                Logs.LogErrores(ex.Message);
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error interno: " + ex.Message });
            }
        }
         

        [HttpGet]
        [Route("Producto/DeshabilitarProducto")]
        public IActionResult DeshabilitarProducto(long IdPrd)
        {
            try
            {
                var producto = _context.productos.Find(IdPrd);
                if (producto != null)
                {
                    producto.Estado = true; //Cuando el estado base, se encuentra en true, signfica que el registro se encuentra deshabilitado.
                    _context.Entry(producto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
