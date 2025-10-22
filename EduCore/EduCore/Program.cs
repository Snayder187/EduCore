using EduCore.Datos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//ÁREA DE SERVICIOS
builder.Services.AddAutoMapper(typeof(Program));
//El agregar el AddJsonOptions es momentaneo y sirve para liberar el ciclo de tablas
builder.Services.AddControllers().AddNewtonsoftJson();

//Aquí va la cadena de conexión
builder.Services.AddDbContext<ApplicationDBContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection"));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//ÁREA DE MIDDLEWARES
app.MapControllers();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
