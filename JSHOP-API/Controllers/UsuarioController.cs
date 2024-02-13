using Api_ChatN.Helpers;
using API_CHATN.Modelo;
using Azure.Core;
using JSHOP_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
namespace JSHOP_API.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        
        UserManager<Usuario> _usermanager;
        IPasswordHasher<Usuario> _passwordHasher;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsuarioController(UserManager<Usuario> user, IPasswordHasher<Usuario> passwordHasher, RoleManager<IdentityRole> roleManager)
        {
            _usermanager = user;
            _passwordHasher = passwordHasher;
            _roleManager = roleManager;
        }

        [Route("usuario/ListaUsuarios")]
        [HttpGet]
        public IActionResult ListaUsuarios()
        {
            try
            {
                var en = _usermanager.Users.ToList();
                return Ok(en);
            }
            catch (Exception ex)
            {
                return BadRequest(new jsonRespuesta
                {
                    estado = Estado.ErrorInterno,
                    Mensaje = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateRol(string Rol)
        {
            try
            {               
                    if (_roleManager.Roles.Any(u => u.Name == Rol))
                    {
                        return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "El rol ingresado ya existe." });
                    }
                    else
                    {
                        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(Rol));
                        if (result.Succeeded)
                            return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Guardado Correctamente" });
                    }
                
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Algo malo ocurrió" });
            }
            catch (Exception ex)
            {
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = ex.Message });
            }

        }

        [HttpPost]
        [Route("usuario/CrearUsuario")]        
        public async Task<IActionResult> CrearUsuario([FromBody]UsuarioVM usuario)
        {
            try
            {
                var yaexiste = _usermanager.FindByNameAsync(usuario.Nombre).Result != null ? true:false;
                if(yaexiste)
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.UsuarioExistente, Mensaje = "Ya existe un usuario con ese nombre" });
                }
                else
                {
                    var user = new Usuario
                    {
                        Nombre = usuario.Nombre,
                        CodigoUsuario = usuario.CodigoUsuario,
                        Email = usuario.correo,
                        UserName = usuario.Nombre,
                        IdSucursal = usuario.IdSucursal
                    };
                   var r = await _usermanager.CreateAsync(user, usuario.Password);
                    if(r.Succeeded)
                    {
                        await _usermanager.AddToRoleAsync(user, usuario.nombrerol);
                        await _usermanager.AddClaimAsync(user, new Claim("Id", user.Id));
                        await _usermanager.AddClaimAsync(user, new Claim("Nombre", user.Nombre));
                        return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje ="Guardado Correctamente"});
                    }
                    else
                    {
                        string msj="";
                        var errors = r.Errors.ToList();
                         errors.ForEach(x => msj = x.Code.StartsWith("Password") ? "La contraseña debe tener al menos un caracter alfanumerico, una mayúscula, una minúsc1ula y un número": "No se guardó"); //Si se encuentra un problema con la contraseña

                        return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = msj });
                    }                   
                }
               
            }
            catch (Exception ex)
            {
              return BadRequest(new jsonRespuesta {
                    estado = Estado.ErrorInterno, Mensaje = ex.Message});
            }
           
       }
        [Route("usuario/ActualizarContraseña")]
        public async Task<IActionResult> ActualizarContraseña(string id, string Contraseña)
        {
            try
            {
                if(User.Identity.IsAuthenticated)
                {                    
                    var user = await _usermanager.FindByIdAsync(id);
                    user.PasswordHash = _passwordHasher.HashPassword(user,Contraseña);                    
                    var cambiado = await _usermanager.UpdateAsync(user);
                    if(cambiado.Succeeded)
                    {
                        return Ok(new jsonRespuesta { estado = Estado.Exito, Mensaje = "Actualizado Correctamente" });
                    }
                    else
                    {
                        return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error al actualizar" });
                    }
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Error al actualizar" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = ex.Message });
            }
        }
        [HttpGet]
        [Route("usuario/GetUser")]        
        public async Task<IActionResult> GetUser()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _usermanager.FindByNameAsync(User.Identity.Name);

                    var roles = await _usermanager.GetRolesAsync(user);
                    var NewUser = new UserAuthenticated()
                    {
                        Name = User.Identity.Name,
                        Role = roles.FirstOrDefault(),
                        IsAuthenticated = User.Identity.IsAuthenticated
                    };
                    return Ok(NewUser);
                }
                else
                {
                    return BadRequest(new jsonRespuesta { estado = Estado.Mal, Mensaje = "Unauthorized" });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        [HttpGet]
        [Route("usuario/GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var rolist = await _roleManager.Roles.ToListAsync();
            return Ok(rolist);
        }
    }
}
