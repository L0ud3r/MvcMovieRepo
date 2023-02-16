using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using MvcMovieInfra;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found.")));

builder.Services.AddScoped<IRepository<Movie>, Repository<Movie, MvcMovieContext>>();
builder.Services.AddScoped<IRepository<Genre>, Repository<Genre, MvcMovieContext>>();
builder.Services.AddScoped<IRepository<User>, Repository<User, MvcMovieContext>>();

// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorOptions(options => options.AllowRecompilingViewsOnFileChange.Date = true);

//#if (DEBUG)
//mvcviews.AddRazorRuntimeCompilation();
//#endif

var mvcviews = builder.Services.AddControllersWithViews();

#if (DEBUG)
    mvcviews.AddRazorRuntimeCompilation();
#endif

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
