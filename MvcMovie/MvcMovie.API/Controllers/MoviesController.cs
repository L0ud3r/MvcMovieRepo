using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.API.Models;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id < 0 || !_movieRepository.Exists(id))
            {
                return NotFound();
            }

            var movie = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).Select(x => new
            {
                x.Id,
                x.Title,
                x.ReleaseDate,
                GenreName = x.Genre.Name,
                x.Price,
                x.Rating
            }).SingleOrDefault();

            MovieViewModel movieDetails = new MovieViewModel();

            movieDetails.Id = movie.Id;
            movieDetails.Title = movie.Title;
            movieDetails.ReleaseDate = movie.ReleaseDate.Date;
            movieDetails.Rating = movie.Rating;
            movieDetails.Price = movie.Price;
            movieDetails.Genre = movie.GenreName;

            //string jsonObject = JsonSerializer.Serialize(movieDetails);

            return new JsonResult(movieDetails);
        }

        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ReleaseDate,Genre,Price,Rating")] MovieViewModel movie)
        {
            if (ModelState.IsValid)
            {
                Movie movieInsert = new Movie();

                movieInsert.Title = movie.Title;
                movieInsert.ReleaseDate = movie.ReleaseDate;
                movieInsert.Rating = movie.Rating;
                movieInsert.Price = movie.Price;

                Genre genre = _genreRepository.Get().Where(x => x.Name == movie.Genre).SingleOrDefault();

                if (genre == null)
                {
                    movieInsert.Genre = new Genre();
                    movieInsert.Genre.Name = movie.Genre;
                }
                else
                    movieInsert.GenreId = genre.Id;

                movieInsert.CreatedDate = DateTime.Now;
                movieInsert.UpdatedDate = DateTime.Now;
                movieInsert.Active = true;

                _movieRepository.Insert(movieInsert);
                _movieRepository.Save();

                var addedMovie = _genreRepository.Get().OrderBy(x => x.Id).LastOrDefault();

                //var genreViewModel = new MovieViewModel
                //{
                //    Id = movieInsert.Id,
                //    Title = movieInsert.Title,
                //    Genre = movieInsert.Genre.Name,
                //    Rating = movieInsert.Rating,
                //    ReleaseDate = movieInsert.ReleaseDate,
                //    Price = movieInsert.Price
                //};

                //string jsonObject = JsonSerializer.Serialize(
                //    genreViewModel,
                //new JsonSerializerOptions
                //{
                //    ReferenceHandler = ReferenceHandler.Preserve
                //});

                //string jsonObject = JsonSerializer.Serialize(
                //    _genreRepository.Get().OrderBy(x => x.Id).LastOrDefault(),
                //new JsonSerializerOptions
                //{
                //    ReferenceHandler = ReferenceHandler.Preserve
                //});

                return new JsonResult(addedMovie);
                //return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        [HttpPatch]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] MovieViewModel movie)
        {
            if (_movieRepository.Exists(movie.Id) == false)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    Movie movieEdit = new Movie();

                    movieEdit.Id = movie.Id;
                    movieEdit.Title = movie.Title;
                    movieEdit.ReleaseDate = movie.ReleaseDate;
                    movieEdit.Rating = movie.Rating;
                    movieEdit.Price = movie.Price;

                    Genre genre = _genreRepository.Get().Where(x => x.Name == movie.Genre).SingleOrDefault();

                    if (genre == null)
                    {
                        movieEdit.Genre = new Genre();
                        movieEdit.Genre.Name = movie.Genre;
                    }
                    else
                        movieEdit.GenreId = genre.Id;

                    movieEdit.Active = true;
                    movieEdit.UpdatedDate = DateTime.Now;

                    _movieRepository.Update(movieEdit);
                    _movieRepository.Save();

                    //string jsonObject = JsonSerializer.Serialize(
                    //movieEdit,
                    //new JsonSerializerOptions
                    //{
                    //    ReferenceHandler = ReferenceHandler.Preserve
                    //});

                    return new JsonResult(movieEdit);
                    // Redirects to the list of movies (Movies/Index)
                    //return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NotFound();
            //return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpDelete("{id}"), ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_movieRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie' is null.");
            }

            var movie = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).SingleOrDefault();

            if (movie != null)
            {
                _movieRepository.Delete(id);
                _movieRepository.Save();

                return new JsonResult(true) { StatusCode = 200 };
            }

            return NotFound();
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
                query = query.Where(x => x.Title.ToUpper().Contains(title.Value.ToUpper()));
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

        private bool MovieExists(int id)
        {
            var entity = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).SingleOrDefault();

            if (entity != null)
                return true;

            return false;
        }
    }
}
