using EduCore;
using EduCore.Datos;
using EduCore.Entidades;
using EduCore.Servicios;
using EduCore.Swagger;
using EduCore.Utilidades;
using EduCore.Utilidades.V1;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//ÁREA DE SERVICIOS

////Caché Local
builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

//Caché en Redis
//builder.Services.AddStackExchangeRedisOutputCache(opciones =>
//{
//    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
//});

builder.Services.AddDataProtection(); //Se encarga de crear el DataProtection (Encriptación)

//CONEXION CON CORS (FRONT & BACK)
var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders("cantidad-total-registros");
    });
});

builder.Services.AddAutoMapper(typeof(Program));
//El agregar el AddJsonOptions es momentaneo y sirve para liberar el ciclo de tablas
builder.Services.AddControllers(opciones =>
{
    opciones.Conventions.Add(new ConvencionAgrupadoPorVersion());
}).AddNewtonsoftJson();

//Aquí va la cadena de conexión
builder.Services.AddDbContext<ApplicationDBContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
//builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>(); //Para subir foto en Azure
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddScoped<FiltroValidacionAlumno>();
builder.Services.AddScoped<EduCore.Servicios.V1.IServicioApoderados, 
            EduCore.Servicios.V1.ServicioApoderados>();

builder.Services.AddScoped<EduCore.Servicios.V1.IGeneradorEnlaces,
            EduCore.Servicios.V1.GeneradorEnlaces>();

builder.Services.AddScoped<HATEOASApoderadoAttribute>();
builder.Services.AddScoped<HATEOASApoderadosAttribute>();

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

builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "EduCore API",
        Description = "Este es un web api para trabajar con datos de apoderados y alumnos",
        Contact = new OpenApiContact
        {
            Email = "hector.rs187@gmail.com",
            Name = "Hector Ramos",
            Url = new Uri("https://www.instagram.com/hector.rs187")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    opciones.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "EduCore API",
        Description = "Este es un web api para trabajar con datos de apoderados y alumnos",
        Contact = new OpenApiContact
        {
            Email = "hector.rs187@gmail.com",
            Name = "Hector Ramos",
            Url = new Uri("https://www.instagram.com/hector.rs187")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opciones.OperationFilter<FiltroAutorizacion>();

    //opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new string[]{}
    //    }
    //});
});


var app = builder.Build();

//ÁREA DE MIDDLEWARES

//Errores
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandlerFeature?.Error!;

    var error = new Error()
    {
        MensajeDeError = excepcion.Message,
        StrackTrace = excepcion.StackTrace,
        Fecha = DateTime.UtcNow
    };

    var dbContext = context.RequestServices.GetRequiredService<ApplicationDBContext>();
    dbContext.Add(error);
    await dbContext.SaveChangesAsync();
    await Results.InternalServerError(new
    {
        tipo = "error",
        mensaje = "Ha ocurrido un error inesperado",
        estatus = 500
    }).ExecuteAsync(context);
}));

//Swagger
app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "EduCore API v1");
    opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "EduCore API v2");
});

app.UseStaticFiles(); //Usar Archivos estaticos en el wwwroot - Imagenes

app.UseCors();

app.UseOutputCache();

app.MapControllers();

app.Run();

public partial class Program { }
