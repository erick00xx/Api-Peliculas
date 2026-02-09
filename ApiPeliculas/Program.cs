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
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AplicationDBContext>(opciones =>
                // opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));
                opciones.UseNpgsql(builder.Configuration.GetConnectionString("ConexionSql")));

builder.Services.AddControllers(option =>
{
    //CACHE profile . Una cache global y asi no tener que ponerlo en todas partes
    option.CacheProfiles.Add("PorDefecto30Segundos", new CacheProfile(){ Duration = 30});
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = 
                "Autenticacion JWT usando el esquema Bearer. \r\n\r\n" +
                "Ingresa la palabra 'Bearer' seguido de un [espacio] y despues su token en el campo de abajo. \r\n\r\n" + 
                "Ejemplo : \"Bearer tkljlse24fi\"", 
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            }
        ); 
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                }
            );
        options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1.0",
                Title = "PeliculasApi V1",
                Description = "Api de Peliculas",
                TermsOfService = new Uri("https://youtube.com"),
                Contact = new OpenApiContact
                {
                    Name = "erick",
                    Url = new Uri("https://youtube.com")
                },
                License = new OpenApiLicense
                {
                    Name = "Licencia Personal",
                    Url = new Uri("https://youtube.com")
                }
            }
            

            );
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2.0",
            Title = "PeliculasApi V2",
            Description = "Api de Peliculas",
            TermsOfService = new Uri("https://youtube.com"),
            Contact = new OpenApiContact
            {
                Name = "erick",
                Url = new Uri("https://youtube.com")
            },
            License = new OpenApiLicense
            {
                Name = "Licencia Personal",
                Url = new Uri("https://youtube.com")
            }
        }
        );
    }
);


//Soporte para CORS
//Se pueden habilitar 1-Um dominio, 2-multiples dominios,
//3-cualquier dominio ( tener CUenta seguridad)
//Usamos de ejemplo el dominio: http://localhost:3223, se debe cambiar por el correcto
//Se usa (*) para todos los dominios

builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
}));


//Soporte para CACHE
builder.Services.AddResponseCaching();




//agregamos los repositoriosss
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");


//Soporte para versiones API
var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1,0);
    option.ReportApiVersions = true;
    // option.ApiVersionReader = ApiVersionReader.Combine(
    //     new QueryStringApiVersionReader("api-version")//?api-version=1.0
    // );
});

apiVersioningBuilder.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true; //para parametrizar de otra forma la verion, ya no query
    }
);


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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}

// app.UseHttpsRedirection();

//Soporte para Autenticacion (ORDEN IMPORTANTE, primero autentica, luego autoriza)
app.UseAuthentication();
app.UseAuthorization();

//Soporte para CORS
app.UseCors("PoliticaCors");

app.MapControllers();

app.Run();
