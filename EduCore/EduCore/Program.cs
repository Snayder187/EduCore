using EduCore;
using EduCore.Datos;
using EduCore.Entidades;
using EduCore.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//ÁREA DE SERVICIOS

builder.Services.AddDataProtection(); //Se encarga de crear el DataProtection (Encriptación)

//CONEXION CON CORS (FRONT & BACK)
var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders("mi-cabecera");
    });
});

builder.Services.AddAutoMapper(typeof(Program));
//El agregar el AddJsonOptions es momentaneo y sirve para liberar el ciclo de tablas
builder.Services.AddControllers().AddNewtonsoftJson();

//Aquí va la cadena de conexión
builder.Services.AddDbContext<ApplicationDBContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
builder.Services.AddTransient<IServicioHash, ServicioHash>(); //HASH

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//ÁREA DE MIDDLEWARES
app.Use(async (contexto, next) =>
{
    contexto.Response.Headers.Append("mi-cabecera", "valor");
    await next();
});

app.MapControllers();

app.UseCors();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
