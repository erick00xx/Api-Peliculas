using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiPeliculas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AplicationDBContext>(opciones =>
                // opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));
                opciones.UseNpgsql(builder.Configuration.GetConnectionString("ConexionSql")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Soporte para CORS
//Se pueden habilitar 1-Um dominio, 2-multiples dominios,
//3-cualquier dominio ( tener CUenta seguridad)
//Usamos de ejemplo el dominio: http://localhost:3223, se debe cambiar por el correcto
//Se usa (*) para todos los dominios

builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
}));







//agregamos los repositoriosss
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");


//agregar el automapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Aqui se configura la autenticacion
builder.Services.AddAuthentication
(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(x=>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

//Soporte para Autenticacion (ORDEN IMPORTANTE, primero autentica, luego autoriza)
app.UseAuthentication();
app.UseAuthorization();

//Soporte para CORS
app.UseCors("PoliticaCors");

app.MapControllers();

app.Run();
