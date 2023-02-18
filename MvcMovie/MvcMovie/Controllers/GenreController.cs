using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using MvcMovieDAL;
using MvcMovieDAL.Entities;
using System.Web;


namespace MvcMovie.Controllers
{
    public class GenreController : Controller
    {
        private readonly IRepository<Genre> _genreRepository;

        public GenreController(IRepository<Movie> movieRepository, IRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;

        }

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre)
        {
            // Use LINQ to get list of genres.
            var genreQuery = (from m in _genreRepository.Get()
                              orderby m.Name
                              select m).AsQueryable();

            if (!string.IsNullOrEmpty(movieGenre))
            {
                genreQuery = genreQuery.Where(s => s.Name!.StartsWith(movieGenre));
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync())
            };

            return View(movieGenreVM);
        }

        // GET: Genre/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                }
                else
                {
                    /*
                     * 
                     * 
                        ALERT USER!!!
                     *
                     *
                     */
                }

                return RedirectToAction("Index", "Genre");
            }
            return View(genre);
        }

        // POST: Genre/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (_genreRepository.Get() == null)
            {
                return Problem("Entity set 'MvcMovieContext.Genre' is null.");
            }

            var genre = _genreRepository.Get().Where(x => x.Id == id).SingleOrDefault();
            int x = id;
            if (genre != null)
            {
                _genreRepository.Delete(id);
            }

            _genreRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> Paginate([FromBody] BootstrapModel model)
        {
            model.Limit = model.Limit.HasValue ? model.Limit.Value : int.MaxValue;

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
