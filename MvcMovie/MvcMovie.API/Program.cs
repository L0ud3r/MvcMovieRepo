using Microsoft.EntityFrameworkCore;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using MvcMovieInfra;

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

            // Add services to the container.

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
        }
    }
}