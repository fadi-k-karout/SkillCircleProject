using Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Web.ProgramExtensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; 
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Use extension methods to add custom configurations

builder.Services.AddCustomDatabase(configuration);
builder.Services.AddCustomIdentity();
builder.Services.AddCustomAuthentication(configuration);
builder.Services.AddCustomAuthorization();
builder.Services.AddApplicationServices();
builder.Services.AddCustomSwagger();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidators.CreateUserDtoValidator>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddControllers();
builder.Services.AddCustomValidationApiBehaviour();

builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Seed the database
/*using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}*/


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


