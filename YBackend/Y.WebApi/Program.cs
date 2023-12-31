using Microsoft.EntityFrameworkCore;
using Y.Infrastructure;
using Y.Infrastructure.Extensions;
using Y.Application.Extensions;
using Y.WebApi.Middlewares;
using Y.Application.ConfigurationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Y.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

ConnectionStringModel connectionStringModel = new(builder.Configuration.GetConnectionString("database"));
builder.Services.AddSingleton<ConnectionStringModel>(connectionStringModel);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Y Web Api",
        Version = "v1",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);

var jwtModel = new JwtModel(
    issuer: builder.Configuration.GetSection("Jwt:Issuer").Value,
    audience: builder.Configuration.GetSection("Jwt:Audience").Value,
    tokenKey: builder.Configuration.GetSection("Jwt:TokenKey").Value
);
builder.Services.AddSingleton<JwtModel>(jwtModel);

builder.Services.AddYInfrastructure();
builder.Services.AddYApplication();

builder.Services.AddCors();

builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtModel.Issuer,
        ValidAudience = jwtModel.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(jwtModel.TokenBytes)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x =>
{
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.SetIsOriginAllowed(origin => true);
    x.AllowCredentials();
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
