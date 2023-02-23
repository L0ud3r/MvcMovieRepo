using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.API.Models;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;

namespace MvcMovie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenreController : Controller
    {
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Movie> _movieRepository;

        public GenreController(IRepository<Genre> genreRepository, IRepository<Movie> movieRepository)
        {
            _genreRepository = genreRepository;
            _movieRepository = movieRepository;

        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IQueryable<string> genreQuery = (from m in _genreRepository.Get()
                                             orderby m.Name
                                             select m.Name).AsQueryable();

            var usedGenres = new SelectList(await genreQuery.Distinct().ToListAsync());

            return new JsonResult(usedGenres);
        }

        [HttpGet("UsedGenres")]
        public async Task<IActionResult> UsedGenres()
        {
            IQueryable<string> genreQuery = (from m in _movieRepository.Get()
                                                orderby m.Genre.Name
                                                select m.Genre.Name).AsQueryable();

            var usedGenres = new SelectList(await genreQuery.Distinct().ToListAsync());

            return new JsonResult(usedGenres);
        }

        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GenreViewModel genre)
        {
            if (ModelState.IsValid)
            {
                Genre genreInsert = new Genre();

                var genreDb = _genreRepository.Get().Where(x => x.Name == genre.Name).SingleOrDefault();

                if (genreDb == null)
                {
                    genreInsert.Name = genre.Name;

                    genreInsert.CreatedDate = DateTime.Now;
                    genreInsert.UpdatedDate = DateTime.Now;
                    genreInsert.Active = true;

                    _genreRepository.Insert(genreInsert);
                    _genreRepository.Save();

                    var addedGenre = _genreRepository.Get().OrderBy(x => x.Id).LastOrDefault();

                    return new JsonResult(addedGenre);
                }
                /*
                * 
                * 
                ALERT USER!!!
                *
                *
                */

                return BadRequest("Genre already exists!");
            }
            return View(genre);
        }

        [HttpPost("Delete"), ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (_genreRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Genre' is null.");
            }

            var genre = _genreRepository.Get().Where(x => x.Id == id).SingleOrDefault();

            if (genre != null)
            {
                _genreRepository.Delete(id);
            }

            _genreRepository.Save();
            return Ok();
        }

        [HttpPost("Paginate")]
        public async Task<JsonResult> Paginate([FromBody] BootstrapModel model)
        {
            model.Limit = model.Limit.HasValue && model.Limit != 0 ? model.Limit.Value : int.MaxValue;

            var query = _genreRepository.Get();

            var genreName = model.Search.FirstOrDefault(x => x.Name == "Genre");
            if (genreName != null && !string.IsNullOrEmpty(genreName.Value))
            {
                query = query.Where(x => x.Name.ToUpper().StartsWith(genreName.Value.ToUpper()));
            }

            var result = await query.Skip(model.Offset).Take(model.Limit.Value).Select(x => new
            {
                x.Id,
                x.Name
            }).ToListAsync();

            var _count = query.Count();

            return Json(new
            {
                rows = result,
                total = _count,
                totalNotFiltered = _count
            });
        }
    }
}
