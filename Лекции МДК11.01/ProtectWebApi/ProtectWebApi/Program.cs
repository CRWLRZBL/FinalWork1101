using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProtectWebApi.Data;
using ProtectWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//jwt
builder.Services.AddScoped<TokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new()
    { 
        ValidateIssuer = false,
        //ValidIssuer = "http://lovalhost:5020", // сервер, выпустивший токен, builder.Configuration["JWT:Issuer]
        ValidateAudience = false,
        //ValidAudience = "http://lovalhost:5020", // где применяется токен, builder.Configuration["JWT:Audience]
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    }



    );

builder.Services.AddControllers();
builder.Services.AddDbContext<GameContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
