using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
//builder.Services.AddDbContext<MoviesContext>(
//    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MyConStr"))
//    );

var app = builder.Build();

app.MapRazorPages();

app.Run();
