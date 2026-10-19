using SerieA.API.Data;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;

var builder = WebApplication.CreateBuilder(args);

// Controller desteði
builder.Services.AddControllers();

// SQL Server veritabaný baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Geliþtirme ortamýnda Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await DataSeeder.SeedAsync(context);
}


app.Run();
