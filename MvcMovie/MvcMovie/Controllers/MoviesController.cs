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
        //    private readonly IRepository<Movie> _movieRepository;
        //    private readonly IRepository<Genre> _genreRepository;

        //    public MoviesController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository)
        //    {
        //        _movieRepository = movieRepository;
        //        _genreRepository = genreRepository;
        //    }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        //// GET: Movies
        //public async Task<IActionResult> Index(string movieGenre, string searchString)
        //{
        //    // Use LINQ to get list of genres.
        //    IQueryable<string> genreQuery = (from m in _movieRepository.Get()
        //                                     orderby m.Genre.Name
        //                                     select m.Genre.Name).AsQueryable();

        //    var movies = (from m in _movieRepository.Get()
        //                  select m).AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        movies = movies.Where(s => s.Title!.Contains(searchString));
        //    }

        //    if (!string.IsNullOrEmpty(movieGenre))
        //    {
        //        movies = movies.Where(x => x.Genre.Name == movieGenre);
        //    }

        //    var movieGenreVM = new MovieGenreViewModel
        //    {
        //        Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
        //        Movies = await movies.Select(x => new MovieViewModel()
        //        {
        //            Genre = x.Genre.Name,
        //            Title = x.Title,
        //            Id = x.Id,
        //            Rating = x.Rating,
        //            ReleaseDate = x.ReleaseDate,
        //            Price = x.Price,
        //        }).ToListAsync()
        //    };

        //    return View(movieGenreVM);
        //}

        // ATUALIZAR METODO PARA NAO ACEDER À BD
        // COM JS METER Genres NAS OPTIONS DO SELECT
        // GET: Movies/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModelMovie = new MovieViewModel();

            viewModelMovie.Id = id;

            return View(viewModelMovie);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var viewModelMovie = new MovieAllGenresViewModel();

            viewModelMovie.Id = id;

            return View(viewModelMovie);
        }

        //// ATUALIZAR METODO PARA NAO ACEDER À BD
        //// COM JS BUSCAR A INFO PARA METER NOS PLACEHOLDERS
        //// GET: Movies/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    if (id == null || _movieRepository.Get() == null)
        //    {
        //        return NotFound();
        //    }

        //    var movie = _movieRepository.Get().Where(x => x.Id == id).Include(x => x.Genre).Select(x => new
        //    {
        //        x.Id,
        //        x.Title,
        //        x.ReleaseDate,
        //        GenreName = x.Genre.Name,
        //        x.Price,
        //        x.Rating
        //    }).SingleOrDefault();

        //    // Use LINQ to get list of genres.
        //    IQueryable<string> genreQuery = (from m in _genreRepository.Get()
        //                                     orderby m.Name
        //                                     select m.Name).AsQueryable();

        //    //Verificar quando estiver vazio

        //    var movieGenreVM = new MovieAllGenresViewModel(
        //        new SelectList(genreQuery),
        //        movie.Id,
        //        movie.Title,
        //        movie.Price,
        //        movie.ReleaseDate,
        //        movie.Rating
        //    );

        //    movieGenreVM.Genre = movie.GenreName;

        //    return View(movieGenreVM);
        //}
    }
}
