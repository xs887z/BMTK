using Microsoft.AspNetCore.Components.Authorization;
using Obrasheniya.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ObrasheniyaService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapGet("/", () => Results.File("wwwroot/index.html", "text/html"));

app.MapControllers();

app.Run();

//см заметки 1
