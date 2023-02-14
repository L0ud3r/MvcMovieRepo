using Microsoft.EntityFrameworkCore;
using MvcMovieDAL.Entities;

namespace MvcMovieDAL
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<Genre> Genres { get; set; } = default!;
    }
}
