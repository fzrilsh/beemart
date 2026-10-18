using BeeMart.Api.Data;
using BeeMart.Api.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BeeMartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BeeMartDb")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BeeMartDbContext>();
    await DataSeeder.SeedAsync(db, CancellationToken.None);
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
