using BeeMart.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BeeMartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BeeMartDb")));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
