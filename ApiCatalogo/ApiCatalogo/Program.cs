using ApiCatalogo.Context;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Extensions;
using ApiCatalogo.Filters;
using ApiCatalogo.Logging;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();

//var OrigensComAcessoPermitido = "_origensComAcessoPermitido";
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: OrigensComAcessoPermitido, policy =>
//    {
//        policy.WithOrigins("https://apirequest.io").WithMethods("GET", "POST");
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigensComAcessoPermitido", policy =>
    {
        policy.WithOrigins("https://localhost:7022").WithMethods("GET", "POST").AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT ",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{ }
        }
    });

});

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key");
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "pallas"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(
        context => context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "pallas") || 
                                                    context.User.IsInRole("SuperAdmin")
        )
    );
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddTransient<IMeuServico, MeuServico>();

builder.Services.AddScoped<ApiLoggingFilter>();

builder.Services.AddScoped<ISpecificCategoriaRepository, SpecificCategoriaRepository>();
builder.Services.AddScoped<ISpecificProdutoRepository, SpecificProdutoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

//Jeito de pegar info do arquivo de configuração
/*var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave1"];*/

//Código pra desabilitar o [FromServices] ser implícito
/*builder.Services.Configure<ApiBehaviorOptions>(option =>
{
    option.DisableImplicitFromServicesParameters = true;
});*/

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration()
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseCors(/*OrigensComAcessoPermitido*/);

app.UseAuthorization();

//Definir um middleware usando um request delegate
/*app.Use(async (context, next) =>
{
    //adicionar o código antes do request
    await next(context);
    //adicionar o código depois do request
});*/

app.MapControllers();

//Usado pra encerrar o pipeline de middleware e gerar uma resposta
/*app.Run(async (context) =>
{
    await context.Response.WriteAsync("Middleware final");
});*/
app.Run();
