using ApiCatalogo.Context;
using ApiCatalogo.Extensions;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddTransient<IMeuServico, MeuServico>();

//Jeito de pegar info do arquivo de configura��o
/*var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave1"];*/

//C�digo pra desabilitar o [FromServices] ser impl�cito
/*builder.Services.Configure<ApiBehaviorOptions>(option =>
{
    option.DisableImplicitFromServicesParameters = true;
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Definir um middleware usando um request delegate
/*app.Use(async (context, next) =>
{
    //adicionar o c�digo antes do request
    await next(context);
    //adicionar o c�digo depois do request
});*/

app.MapControllers();

//Usado pra encerrar o pipeline de middleware e gerar uma resposta
/*app.Run(async (context) =>
{
    await context.Response.WriteAsync("Middleware final");
});*/
app.Run();
