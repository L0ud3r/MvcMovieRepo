using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMovieDAL.Entities;
using MvcMovieDAL;

namespace MvcMovie.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Favourite> _favouriteRepository;

        public MoviesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository, IRepository<Favourite> favouriteRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _favouriteRepository = favouriteRepository;
        }

        // GET: FavouritesController
        public ActionResult Index()
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = (from m in _movieRepository.Get()
                                             orderby m.Genre.Name
                                             select m.Genre.Name).AsQueryable();

            var movies = (from m in _movieRepository.Get()
                          select m).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre.Name == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.Select(x => new MovieViewModel()
                {
                    Genre = x.Genre.Name,
                    Title = x.Title,
                    Id = x.Id,
                    Rating = x.Rating,
                    ReleaseDate = x.ReleaseDate,
                    Price = x.Price,
                }).ToListAsync()
            };

            return View(movieGenreVM);
            return View();
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
            return View();
        }

        // POST: FavouritesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
