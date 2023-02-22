using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.API.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;

namespace MvcMovie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;

        public MoviesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
        }

        [HttpPost("Paginate")]
        public async Task<JsonResult> Paginate([FromBody] BootstrapModel model)
        {
            model.Limit = model.Limit.HasValue && model.Limit != 0 ? model.Limit.Value : int.MaxValue;

            var query = _movieRepository.Get();

            var genreName = model.Search.FirstOrDefault(x => x.Name == "Genre");

            if (genreName != null && !string.IsNullOrEmpty(genreName.Value))
            {
                query = query.Where(x => x.Genre.Name.ToUpper().StartsWith(genreName.Value.ToUpper()));
            }

            var title = model.Search.FirstOrDefault(x => x.Name == "Title");
            if (title != null && !string.IsNullOrEmpty(title.Value))
            {
                query = query.Where(x => x.Title.ToUpper().StartsWith(title.Value.ToUpper()));
            }

            query = query.Include(x => x.Genre);

            var result = await query.Skip(model.Offset).Take(model.Limit.Value).Select(x => new
            {
                x.Id,
                x.Title,
                x.ReleaseDate,
                GenreName = x.Genre.Name,
                x.Price,
                x.Rating
            }).ToListAsync();

            var _count = query.Count();

            return Json(new
            {
                rows = result,
                total = _count,
                //To fix
                totalNotFiltered = _count
            });
        }
    }
}
