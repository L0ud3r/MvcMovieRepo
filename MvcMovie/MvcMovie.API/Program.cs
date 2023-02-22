using Microsoft.EntityFrameworkCore;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using MvcMovieInfra;
using Newtonsoft.Json;

namespace MvcMovie.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<MvcMovieContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found.")));

            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie, MvcMovieContext>>();
            builder.Services.AddScoped<IRepository<Genre>, Repository<Genre, MvcMovieContext>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User, MvcMovieContext>>();
            builder.Services.AddScoped<IRepository<Favourite>, Repository<Favourite, MvcMovieContext>>();

            builder.Services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(
                options => {
                    options.SerializerSettings.MaxDepth = 3;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            var app = builder.Build();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();
        }
    }
}