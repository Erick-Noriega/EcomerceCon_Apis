using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var mpAccesToken = builder.Configuration["MercadoPago:AccesToken"];
if (!string.IsNullOrEmpty(mpAccesToken))
{
    MercadoPagoConfig.AccessToken = mpAccesToken;
}

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var secretKey = builder.Configuration
    .GetSection("Jwt")
    .GetValue<string>("SecretKey");

var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    config.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;

    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateIssuer = false,
        ValidateAudience = false,

        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5023")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<ProductosDbcontext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));
    builder.Services.AddDefaultIdentity<Usuario>(o =>
    {
        o.SignIn.RequireConfirmedAccount = false;
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
        o.Password.RequiredLength = 6;
    }).AddEntityFrameworkStores<ProductosDbcontext>();
builder.Services.AddScoped<IWhatsappService,WhattsapServices>();
builder.Services.AddScoped<IPagoService,PagosServices>();
builder.Services.AddScoped<IProductosService, ProductoService>();
builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();
app.UseCors("AllowFrontend");
app.MapGet("/", (HttpContext context) =>
    {
        context.Response.Redirect("/swagger/index.html", permanent: false);

    });
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI();
        app.UseSwagger();
    }
app.UseAuthentication();
app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    foreach (var item in ex.LoaderExceptions)
    {
        // Imprime en la ventana "Salida" de Visual Studio el detalle exacto
        System.Diagnostics.Debug.WriteLine($"---> ERROR DETALLADO: {item?.Message}");
    }
    throw;
}


app.Run();

