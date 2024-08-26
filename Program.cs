using Microsoft.EntityFrameworkCore;
using Helsi_TestTask.Models;
using Helsi_TestTask.Models.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


/// DbContextOptionsBuilder opt = new DbContextOptionsBuilder();
/// opt.UseMongoDB("mongodb://localhost:27017", "TestTaskForHelsi"));
/// builder.Services.AddDbContext<DefaultContext, MongoContext>(opt);
///

builder.Services.AddSingleton<IDefaultContext>(new MongoContext(new DbContextOptionsBuilder<MongoContext>().UseMongoDB("mongodb://localhost:27017", "TestTaskForHelsi").Options));

builder.Services.AddSingleton<IDefaultAPI, APIRealization>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
