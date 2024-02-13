
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using API_CHATN.Modelo;
using static Api_ChatN.Helpers.IAuthService;
using Api_ChatN.Helpers;
using JSHOP_API.Modelo;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Configuracion del identity
builder.Services.AddIdentityCore<Usuario>().AddRoles<IdentityRole>().AddEntityFrameworkStores<Stokdbcontext>();

builder.Services.AddControllers();
builder.Services.AddScoped<Stokdbcontext>();//Inyección de la dependencia solicitada


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();



//Configuración de servicios para la autenticación del JASON WEB TOKEN (JWT)
builder.Services.AddAuthorization(options => options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

//Obtenemos los valores asignados en nuestro appsetting:
var issuer = builder.Configuration["AuthenticationSetting:Issuer"];
var Audence = builder.Configuration["AuthenticationSetting:Audience"];
var SignKey = builder.Configuration["AuthenticationSetting:SigningKey"];

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler  =System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});//configura la serialización JSON para que ignore las referencias circulares

//Asignamos la autenticacion
builder.Services.AddHttpContextAccessor()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
    {
        opt.Audience = Audence;
        opt.SaveToken = true;
        opt.RequireHttpsMetadata = false;

        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = Audence,
            ValidIssuer = issuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SignKey)),
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");//asigna index en home como la vista principal    
});


app.MapPost("/Token", async ([FromBody]validateLogin request, UserManager<Usuario> userManager) =>
{
    // Verificamos credenciales con Identity
    var user = await userManager.FindByNameAsync(request.username);

    if (user is null || !await userManager.CheckPasswordAsync(user, request.password))
    {
        return Results.Conflict(
            new
            {
                msj = "Credenciales incorrectas"
            }
          );
    }

    var roles = await userManager.GetRolesAsync(user);

    // Generamos un token según los claims
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Sid, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.GivenName, $"{user.Nombre}")
    };

    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SignKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
    var tokenDescriptor = new JwtSecurityToken(
        issuer: issuer,
        audience: Audence,

        claims: claims,
        expires: DateTime.Now.AddMinutes(720),
        signingCredentials: credentials);

    var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

    return Results.Ok(new
    {
        AccessToken = jwt
    });
});

//Creamos los usuarios, de no haber uno, esto es al comienzo de el uso de la aplicación
async Task SeedData()
 {
    var scopedFactory = app!.Services.GetRequiredService<IServiceScopeFactory>();
    using var scope = scopedFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<Stokdbcontext>();
    var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    context.Database.EnsureCreated();
    var c = builder.Configuration;
    Sucursal sucursal = new Sucursal();
    using(var j = new Stokdbcontext(c))
    {
        var s = new Sucursal()
        {
            Nombre = "Todos",
            Descripcion ="Todas las sucursales",
            Dirección = "Todas las sucursales",
            FechaCreacion = DateTime.Now,
            Estado = false
        };
        if(!j.sucursal.Any())
        {
            j.sucursal.Add(s);
            j.SaveChanges();
        }
        
        sucursal = j.sucursal.FirstOrDefault(x=>x.Nombre.ToLower()=="Todos");
        
    }
    if (!usermanager.Users.Any())
    {
        logger.LogInformation("Creando nuevo");
        var newuser = new Usuario
        {
            Email = "admin@prueba.com",
            UserName = "Administrador",
            CodigoUsuario = 1234,
            Nombre = "Admin",
            IdSucursal = sucursal!=null?sucursal.Id:throw new Exception("Error Interno")
        };
        await usermanager.CreateAsync(newuser, "Admin123$");
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "Admin"
        });
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "Estandar"
        });
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Sid, newuser.Id),
        new Claim(ClaimTypes.Name, newuser.UserName),
        new Claim(ClaimTypes.GivenName, $"{newuser.Nombre}")
    };
        await usermanager.AddToRoleAsync(newuser, "Admin");
        foreach (var j in claims)
        {
            await usermanager.AddClaimAsync(newuser, j);
        }

    }
}

await SeedData();

app.Run();
