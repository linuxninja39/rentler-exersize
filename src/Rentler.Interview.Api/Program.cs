using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Rentler.Interview.Api.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(logging => { logging.LoggingFields = HttpLoggingFields.All; });


builder.Services.AddDbContext<FoodContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabaseConnection"))
);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.WebHost.UseSerilog();

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);


builder.Services.AddControllers();
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