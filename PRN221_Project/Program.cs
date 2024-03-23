using Microsoft.EntityFrameworkCore;
using PRN221_Project.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PRN221_ProjectContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MyConStr"))
    );

var app = builder.Build();

app.MapRazorPages();

app.Run();
