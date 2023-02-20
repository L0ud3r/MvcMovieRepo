using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMovieDAL.Entities;
using MvcMovieDAL;
using MvcMovie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MvcMovieDAL.Migrations;

namespace MvcMovie.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Favourite> _favouriteRepository;
        private readonly IRepository<User> _userRepository;

        public FavouritesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository,
            IRepository<Favourite> favouriteRepository, IRepository<User> userRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _favouriteRepository = favouriteRepository;
            _userRepository = userRepository;
        }

        // GET: FavouritesController
        public async Task<ActionResult> Index(string movieGenre, string searchString)
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = (from m in _movieRepository.Get()
                                             orderby m.Genre.Name
                                             select m.Genre.Name).AsQueryable();

            //OBTER ID DO CLIENTE PELA TOKEN

            var movies = _favouriteRepository.Get().Where(x => x.User.Id == 2).Include(x => x.Movie).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Movie.Title!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Movie.Genre.Name == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.Select(x => new MovieViewModel()
                {
                    Genre = x.Movie.Genre.Name,
                    Title = x.Movie.Title,
                    Id = x.Movie.Id,
                    Rating = x.Movie.Rating,
                    ReleaseDate = x.Movie.ReleaseDate,
                    Price = x.Movie.Price,
                }).ToListAsync()
            };

            return View(movieGenreVM);
        }

        public ActionResult Update(int movieId, string action)
        {
            //BUSCAR ID PELA TOKEN
            var userId = 2;

            var favorite = _favouriteRepository.Get().SingleOrDefault(f => f.Movie.Id == movieId && f.User.Id == userId);

            if (favorite == null && action == "add")
            {
                // Add the movie to the user's favorites
                var newFavorite = new Favourite {  };

                newFavorite.Movie = _movieRepository.Get().Where(x => x.Id == movieId).SingleOrDefault();
                newFavorite.User = _userRepository.Get().Where(x => x.Id == userId).SingleOrDefault();

                _favouriteRepository.Insert(newFavorite);
                _favouriteRepository.Save();
            }
            else if (favorite != null && action == "remove")
            {
                // Remove the movie from the user's favorites
                _favouriteRepository.Delete(favorite.Id);
                _favouriteRepository.Save();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult<bool> IsFavourite(int movieId)
        {
            var userId = 2;
            var isFavourite = _favouriteRepository.Get().Any(f => f.Movie.Id == movieId && f.User.Id == userId);

            return Json(new { success = isFavourite });
        }

        // GET: FavouritesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FavouritesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FavouritesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FavouritesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FavouritesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FavouritesController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null || _favouriteRepository.Get() == null)
            {
                return NotFound();
            }

            //TOKEN
            var userId = 2;

            var movieFav = _favouriteRepository.Get().Where(x => x.Movie.Id == id && x.User.Id == userId).SingleOrDefault();

            MovieViewModel movieView = new MovieViewModel();

            movieView.Id = movieFav.Movie.Id;
            movieView.Title = movieFav.Movie.Title;
            movieView.Price = movieFav.Movie.Price;
            movieView.ReleaseDate = movieFav.Movie.ReleaseDate;
            movieView.Rating = movieFav.Movie.Rating;
            movieView.Genre = movieFav.Movie.Genre.Name;

            return View(movieView);
        }

        // POST: FavouritesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_favouriteRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Favourites' is null.");
            }

            //OBTER PELA TOKEN
            var userId = 2;

            var movieFav = _favouriteRepository.Get().Where(x => x.Movie.Id == id && x.User.Id == userId).SingleOrDefault();

            if (movieFav != null)
            {
                _favouriteRepository.Delete(movieFav.Id);
            }

            _movieRepository.Save();
            return RedirectToAction("Index", "Favourites");
        }

        [HttpPost]
        public async Task<JsonResult> Paginate([FromBody] BootstrapModel model)
        {
            model.Limit = model.Limit.HasValue ? model.Limit.Value : int.MaxValue;

            var query = _favouriteRepository.Get().Where(x => x.User.Id == 2).Include(x => x.Movie).AsQueryable();

            var genreName = model.Search.FirstOrDefault(x => x.Name == "Genre");

            if (genreName != null && !string.IsNullOrEmpty(genreName.Value))
            {
                query = query.Where(x => x.Movie.Genre.Name.ToUpper().StartsWith(genreName.Value.ToUpper()));
            }

            var title = model.Search.FirstOrDefault(x => x.Name == "Title");
            if (title != null && !string.IsNullOrEmpty(title.Value))
            {
                query = query.Where(x => x.Movie.Title.ToUpper().StartsWith(title.Value.ToUpper()));
            }

            query = query.Include(x => x.Movie.Genre);

            var result = await query.Skip(model.Offset).Take(model.Limit.Value).Select(x => new
            {
                x.Movie.Id,
                x.Movie.Title,
                x.Movie.ReleaseDate,
                GenreName = x.Movie.Genre.Name,
                x.Movie.Price,
                x.Movie.Rating
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
