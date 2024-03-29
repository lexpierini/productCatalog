using APICatalogo.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using productCatalog.Context;
using productCatalog.DTOs.Mappings;
using productCatalog.Filter;
using productCatalog.Logging;
using productCatalog.Repository;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // API details
    options.SwaggerDoc("v1", new OpenApiInfo { 
        Version = "v1",
        Title = "ProductCatalog.API",
        Description = "Product and Category Catalog",
        TermsOfService = new Uri("https://github.com/lexpierini"),
        Contact = new OpenApiContact { 
            Name = "Alex Pierini",
            Email = "alex_pierini@hotmail.com",
            Url = new Uri("https://github.com/lexpierini"),
        },
        License = new OpenApiLicense { 
            Name = "",
            Url = new Uri("https://github.com/lexpierini")
        },
    });

    // XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Enable token authorization using Swagger (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme." +
        "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
        "\r\n\r\n Example: \"Bearer 12345abcdef\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Enabling CORS via Attribute
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("MyPolicy",
//        builder => builder
//            .WithOrigins("https://www.apirequest.io/") // Used for specific origins
//            .WithMethods("GET") // Used for specific methods
//    );
//});

// Enabling CORS via Midleware
builder.Services.AddCors();


builder.Services.AddScoped<ApiLoggingFilter>();

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

// AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Database Connection Setup
string postgreSqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(postgreSqlConnection));

// Setup of security standards (Identity)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
                     ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
                 });


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Error Handling Midleware
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

// Setup of security standards (Authentication Midleware)
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


// Enabling CORS via Midleware - Restrictive
//app.UseCors(option => option
//    .WithOrigins("https://www.apirequest.io/") // Used for specific origins
//    .WithMethods("GET") // Used for specific methods
//);

// Enabling CORS via Midleware - non-restrictive
app.UseCors(options => options.AllowAnyOrigin());


app.MapControllers();
app.Run();
