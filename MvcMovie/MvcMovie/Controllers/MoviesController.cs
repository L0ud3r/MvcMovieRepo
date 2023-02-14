using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Genre> _genreRepository;

        public MoviesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = (from m in _movieRepository.Get()
                                             orderby m.Title
                                             select m.Title).AsQueryable();

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

            //Return entire list in database
            //return View(await _context.Movie.ToListAsync());
        }

        [HttpPost]
        public string Index(string search, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + search;
        }

        // GET: Movies/Details/5
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
            movieDetails.ReleaseDate = movie.ReleaseDate;
            movieDetails.Rating = movie.Rating;
            movieDetails.Price = movie.Price;
            movieDetails.Genre = movie.GenreName;

            return View(movieDetails);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] MovieViewModel movie)
        {
            if (ModelState.IsValid)
            {
                Movie movieInsert = new Movie();

                movieInsert.Title = movie.Title;
                movieInsert.ReleaseDate = movie.ReleaseDate;
                movieInsert.Rating = movie.Rating;
                movieInsert.Price = movie.Price;

                Genre genre = _genreRepository.Get().Where(x => x.Name == movie.Genre).SingleOrDefault();

                if (genre == null){
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
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _movieRepository.Get() == null)
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

            MovieViewModel movieView = new MovieViewModel();

            movieView.Id = movie.Id;
            movieView.Title = movie.Title;
            movieView.Price = movie.Price;
            movieView.ReleaseDate = movie.ReleaseDate;
            movieView.Rating = movie.Rating;
            movieView.Genre = movie.GenreName;

            return View(movieView);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] MovieViewModel movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

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

                    movieEdit.UpdatedDate = DateTime.Now;

                    _movieRepository.Update(movieEdit);
                    _movieRepository.Save();
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
                // Redirects to the list of movies (Movies/Index)
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _movieRepository.Get() == null)
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

            MovieViewModel movieView = new MovieViewModel();

            movieView.Id = movie.Id;
            movieView.Title = movie.Title;
            movieView.Price = movie.Price;
            movieView.ReleaseDate = movie.ReleaseDate;
            movieView.Rating = movie.Rating;
            movieView.Genre = movie.GenreName;

            return View(movieView);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_movieRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }

            var movie = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).SingleOrDefault();

            if (movie != null)
            {
                _movieRepository.Delete(id);
            }

            _movieRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            var entity = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).SingleOrDefault();

            if(entity != null)
                return true;

            return false;
        }
    }
}
